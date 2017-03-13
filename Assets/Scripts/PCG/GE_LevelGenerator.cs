using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class GE_LevelGenerator : MonoBehaviour
{

    enum TileType
    {
        wall, empty
    }

    [SerializeField]
    private GameObject[] platforms;

    [SerializeField]
    private GameObject[] fruits;

    [SerializeField]
    private GameObject[] frames;

    [SerializeField]
    private GameObject spike;

    [SerializeField]
    private int spikeChance;

    [SerializeField]
    private GameObject character;

    private TileType[][] tiles;

    private RoomEndPoint roomEndPoint;
    private EvolutionManager evolutionManager;

    private GameObject start;
    private string lhs;
    private string[] oRHS;
    private string[] fRHS;
    private float fitness;

    private int width, height;
    private float widthOffset, heightOffset;
    private Random random = new Random();

    private Vector2 startPosition;
    private Vector2 startPlatformEndPoint, endPlatformPosition;

    private List<GameObject> instantiatedObjects = new List<GameObject>();

    private GameObject boardHolder;

    float leftEdgeX;
    float rightEdgeX;
    float bottomEdgeY;
    float topEdgeY;

    // Use this for initialization
    void Start()
    {
        evolutionManager = GetComponent<EvolutionManager>();
        character = GameObject.Find("Player");

        boardHolder = new GameObject("BoardHolder");
        width = 30;
        height = 25;
        widthOffset = platforms[1].GetComponent<RoomEndPoint>().GetObjectWidth();
        heightOffset = platforms[2].GetComponent<RoomEndPoint>().GetObjectHeight();
        InstantiateOuterWalls();

        oRHS = new string[3];
        lhs = "SOFE";
        oRHS[0] = "1";
        oRHS[1] = "2";
        oRHS[2] = "p";

        fRHS = new string[4];
        fRHS[0] = "C";
        fRHS[1] = "T";
        fRHS[2] = "B";
        fRHS[3] = "f";

        GenerateLevel();
    }

    void GenerateLevel()
    {
        foreach (GameObject o in instantiatedObjects)
        {
            Destroy(o);
        }
        instantiatedObjects.Clear();
        lhs = "SOFE";
        ReadLHS();
        BuildLevel();
        fitness = CalculateFitness();
        evolutionManager.InsertLevelData(lhs, instantiatedObjects, fitness);
        Debug.Log(lhs);
    }

    public Vector2 GetStartPosition()
    {
        return startPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ClearConsole();
            GenerateLevel();
            //SceneManager.LoadScene("PCG_Level");
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            character.transform.position = startPosition;//Instantiate(character, startPosition, character.transform.rotation);
            GameObject camera = GameObject.Find("Main Camera");
            camera.SetActive(false);
        }
    }

    static void ClearConsole()
    {
        // This simply does "LogEntries.Clear()" the long way:
        var logEntries = System.Type.GetType("UnityEditorInternal.LogEntries,UnityEditor.dll");
        var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        clearMethod.Invoke(null, null);
    }

    void ReadLHS()
    {
        int i = 0;
        while (i < lhs.Length)
        {
            if (lhs[i] != 'O' && lhs[i] != 'p' && lhs[i] != 'F' && lhs[i] != 'f' && lhs[i] != 'E')
            {
                ++i;
                continue;
            }

            if (lhs[i] == 'O')
            {
                int platform = Random.Range(0, oRHS.Length - 1);
                lhs = lhs.Remove(i, 1);
                lhs = lhs.Insert(i, oRHS[platform] + 'p');
            }
            if (lhs[i] == 'p')
            {
                int platform = Random.Range(0, oRHS.Length);
                if (platform == oRHS.Length - 1)
                {
                    int extraPlatform = Random.Range(0, oRHS.Length - 1);
                    lhs = lhs.Remove(i, 1);
                    lhs = lhs.Insert(i, oRHS[extraPlatform] + 'p');
                }
                else
                {
                    lhs = lhs.Remove(i, 1);
                    lhs = lhs.Insert(i, oRHS[platform]);
                }
            }

            if (lhs[i] == 'F')
            {
                int fruit = Random.Range(0, fRHS.Length - 1);
                lhs = lhs.Remove(i, 1);
                lhs = lhs.Insert(i, fRHS[fruit] + 'f');
            }
            if (lhs[i] == 'f')
            {
                int fruit = Random.Range(0, fRHS.Length);
                if (fruit == fRHS.Length - 1)
                {
                    int extrafruit = Random.Range(0, fRHS.Length - 1);
                    lhs = lhs.Remove(i, 1);
                    lhs = lhs.Insert(i, fRHS[extrafruit] + 'f');
                }
                else
                {
                    lhs = lhs.Remove(i, 1);
                    lhs = lhs.Insert(i, fRHS[fruit]);
                }
            }

            ++i;
        }
    }

    void BuildLevel()
    {
        for (int i = 0; i < lhs.Length; i++)
        {
            Vector2 randomStartPosition;

            if (lhs[i] == 'S')
            {
                randomStartPosition = new Vector2(leftEdgeX, Random.Range(topEdgeY - heightOffset, bottomEdgeY + 1)); //+ 1 for frame offset
                GameObject o = Instantiate(platforms[0], randomStartPosition + getObjectOffset(lhs[i]), platforms[0].transform.rotation);
                instantiatedObjects.Add(o);
                startPosition = randomStartPosition + new Vector2(1, 1);
                startPlatformEndPoint = o.GetComponent<RoomEndPoint>().GetEndPosition();
            }
            #region Platforms
            if (lhs[i] == '1')
            {
                randomStartPosition = new Vector2(Random.Range(leftEdgeX, rightEdgeX - widthOffset), Random.Range(topEdgeY - heightOffset, bottomEdgeY + 1)); //+ 1 for frame offset
                GameObject o = Instantiate(platforms[1], randomStartPosition + getObjectOffset(lhs[i]), platforms[1].transform.rotation);
                instantiatedObjects.Add(o);

                if (Random.Range(0, 11) < 5)
                    AddSpikes(o, 0, new List<int>());
            }
            if (lhs[i] == '2')
            {
                randomStartPosition = new Vector2(Random.Range(leftEdgeX, rightEdgeX - widthOffset), Random.Range(topEdgeY, bottomEdgeY + heightOffset)); //+ 1 for frame offset
                GameObject o = Instantiate(platforms[2], randomStartPosition + getObjectOffset(lhs[i]), platforms[2].transform.rotation);
                instantiatedObjects.Add(o);

                if (Random.Range(0, 11) < 5)
                    AddSpikes(o, 0, new List<int>());
            }
            #endregion

            #region Fruits
            if (lhs[i] == 'C')
            {
                randomStartPosition = new Vector2(Random.Range(leftEdgeX, rightEdgeX - widthOffset), Random.Range(topEdgeY, bottomEdgeY + heightOffset));
                GameObject o = Instantiate(fruits[0], randomStartPosition + getObjectOffset(lhs[i]), fruits[0].transform.rotation);
                instantiatedObjects.Add(o);

            }
            if (lhs[i] == 'T')
            {
                randomStartPosition = new Vector2(Random.Range(leftEdgeX, rightEdgeX - widthOffset), Random.Range(topEdgeY, bottomEdgeY + heightOffset));
                GameObject o = Instantiate(fruits[1], randomStartPosition + getObjectOffset(lhs[i]), fruits[1].transform.rotation);
                instantiatedObjects.Add(o);

            }
            if (lhs[i] == 'B')
            {
                randomStartPosition = new Vector2(Random.Range(leftEdgeX, rightEdgeX - widthOffset), Random.Range(topEdgeY, bottomEdgeY + heightOffset));
                GameObject o = Instantiate(fruits[2], randomStartPosition + getObjectOffset(lhs[i]), fruits[2].transform.rotation);
                instantiatedObjects.Add(o);

            }
            #endregion

            if (lhs[i] == 'E')
            {
                randomStartPosition = new Vector2(Random.Range(leftEdgeX, rightEdgeX - widthOffset), Random.Range(topEdgeY - heightOffset, bottomEdgeY + 1)); //+ 1 for frame offset
                GameObject o = Instantiate(platforms[3], randomStartPosition + getObjectOffset(lhs[i]), platforms[3].transform.rotation);
                instantiatedObjects.Add(o);

                endPlatformPosition = o.GetComponentInChildren<Transform>().FindChild("End").position;
            }
        }
    }

    void AddSpikes(GameObject platform, int spikeCount, List<int> spikePositionsCreated)
    {
        List<int> spikePositionsList = spikePositionsCreated;
        if (Random.Range(0, 11) < spikeChance && spikeCount != 14)
        {
            int spikePosition = Random.Range(0, 14);
            while (spikePositionsList.Contains(spikePosition))
                spikePosition = Random.Range(0, 14);

            GameObject o = Instantiate(spike, platform.GetComponent<RoomEndPoint>().getSpikePosition(spikePosition).position, platform.GetComponent<RoomEndPoint>().getSpikePosition(spikePosition).rotation);
            instantiatedObjects.Add(o);

            spikeCount++;
            spikePositionsList.Add(spikePosition);
            AddSpikes(platform, spikeCount, spikePositionsList);
        }
    }

    float CanRaycastToEnd()
    {
        float rayLength = Vector2.Distance(startPlatformEndPoint, endPlatformPosition);
        Vector2 direction = endPlatformPosition - startPlatformEndPoint;
        RaycastHit2D ray = Physics2D.Raycast(startPlatformEndPoint, direction, rayLength);
        Debug.DrawRay(startPlatformEndPoint, direction, Color.red, 1);

        if (ray.collider.name == "End")
        {
            return 0.7f;
        }
        return 1f;
    }

    float CheckStartEndDistance()
    {
        float distance = Vector2.Distance(startPlatformEndPoint, endPlatformPosition);

        if (distance <= 6)
        {
            return 0.4f;
        }

        return 1f;
    }

    float CheckEndHeightPosition()
    {
        float distance = startPlatformEndPoint.y - (endPlatformPosition.y + 2);

        if (lhs.Contains("B") && !lhs.Contains("C") && !lhs.Contains("T") && distance < 0)
        {
            return 0.2f;
        }
        return 1f;
    }

    float CheckPlayability(GameObject next, List<bool> visitedList, ref bool endFound)
    {
        GameObject current = next;

        for (int i = 0; i < instantiatedObjects.Count; i++)
        {
            if (endFound)
                break;


            if (current != instantiatedObjects[i] && current.name == "Start(Clone)" && current.GetComponentInChildren<RaycastPlayability>().isRayHittingPlatform(instantiatedObjects[i]))
            {
                if (instantiatedObjects[i].GetComponentInParent<Transform>().gameObject.name == "RoomFinish")
                {
                    endFound = true;
                    return 1;
                }
                current = instantiatedObjects[i];
                visitedList[i] = true;
                CheckPlayability(current, visitedList, ref endFound);
            }

            //if (current.bounds.Intersects(instantiatedObjects[i].GetComponentInChildren<BoxCollider2D>().bounds) && current != instantiatedObjects[i] && !visitedList[i])
            //{
            //    if (instantiatedObjects[i].GetComponentInParent<Transform>().gameObject.name == "RoomFinish")
            //    {
            //        endFound = true;
            //        return 1;
            //    }
            //    current = instantiatedObjects[i].GetComponentInChildren<PolygonCollider2D>();
            //    visitedList[i] = true;
            //    CheckPlayability(current, visitedList, ref endFound);
            //}
        }
        if (endFound)
            return 1;
        return 0f;
    }

    float CalculateFitness()
    {
        bool endFound = false;
        List<bool> visited = new List<bool>();
        for (int i = 0; i < instantiatedObjects.Count; i++)
        {
            visited.Add(false);
        }
        visited[0] = true;
        float fitness = 25 * CheckEndHeightPosition() + 10 * CheckStartEndDistance() + 10 * CanRaycastToEnd() + 100 * CheckPlayability(instantiatedObjects[0], visited, ref endFound);

        Debug.Log(fitness);
        return fitness;
    }

    Vector2 getObjectOffset(char index)
    {
        if (index == 'S' || index == '1' || index == 'E')
        {
            return new Vector2(platforms[0].GetComponent<RoomEndPoint>().GetObjectWidth() / 2, 0);
        }
        else if (index == '2')
        {
            return new Vector2(0, -platforms[2].GetComponent<RoomEndPoint>().GetObjectHeight() / 2);
        }

        else if (index == 'C')
        {
            return new Vector2(fruits[0].GetComponent<SpriteRenderer>().bounds.size.x / 2, -fruits[0].GetComponent<SpriteRenderer>().bounds.size.y / 2);
        }
        else if (index == 'T')
        {
            return new Vector2(fruits[1].GetComponent<SpriteRenderer>().bounds.size.x / 2, -fruits[1].GetComponent<SpriteRenderer>().bounds.size.y / 2);
        }
        else if (index == 'B')
        {
            return new Vector2(fruits[0].GetComponent<SpriteRenderer>().bounds.size.x / 2, -fruits[0].GetComponent<SpriteRenderer>().bounds.size.y / 2);
        }

        return new Vector2(0, 0);
    }

    void CheckProjection(GameObject o)
    {
        PolygonCollider2D temp = o.GetComponent<PolygonCollider2D>();
        //temp.points.
    }

    void InstantiateOuterWalls()
    {
        // The outer walls are one unit left, right, up and down from the board.
        leftEdgeX = transform.position.x;
        rightEdgeX = width;
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
