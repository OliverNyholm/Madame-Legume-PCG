using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class ShapeGrammarGenerator : MonoBehaviour
{
    enum TileType
    {
        wall, empty
    }

    [SerializeField]
    private GameObject[] platforms, frames, fruits;

    [SerializeField]
    private GameObject character, spike;

    [SerializeField]
    private int spikeChance;

    [SerializeField]
    private string levelSaveNumber;

    private TileType[][] tiles;

    private GameObject start;
    private string lhs;
    private string[] rRhs;
    private string[] xRhs;

    private int width, height;
    private Random random = new Random();

    private RoomEndPoint roomEndPoint;
    private Vector2 startPosition;
    private List<Vector3> usablePlatformsPos = new List<Vector3>();

    private GameObject boardHolder;

    private List<GameObject> instantiatedObjects = new List<GameObject>();

    string fruitLHS;
    List<Transform> fruitPositions = new List<Transform>();
    List<Transform> bladetPositions = new List<Transform>();

    private List<bool> activePlatforms = new List<bool>();

    private int saviour = 0;

    float leftEdgeX;
    float rightEdgeX;
    float bottomEdgeY;
    float topEdgeY;

    Level bestLevel = new Level();

    // Use this for initialization
    void Start()
    {
        boardHolder = new GameObject("BoardHolder");
        width = height = 45;
        InstantiateOuterWalls();
        start = Instantiate(platforms[0], new Vector2(0, 0), Quaternion.identity);
        start.GetComponentInChildren<SpriteRenderer>().color = Color.green;
        instantiatedObjects.Add(start);
        activePlatforms.Add(true);
        usablePlatformsPos.Add(start.transform.FindChild("MovementArea").transform.position);
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
        rRhs[5] = "2RR";
        rRhs[6] = "RR";
        rRhs[7] = "E";

        //xRhs[0] = "xR";
        //xRhs[1] = "xR";

        bool isGenerationComplete = false;
        while (!isGenerationComplete)
            isGenerationComplete = ReadLHS();

        Debug.Log(2 + lhs);
        BuildLevel();
        Debug.Log("platform count: " + usablePlatformsPos.Count);
        List<bool> visitedList = new List<bool>();

        foreach (GameObject o in instantiatedObjects)
        {
            visitedList.Add(false);
        }
        visitedList[0] = true;
        bool endFound = false;
        CheckPlayabilityWithFruits(instantiatedObjects[0], 0, visitedList, ref endFound);
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
        if (Input.GetKeyDown(KeyCode.R))
        {
            while (lhs.IndexOf('G') != -1)
            {
                int pos = lhs.IndexOf('G');
                Destroy(instantiatedObjects[pos]);
                instantiatedObjects.RemoveAt(pos);
                activePlatforms.RemoveAt(pos);
                lhs = lhs.Remove(pos, 1);
                saviour = 0;
            }
            List<bool> visitedList = new List<bool>();

            foreach (GameObject o in instantiatedObjects)
            {
                visitedList.Add(false);
            }
            visitedList[0] = true;
            bool endFound = false;
            CheckPlayabilityWithFruits(instantiatedObjects[0], 0, visitedList, ref endFound);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            SaveToFile();
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            List<Transform> tempTransform = ES2.LoadList<Transform>("instantiatedObjects");
            string tempLHS = ES2.Load<string>("LHS");

            foreach (Transform t in tempTransform)
            {
                Debug.Log("Transform: " + t.position);
            }
        }
    }

    private void SaveToFile()
    {
        List<Transform> tempTransform = new List<Transform>();

        foreach (GameObject o in instantiatedObjects)
        {
            tempTransform.Add(o.transform);
        }

        try
        {
            ES2.Save(tempTransform, "instantiatedObjects" + levelSaveNumber);
            ES2.Save(lhs, "LHS" + levelSaveNumber);
            ES2.Save(fruitLHS, "fruitLHS" + levelSaveNumber);
            ES2.Save(activePlatforms, "activePlatforms" + levelSaveNumber);

            Debug.Log("Save successful");
        }
        catch (System.Exception)
        {
            Debug.Log("Save failed");
            throw;
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
                nextStartposition.y -= platforms[1].GetComponent<RoomEndPoint>().GetObjectHeight();
            }
            if (previous == 'x')
            {
                nextStartposition.y += platforms[1].GetComponent<RoomEndPoint>().GetObjectHeight();
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

    bool RandomizeGaps(GameObject o, int i)
    {
        int chance = Random.Range(0, 5);
        if (chance <= 1)
        {
            o.SetActive(false);
            activePlatforms.Add(false);
            return true;
        }
        else
        {
            if (Random.Range(0, 11) < 5)
                AddSpikes(o, 0, new List<int>(), i);

            activePlatforms.Add(true);
            return false;
        }
    }

    void BuildLevel()
    {
        int previousPlatformIndexPos = 0;
        Vector2 instatiatePosition = startPosition;
        for (int i = 0; i < lhs.Length; i++)
        {
            if (lhs[i] == '1')
            {
                instatiatePosition += nextInstatiatePosition((int)char.GetNumericValue(lhs[i]));

                GameObject o = Instantiate(platforms[0], instatiatePosition, platforms[0].transform.rotation);
                instatiatePosition = o.GetComponent<RoomEndPoint>().GetStartPosition();
                instantiatedObjects.Add(o);
                if (!RandomizeGaps(o, i))
                {
                    usablePlatformsPos.Add(o.transform.FindChild("MovementArea").transform.position);
                }
                previousPlatformIndexPos = i;
            }
            if (lhs[i] == '2')
            {
                instatiatePosition += nextInstatiatePosition((int)char.GetNumericValue(lhs[i]));

                GameObject o = Instantiate(platforms[0], instatiatePosition, platforms[0].transform.rotation);
                instatiatePosition = o.GetComponent<RoomEndPoint>().GetEndPosition();
                instantiatedObjects.Add(o);
                if (!RandomizeGaps(o, i))
                {
                    usablePlatformsPos.Add(o.transform.FindChild("MovementArea").transform.position);
                }
                previousPlatformIndexPos = i;
            }
            if (lhs[i] == '3')
            {
                instatiatePosition += nextInstatiatePosition((int)char.GetNumericValue(lhs[i]));

                GameObject o = Instantiate(platforms[1], instatiatePosition, platforms[1].transform.rotation);
                instatiatePosition = o.GetComponent<RoomEndPoint>().GetStartPosition();
                instantiatedObjects.Add(o);
                if (!RandomizeGaps(o, i))
                {
                    if (activePlatforms[activePlatforms.Count - 1] && lhs[previousPlatformIndexPos] == '3')
                    {
                        usablePlatformsPos.RemoveAt(usablePlatformsPos.Count - 1);
                    }
                    usablePlatformsPos.Add(o.transform.FindChild("MovementArea").transform.position);
                }
                previousPlatformIndexPos = i;
            }
            if (lhs[i] == '4')
            {
                instatiatePosition += nextInstatiatePosition((int)char.GetNumericValue(lhs[i]));

                GameObject o = Instantiate(platforms[1], instatiatePosition, platforms[1].transform.rotation);
                instatiatePosition = o.GetComponent<RoomEndPoint>().GetEndPosition();
                instantiatedObjects.Add(o);
                if (!RandomizeGaps(o, i))
                {
                    usablePlatformsPos.Add(o.transform.FindChild("MovementArea").transform.position);
                    if (activePlatforms[activePlatforms.Count - 1] && (lhs[previousPlatformIndexPos] == '4'))
                    {
                        usablePlatformsPos.RemoveAt(usablePlatformsPos.Count - 1);
                    }
                }
                previousPlatformIndexPos = i;
            }
            if (lhs[i] == 'E')
            {
                instatiatePosition += nextInstatiatePosition(System.Convert.ToInt32(lhs[i]));
                GameObject o = Instantiate(platforms[2], instatiatePosition, platforms[2].transform.rotation);
                instantiatedObjects.Add(o);
                activePlatforms.Add(true);
                usablePlatformsPos.Add(o.transform.FindChild("MovementArea").transform.position);
            }
        }
        CheckForAvailablePlatforms();
    }

    void CheckForAvailablePlatforms()
    {
        List<bool> visitedList = new List<bool>();
        //visitedList.Add(true);
        for (int i = 0; i < usablePlatformsPos.Count; i++)
            visitedList.Add(false);

        usablePlatformsPos = usablePlatformsPos.OrderBy(x => Vector2.Distance(usablePlatformsPos[0], x)).ToList();

        for (int i = 1; i < usablePlatformsPos.Count; i++)
        {
            if (!visitedList[i])
            {
                VisitPlatform(usablePlatformsPos[i], usablePlatformsPos[i - 1]);
                visitedList[i] = true;
            }
        }

        //for (int i = 0; i < usablePlatformsPos.Count; i++)
        //{
        //    Debug.Log("Usable list: " + usablePlatformsPos[i]);
        //}
        //Debug.Log("Usable Count: " + usablePlatformsPos.Count);

    }

    void VisitPlatform(Vector3 platformPos, Vector3 prevPlatformPos)
    {
        if (platformPos.x - 1.5f > prevPlatformPos.x + 6 && platformPos.y < prevPlatformPos.y - 6)
        {
            AddFruit("Banana", prevPlatformPos, platformPos);
        }
        else if ((platformPos.x - 1.5f > prevPlatformPos.x + 4 && platformPos.y > prevPlatformPos.y + 3) || platformPos.y > prevPlatformPos.y + 3)
        {
            AddFruit("Tomato", prevPlatformPos, platformPos);
        }
        else if (platformPos.x - 1.5f > prevPlatformPos.x + 6 && (platformPos.y > prevPlatformPos.y + 3 || platformPos.y == prevPlatformPos.y))
        {
            AddFruit("Carrot", prevPlatformPos, platformPos);
        }
    }

    void AddFruit(string fruitChoice, Vector3 currentPos, Vector3 nextPos)
    {
        int fruitIndex = 0;

        if (fruitChoice == "Banana")
        {
            fruitIndex = 0;
            fruitLHS += "B";
        }
        else if (fruitChoice == "Carrot")
        {
            fruitIndex = 1;
            fruitLHS += "C";
        }
        else if (fruitChoice == "Tomato")
        {
            fruitIndex = 2;
            fruitLHS += "T";
        }

        currentPos.x -= 1.5f;
        nextPos.x -= 1.5f;

        Vector3 fruitPos = nextPos - (nextPos - currentPos) / 2;

        GameObject o = Instantiate(fruits[fruitIndex], fruitPos, fruits[fruitIndex].transform.rotation);
        fruitPositions.Add(o.transform);
    }

    void CheckPlayabilityWithFruits(GameObject next, int index, List<bool> visitedList, ref bool endFound)
    {
        if (next.tag == "Start") //Checks to see if the player can move from startPosition
            if (!next.GetComponentInChildren<RaycastPlayabilityStartPoint>().checkStartPossible())
                return;

        GameObject current = next;

        for (int i = 0; i < instantiatedObjects.Count; i++) //Gå FRAMÅT i loopen, för att kolla om man kommer åt endpos direkt
        {
            if (endFound)
                return;

            if (instantiatedObjects[i].tag != "Blade" && !visitedList[i] && current.GetComponentInChildren<RaycastPlayability>().isRayHittingPlatform(instantiatedObjects[i]))
            {
                current.GetComponentInChildren<RaycastPlayability>().isCheckingPlayability = true; //Draws the rays hit
                if (instantiatedObjects[i].GetComponentInParent<Transform>().gameObject.tag == "End")
                {
                    Debug.Log("Found End Platform");
                    endFound = true;
                    return;
                    if (instantiatedObjects[i].GetComponent<RaycastToEnd>().RetraceRaycast(current))
                    {
                        Debug.Log("Found End Box");
                        return;
                    }
                    continue;
                }
                visitedList[i] = true;
                CheckPlayabilityWithFruits(instantiatedObjects[i], i, visitedList, ref endFound);

                if (endFound)
                    return;

                //instantiatedObjects[i].GetComponentInChildren<RaycastPlayability>().isCheckingPlayability = false; //stops drawing
                visitedList[i] = false; //Reset position if previous path didn't find end
            }

        }

        if (endFound)
            return;

        int nextPlatform = index + 1;

        while (!activePlatforms[nextPlatform])
        {
            nextPlatform++;
        }

        while (saviour < 3)
        {
            Debug.Log("s" + lhs);
            Debug.Log("Current Platform: " + index + "   Next platform Pos:  " + nextPlatform + "   Tag: " + instantiatedObjects[nextPlatform].tag);
            //AddFruit(instantiatedObjects[nextPlatform].transform.position, instantiatedObjects[index].transform.position, ref visitedList, index);
            saviour++;
            for (int i = 1; i < visitedList.Count; i++)
            {
                visitedList[i] = false;
            }
            CheckPlayabilityWithFruits(instantiatedObjects[0], 0, visitedList, ref endFound);
            if (endFound)
                break;
        }

        if (endFound)
            return;
    }

    void AddSpikes(GameObject platform, int spikeCount, List<int> spikePositionsCreated, int lhsPos)
    {
        List<int> spikePositionsList = spikePositionsCreated;
        if (Random.Range(0, 11) < spikeChance && spikeCount != 12)
        {
            int spikePosition = Random.Range(2, 14);
            while (spikePositionsList.Contains(spikePosition))
                spikePosition = Random.Range(2, 14);

            GameObject o = Instantiate(spike, platform.GetComponent<RoomEndPoint>().getSpikePosition(spikePosition).position, platform.GetComponent<RoomEndPoint>().getSpikePosition(spikePosition).rotation);
            lhs = lhs.Insert(lhsPos + 1, "b");
            instantiatedObjects.Add(o);
            activePlatforms.Add(true);
            spikeCount++;
            lhsPos++;
            spikePositionsList.Add(spikePosition);
            AddSpikes(platform, spikeCount, spikePositionsList, lhsPos);
        }
    }

    Vector2 nextInstatiatePosition(int index)
    {
        if (index == 1)
        {
            return new Vector2(-platforms[0].GetComponent<RoomEndPoint>().GetObjectWidth() / 2, 0); // minus because index 1 instatiate position is start (left side of object)
        }
        else if (index == 2 || index == 5)
        {
            return new Vector2(platforms[0].GetComponent<RoomEndPoint>().GetObjectWidth() / 2, 0); // plus because index 2 instatiate position is end (right side of object)
        }
        else if (index == 3)
        {
            return new Vector2(0, platforms[1].GetComponent<RoomEndPoint>().GetObjectHeight() / 2); // plus because index 3 instatiate position is start (top side of object)
        }
        else if (index == 4)
        {
            return new Vector2(0, -platforms[1].GetComponent<RoomEndPoint>().GetObjectHeight() / 2); // minus because index 4 instatiate position is end (bottom side of object)
        }
        //else if (index == System.Convert.ToInt32('x') || index == 6)
        //{
        //    return new Vector2(rooms[3].GetComponent<RoomEndPoint>().GetObjectWidth() / 2, 0);
        //}


        return new Vector2(platforms[2].GetComponent<RoomEndPoint>().GetObjectWidth() / 2, 0); // if E
    }

    #region OuterWallInstantiations
    void InstantiateOuterWalls()
    {
        // The outer walls are one unit left, right, up and down from the board.
        leftEdgeX = transform.position.x - (platforms[0].GetComponentInChildren<SpriteRenderer>().bounds.size.x / 2) - (frames[0].GetComponent<SpriteRenderer>().bounds.size.x / 2);
        rightEdgeX = (width + platforms[0].GetComponentInChildren<SpriteRenderer>().bounds.size.x) - (frames[0].GetComponent<SpriteRenderer>().bounds.size.x);
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
    #endregion

}
