using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class ShapeGrammarGenerator : MonoBehaviour
{
    enum TileType
    {
        wall, empty
    }

    [SerializeField]
    private GameObject[] rooms;

    [SerializeField]
    private GameObject[] frames;

    [SerializeField]
    private GameObject character;

    private TileType[][] tiles;

    private GameObject start;
    private string lhs;
    private string[] rRhs;
    private string[] xRhs;

    private int width, height;
    private Random random = new Random();

    private RoomEndPoint roomEndPoint;
    private Vector2 startPosition;

    private GameObject boardHolder;

    float leftEdgeX;
    float rightEdgeX;
    float bottomEdgeY;
    float topEdgeY;

    // Use this for initialization
    void Start()
    {
        boardHolder = new GameObject("BoardHolder");
        width = height = 45;
        InstantiateOuterWalls();
        start = Instantiate(rooms[0], new Vector2(0, 0), Quaternion.identity);
        start.GetComponentInChildren<SpriteRenderer>().color = Color.green;
        roomEndPoint = start.GetComponent<RoomEndPoint>();
        startPosition = roomEndPoint.GetEndPosition();
        rRhs = new string[8];
        //xRhs = new string[2];
        lhs = "R";
        rRhs[0] = "R";
        rRhs[1] = "1RR";
        rRhs[2] = "2R";
        rRhs[3] = "3R";
        rRhs[4] = "4R";
        rRhs[5] = "2X";
        rRhs[6] = "xR";
        rRhs[7] = "E";

        //xRhs[0] = "xR";
        //xRhs[1] = "xR";

        bool isGenerationComplete = false;
        while (!isGenerationComplete)
            isGenerationComplete = ReadLHS();

        Debug.Log(2 + lhs);
        BuildLevel();

        //character = Instantiate(character, startPosition + new Vector2(-roomEndPoint.GetObjectWidth() / 2, 1), character.transform.rotation);
        //Camera camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        //camera.enabled = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ClearConsole();
            SceneManager.LoadScene("PCG_Rule_Level");
        }
    }

    static void ClearConsole()
    {
        // This simply does "LogEntries.Clear()" the long way:
        var logEntries = System.Type.GetType("UnityEditorInternal.LogEntries,UnityEditor.dll");
        var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        clearMethod.Invoke(null, null);
    }

    bool ReadLHS()
    {
        Vector2 nextStartposition = startPosition;
        List<Vector2> lhsPositions = new List<Vector2>();
        bool isLastRemoved = false;
        int i, j;
        i = j = 0;

        while (i < lhs.Length)
        {
            j++; //Saves the program from crashing if it ends up in an endless loop
            if (j == 1000)
            {
                lhs = "R";
                return false;
            }

            if (lhs[i] != 'R' && lhs[i] != 'X')
            {
                i++;
                continue;
            }

            char previous = i > 0 ? lhs[i - 1] : '2';

            int rhsSelection = i < 10 ? Random.Range(1, rRhs.Length - 2) : rRhs.Length - 1;
            if (lhs[i] == 'X')
                rhsSelection = 6;

            #region if-while section to make sure that you can't create object going back from where you came
            if (rhsSelection == 3 && previous == '4')
            {
                while (rhsSelection == 3)
                    rhsSelection = Random.Range(1, rRhs.Length - 2);
            }
            else if (rhsSelection == 4 && previous == '3')
            {
                while (rhsSelection == 4)
                    rhsSelection = Random.Range(1, rRhs.Length - 2);
            }
            if (rhsSelection == 1 && previous == '2')
            {
                while (rhsSelection == 1)
                    rhsSelection = Random.Range(1, rRhs.Length - 2);
            }
            else if (rhsSelection == 2 && previous == '1' || rhsSelection == 5 && previous == '1')
            {
                while (rhsSelection == 2 || rhsSelection == 5)
                    rhsSelection = Random.Range(1, rRhs.Length - 2);
            }
            else if ((rhsSelection == 1 || rhsSelection == 4) && previous == 'x')
            {
                while ((rhsSelection == 1 || rhsSelection == 4))
                    rhsSelection = Random.Range(2, 4);
            }
            #endregion


            lhsPositions.Add(nextStartposition); //Saves the previous position to a list at i-position
            nextStartposition += nextInstatiatePosition(rhsSelection) * 2; // * 2 to go from center of object to start/endposition
            if (rhsSelection == 6)
            {
                nextStartposition.y -= rooms[1].GetComponent<RoomEndPoint>().GetObjectHeight();
            }
            if (previous == 'x')
            {
                nextStartposition.y += rooms[1].GetComponent<RoomEndPoint>().GetObjectHeight();
            }

            //Debug.Log("OldStart: " + lhsPositions[i] + "    NextStart: " + nextStartposition);


            //-------------------------------------------- Checks if the next object will intersect with an edge ----------------------------------------------
            if (nextStartposition.x >= rightEdgeX || nextStartposition.x <= leftEdgeX || nextStartposition.y >= topEdgeY || nextStartposition.y <= bottomEdgeY)
            {
                Debug.Log("<color=purple>INTERSECT FRAME</color>");
                if (!isLastRemoved) //If it reaches an edge the first time, go one extra step back in the generation
                {
                    lhs = lhs.Remove(i - 1);
                    lhs = lhs.Insert(i - 1, "R");
                    lhsPositions.RemoveAt(i);
                    i--;
                }

                nextStartposition = lhsPositions[i];
                lhsPositions.RemoveAt(i);
                isLastRemoved = true;
                continue;
            }
            isLastRemoved = false;

            //Adds the new RHS rule to the list.
            if (lhs[i] == 'R')
            {
                lhs = lhs.Remove(i);
                lhs = lhs.Insert(i, rRhs[rhsSelection]);
            }
            if (lhs[i] == 'X')
            {
                lhs = lhs.Remove(i);
                lhs = lhs.Insert(i, rRhs[rhsSelection]);
            }

            i++;
        }
        return true;
    }

    void RandomizeGaps(GameObject o)
    {
        int chance = Random.Range(0, 11);
        if (chance <= 1)
        {
            o.GetComponentInChildren<SpriteRenderer>().enabled = false;
            o.GetComponentInChildren<BoxCollider2D>().enabled = false;
        }
    }

    void BuildLevel()
    {
        Vector2 instatiatePosition = startPosition;
        for (int i = 0; i < lhs.Length; i++)
        {
            if (lhs[i] == '1')
            {
                instatiatePosition += nextInstatiatePosition((int)char.GetNumericValue(lhs[i]));

                GameObject o = Instantiate(rooms[0], instatiatePosition, rooms[0].transform.rotation);
                instatiatePosition = o.GetComponent<RoomEndPoint>().GetStartPosition();

                RandomizeGaps(o);
            }
            if (lhs[i] == '2')
            {
                instatiatePosition += nextInstatiatePosition((int)char.GetNumericValue(lhs[i]));

                GameObject o = Instantiate(rooms[0], instatiatePosition, rooms[0].transform.rotation);
                instatiatePosition = o.GetComponent<RoomEndPoint>().GetEndPosition();

                RandomizeGaps(o);
            }
            if (lhs[i] == '3')
            {
                instatiatePosition += nextInstatiatePosition((int)char.GetNumericValue(lhs[i]));

                GameObject o = Instantiate(rooms[1], instatiatePosition, rooms[1].transform.rotation);
                instatiatePosition = o.GetComponent<RoomEndPoint>().GetStartPosition();

                RandomizeGaps(o);
            }
            if (lhs[i] == '4')
            {
                instatiatePosition += nextInstatiatePosition((int)char.GetNumericValue(lhs[i]));

                GameObject o = Instantiate(rooms[1], instatiatePosition, rooms[1].transform.rotation);
                instatiatePosition = o.GetComponent<RoomEndPoint>().GetEndPosition();

                RandomizeGaps(o);
            }
            if (lhs[i] == 'E')
            {
                //instatiatePosition += nextInstatiatePosition((int)char.GetNumericValue(lhs[i]));
                instatiatePosition += nextInstatiatePosition(System.Convert.ToInt32(lhs[i]));

                rooms[2] = Instantiate(rooms[2], instatiatePosition, rooms[2].transform.rotation);

            }
            if (lhs[i] == 'x')
            {
                instatiatePosition += nextInstatiatePosition(System.Convert.ToInt32(lhs[i]));

                GameObject o = Instantiate(rooms[3], instatiatePosition, rooms[3].transform.rotation);
                instatiatePosition = o.GetComponent<RoomEndPoint>().GetEndPosition();
                RandomizeGaps(o);
            }
        }
    }

    Vector2 nextInstatiatePosition(int index)
    {
        if (index == 1)
        {
            return new Vector2(-rooms[0].GetComponent<RoomEndPoint>().GetObjectWidth() / 2, 0); // minus because index 1 instatiate position is start (left side of object)
        }
        else if (index == 2 || index == 5)
        {
            return new Vector2(rooms[0].GetComponent<RoomEndPoint>().GetObjectWidth() / 2, 0); // plus because index 2 instatiate position is end (right side of object)
        }
        else if (index == 3)
        {
            return new Vector2(0, rooms[1].GetComponent<RoomEndPoint>().GetObjectHeight() / 2); // plus because index 3 instatiate position is start (top side of object)
        }
        else if (index == 4)
        {
            return new Vector2(0, -rooms[1].GetComponent<RoomEndPoint>().GetObjectHeight() / 2); // minus because index 4 instatiate position is end (bottom side of object)
        }
        else if (index == System.Convert.ToInt32('x') || index == 6)
        {
            return new Vector2(rooms[3].GetComponent<RoomEndPoint>().GetObjectWidth() / 2, 0);
        }


        return new Vector2(rooms[2].GetComponent<RoomEndPoint>().GetObjectWidth() / 2, 0); // if E
    }

    void InstantiateOuterWalls()
    {
        // The outer walls are one unit left, right, up and down from the board.
        leftEdgeX = transform.position.x - (rooms[0].GetComponentInChildren<SpriteRenderer>().bounds.size.x / 2) - (frames[0].GetComponent<SpriteRenderer>().bounds.size.x / 2);
        rightEdgeX = (width + rooms[0].GetComponentInChildren<SpriteRenderer>().bounds.size.x) - (frames[0].GetComponent<SpriteRenderer>().bounds.size.x);
        bottomEdgeY = transform.position.y - height / 2;
        topEdgeY = height / 2;

        // Instantiate both vertical walls (one on each side).
        InstantiateVerticalOuterWall(leftEdgeX, bottomEdgeY, topEdgeY);
        InstantiateVerticalOuterWall(rightEdgeX, bottomEdgeY, topEdgeY);

        // Instantiate both horizontal walls, these are one in left and right from the outer walls.
        InstantiateHorizontalOuterWall(leftEdgeX + 1f, rightEdgeX - 1f, bottomEdgeY);
        InstantiateHorizontalOuterWall(leftEdgeX + 1f, rightEdgeX - 1f, topEdgeY);
    }

    void InstantiateVerticalOuterWall(float xCoord, float startingY, float endingY)
    {
        // Start the loop at the starting value for Y.
        float currentY = startingY;

        // While the value for Y is less than the end value...
        while (currentY <= endingY)
        {
            // ... instantiate an outer wall tile at the x coordinate and the current y coordinate.
            InstantiateFromArray(frames, xCoord, currentY);

            currentY++;
        }
    }

    void InstantiateHorizontalOuterWall(float startingX, float endingX, float yCoord)
    {
        // Start the loop at the starting value for X.
        float currentX = startingX;

        // While the value for X is less than the end value...
        while (currentX <= endingX)
        {
            // ... instantiate an outer wall tile at the y coordinate and the current x coordinate.
            InstantiateFromArray(frames, currentX, yCoord);

            currentX++;
        }
    }

    void InstantiateFromArray(GameObject[] prefabs, float xCoord, float yCoord)
    {
        // Create a random index for the array.
        int randomIndex = Random.Range(0, prefabs.Length);

        // The position to be instantiated at is based on the coordinates.
        Vector3 position = new Vector3(xCoord, yCoord, 0f);

        // Create an instance of the prefab from the random index of the array.
        GameObject tileInstance = Instantiate(prefabs[randomIndex], position, Quaternion.identity) as GameObject;

        // Set the tile's parent to the board holder.
        tileInstance.transform.parent = boardHolder.transform;
    }


}
