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

    private GameObject mainCamera;

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
    private List<Level> bestLevels = new List<Level>();
    private int populationSize, populationPos;

    private GameObject boardHolder;

    float leftEdgeX;
    float rightEdgeX;
    float bottomEdgeY;
    float topEdgeY;

    // Use this for initialization
    void Start()
    {
        mainCamera = GameObject.Find("Main Camera");
        evolutionManager = GetComponent<EvolutionManager>();
        character = GameObject.Find("Player");
        character.GetComponentInChildren<Camera>().enabled = false;

        boardHolder = new GameObject("BoardHolder");
        width = 30;
        height = 25;
        populationSize = 200;
        populationPos = 0;
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

        for (int i = 0; i < populationSize; i++)
        {
            GenerateLevel();
            //return;
        }

        Level bestLevel = evolutionManager.CreateBestLevel();
        BuildBestLevel(bestLevel);

        bestLevels.Add(bestLevel);
        for (int i = 1; i < 20; ++i)
        {
            bestLevels.Add(evolutionManager.GetLevel(i));
        }
    }

    void GenerateLevel()
    {
        foreach (GameObject o in instantiatedObjects)
        {
            Destroy(o);
        }
        instantiatedObjects.Clear();
        fitness = 0;
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
        if (Input.GetKeyDown(KeyCode.Return)) //Generate new level
        {
            ClearConsole();
            GenerateLevel();

            mainCamera.SetActive(true);

            //SceneManager.LoadScene("PCG_Level");
        }

        if (Input.GetKeyDown(KeyCode.Backspace)) //Jump into level with player
        {
            character.transform.position = startPosition;//Instantiate(character, startPosition, character.transform.rotation);
            //GameObject camera = GameObject.Find("Main Camera");
            character.GetComponentInChildren<Camera>().enabled = true;
            mainCamera.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.L))
            GiveFruitsToPlayer();


        if (Input.GetKeyDown(KeyCode.P)) //
        {
            ClearConsole();
            CalculateFitness();
            Debug.Log(lhs);
        }

        if (Input.GetKeyDown(KeyCode.N)) //
        {
            bestLevels.Clear();
            evolutionManager.ClearLevels();
            for (int i = 0; i < populationSize; i++)
            {
                GenerateLevel();
            }
            foreach (GameObject o in instantiatedObjects)
            {
                Destroy(o);
            }
            instantiatedObjects.Clear();

            populationPos = 0;
            Level bestLevel = evolutionManager.CreateBestLevel();
            BuildBestLevel(bestLevel);

            bestLevels.Add(bestLevel);
            for (int i = 1; i < 20; ++i)
            {
                bestLevels.Add(evolutionManager.GetLevel(i));
            }
        }

        if (Input.GetKeyDown(KeyCode.B)) //
        {
            populationPos++;
            if (populationPos >= 20)
                populationPos = 0;
            BuildBestLevel(bestLevels[populationPos]); //funkar inte.... transformlistorna i Level deletas efter start är färdigt
            Debug.Log("PopulationPos: " + populationPos);
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
                    AddSpikes(o, 0, new List<int>(), i);
            }
            if (lhs[i] == '2')
            {
                randomStartPosition = new Vector2(Random.Range(leftEdgeX, rightEdgeX - widthOffset), Random.Range(topEdgeY, bottomEdgeY + heightOffset)); //+ 1 for frame offset
                GameObject o = Instantiate(platforms[2], randomStartPosition + getObjectOffset(lhs[i]), platforms[2].transform.rotation);
                instantiatedObjects.Add(o);

                if (Random.Range(0, 11) < 5)
                    AddSpikes(o, 0, new List<int>(), i);
            }

            if (lhs[i] == 'b') //blade
                continue;
            #endregion

            #region Fruits
            if (lhs[i] == 'C')
            {
                randomStartPosition = new Vector2(Random.Range(leftEdgeX, rightEdgeX - widthOffset), Random.Range(topEdgeY, bottomEdgeY + heightOffset));
                GameObject o = Instantiate(fruits[0], randomStartPosition + getObjectOffset(lhs[i]), fruits[0].transform.rotation);

                //int saviour = 0;
                //while (Physics2D.OverlapAreaAll(o.GetComponent<Collider2D>().bounds.min, o.GetComponent<Collider2D>().bounds.max).Length > 2 || saviour < 10)
                //{
                //    Destroy(o);
                //    randomStartPosition = new Vector2(Random.Range(leftEdgeX, rightEdgeX - widthOffset), Random.Range(topEdgeY, bottomEdgeY + heightOffset));
                //    o = Instantiate(fruits[0], randomStartPosition + getObjectOffset(lhs[i]), fruits[0].transform.rotation);
                //    saviour++;
                //}
                instantiatedObjects.Add(o);

            }
            if (lhs[i] == 'T')
            {
                randomStartPosition = new Vector2(Random.Range(leftEdgeX, rightEdgeX - widthOffset), Random.Range(topEdgeY, bottomEdgeY + heightOffset));
                GameObject o = Instantiate(fruits[1], randomStartPosition + getObjectOffset(lhs[i]), fruits[1].transform.rotation);

                //int saviour = 0;
                //while (Physics2D.OverlapAreaAll(o.GetComponent<Collider2D>().bounds.min, o.GetComponent<Collider2D>().bounds.max).Length > 1 || saviour < 10)
                //{
                //    Destroy(o);
                //    randomStartPosition = new Vector2(Random.Range(leftEdgeX, rightEdgeX - widthOffset), Random.Range(topEdgeY, bottomEdgeY + heightOffset));
                //    o = Instantiate(fruits[1], randomStartPosition + getObjectOffset(lhs[i]), fruits[1].transform.rotation);
                //    saviour++;
                //}
                instantiatedObjects.Add(o);

            }
            if (lhs[i] == 'B')
            {
                randomStartPosition = new Vector2(Random.Range(leftEdgeX, rightEdgeX - widthOffset), Random.Range(topEdgeY, bottomEdgeY + heightOffset));
                GameObject o = Instantiate(fruits[2], randomStartPosition + getObjectOffset(lhs[i]), fruits[2].transform.rotation);

                //int saviour = 0;
                //while (Physics2D.OverlapAreaAll(o.GetComponent<Collider2D>().bounds.min, o.GetComponent<Collider2D>().bounds.max).Length > 4 || saviour < 100)
                //{
                //    Destroy(o);
                //    randomStartPosition = new Vector2(Random.Range(leftEdgeX, rightEdgeX - widthOffset), Random.Range(topEdgeY, bottomEdgeY + heightOffset));
                //    o = Instantiate(fruits[2], randomStartPosition + getObjectOffset(lhs[i]), fruits[2].transform.rotation);
                //    saviour++;
                //}
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

    void BuildBestLevel(Level bestLevel)
    {
        ClearConsole();

        foreach (GameObject o in instantiatedObjects)
        {
            Destroy(o);
        }
        instantiatedObjects.Clear();

        Debug.Log(bestLevel.LHS);

        for (int i = 0; i < bestLevel.LHS.Length; i++)
        {
            if (bestLevel.LHS[i] == 'S')
            {
                GameObject o = Instantiate(platforms[0], bestLevel.objectPositions[i], platforms[0].transform.rotation);
                instantiatedObjects.Add(o);
                startPosition = (bestLevel.objectPositions[i] - new Vector3(getObjectOffset(lhs[0]).x, 0, 0)) + new Vector3(1, 1, 0);
                startPlatformEndPoint = o.GetComponent<RoomEndPoint>().GetEndPosition();
            }
            #region Platforms
            if (bestLevel.LHS[i] == '1')
            {
                GameObject o = Instantiate(platforms[1], bestLevel.objectPositions[i], platforms[1].transform.rotation);
                instantiatedObjects.Add(o);

            }
            if (bestLevel.LHS[i] == '2')
            {
                GameObject o = Instantiate(platforms[2], bestLevel.objectPositions[i], platforms[2].transform.rotation);
                instantiatedObjects.Add(o);

            }
            if (bestLevel.LHS[i] == 'b') //blade
            {
                GameObject o = Instantiate(spike, bestLevel.objectPositions[i], bestLevel.objectRotations[i]);
                instantiatedObjects.Add(o);
            }
            #endregion

            #region Fruits
            if (bestLevel.LHS[i] == 'C')
            {
                GameObject o = Instantiate(fruits[0], bestLevel.objectPositions[i], fruits[0].transform.rotation);
                instantiatedObjects.Add(o);

            }
            if (bestLevel.LHS[i] == 'T')
            {
                GameObject o = Instantiate(fruits[1], bestLevel.objectPositions[i], fruits[1].transform.rotation);
                instantiatedObjects.Add(o);

            }
            if (bestLevel.LHS[i] == 'B')
            {
                GameObject o = Instantiate(fruits[2], bestLevel.objectPositions[i], fruits[2].transform.rotation);
                instantiatedObjects.Add(o);

            }
            #endregion

            if (bestLevel.LHS[i] == 'E')
            {
                GameObject o = Instantiate(platforms[3], bestLevel.objectPositions[i], platforms[3].transform.rotation);
                instantiatedObjects.Add(o);

                endPlatformPosition = o.GetComponentInChildren<Transform>().FindChild("End").position;
            }


        }
        lhs = bestLevel.LHS;
        CalculateFitness();
    }

    public float GetCandiateFitness(Level candidate)
    {
        ClearConsole();
        foreach (GameObject o in instantiatedObjects)
        {
            Destroy(o);
        }
        instantiatedObjects.Clear();

        Level individual = candidate;

        for (int i = 0; i < individual.LHS.Length; i++)
        {
            if (individual.LHS[i] == 'S')
            {
                GameObject o = Instantiate(platforms[0], individual.objectPositions[i], platforms[0].transform.rotation);
                instantiatedObjects.Add(o);
                startPosition = (individual.objectPositions[i] - new Vector3(getObjectOffset(lhs[0]).x, 0, 0)) + new Vector3(1, 1, 0);
                startPlatformEndPoint = o.GetComponent<RoomEndPoint>().GetEndPosition();
            }
            #region Platforms
            if (individual.LHS[i] == '1')
            {
                GameObject o = Instantiate(platforms[1], individual.objectPositions[i], platforms[1].transform.rotation);
                instantiatedObjects.Add(o);

            }
            if (individual.LHS[i] == '2')
            {
                GameObject o = Instantiate(platforms[2], individual.objectPositions[i], platforms[2].transform.rotation);
                instantiatedObjects.Add(o);

            }
            if (individual.LHS[i] == 'b') //blade
            {
                GameObject o = Instantiate(spike, individual.objectPositions[i], individual.objectRotations[i]);
                instantiatedObjects.Add(o);
            }
            #endregion

            #region Fruits
            if (individual.LHS[i] == 'C')
            {
                GameObject o = Instantiate(fruits[0], individual.objectPositions[i], fruits[0].transform.rotation);
                instantiatedObjects.Add(o);

            }
            if (individual.LHS[i] == 'T')
            {
                GameObject o = Instantiate(fruits[1], individual.objectPositions[i], fruits[1].transform.rotation);
                instantiatedObjects.Add(o);

            }
            if (individual.LHS[i] == 'B')
            {
                GameObject o = Instantiate(fruits[2], individual.objectPositions[i], fruits[2].transform.rotation);
                instantiatedObjects.Add(o);

            }
            #endregion

            if (individual.LHS[i] == 'E')
            {
                GameObject o = Instantiate(platforms[3], individual.objectPositions[i], platforms[3].transform.rotation);
                instantiatedObjects.Add(o);

                endPlatformPosition = o.GetComponentInChildren<Transform>().FindChild("End").position;
            }
        }

        lhs = individual.LHS;
        return CalculateFitness();
    }

    void AddSpikes(GameObject platform, int spikeCount, List<int> spikePositionsCreated, int lhsPos)
    {
        List<int> spikePositionsList = spikePositionsCreated;
        if (Random.Range(0, 11) < spikeChance && spikeCount != 14)
        {
            int spikePosition = Random.Range(0, 14);
            while (spikePositionsList.Contains(spikePosition))
                spikePosition = Random.Range(0, 14);

            GameObject o = Instantiate(spike, platform.GetComponent<RoomEndPoint>().getSpikePosition(spikePosition).position, platform.GetComponent<RoomEndPoint>().getSpikePosition(spikePosition).rotation);
            instantiatedObjects.Add(o);
            lhs = lhs.Insert(lhsPos + 1, "b");

            spikeCount++;
            lhsPos++;
            spikePositionsList.Add(spikePosition);
            AddSpikes(platform, spikeCount, spikePositionsList, lhsPos);
        }
    }

    /// <summary>
    /// Checks if it is possible to raycast to end from start
    /// </summary>
    /// <returns>1 if can't cast to end, 0.7 if possible</returns>
    float CanRaycastToEnd()
    {
        float rayLength = Vector2.Distance(startPlatformEndPoint, endPlatformPosition);
        Vector2 direction = endPlatformPosition - startPlatformEndPoint;
        RaycastHit2D ray = Physics2D.Raycast(startPlatformEndPoint, direction, rayLength);
        Debug.DrawRay(startPlatformEndPoint, direction, Color.cyan, 1);

        if (ray.collider.name == "End")
        {
            return 0.7f;
        }
        return 1f;
    }

    /// <summary>
    /// Checks the distance from start to end. Give more fitness if end further away
    /// </summary>
    /// <returns>1 if more than 6 in distance, 0.4 if close</returns>
    float CheckStartEndDistance()
    {
        float distance = Vector2.Distance(startPlatformEndPoint, endPlatformPosition);

        if (distance <= 6)
        {
            return 0.4f;
        }

        return 1f;
    }

    /// <summary>
    /// Checks height difference from start to end. If end above start and no tomatos or carrots, give lower fitness
    /// </summary>
    /// <returns>1 if can move up, 0.2 if not possible and level up high</returns>
    float CheckEndHeightPosition()
    {
        float distance = startPlatformEndPoint.y - (endPlatformPosition.y + 2); // +2 cause player can jump that height up

        if (lhs.Contains("B") && !lhs.Contains("C") && !lhs.Contains("T") && distance < 0)
        {
            return 0.2f;
        }
        return 1f;
    }

    /// <summary>
    /// Checks if it is possible to move from start to end with raycasting and without fruits. DFS through all instatiated objects
    /// </summary>
    /// <param name="next"> Next instatiated object to raycast from</param>
    /// <param name="visitedList">List checking which locations have been visited previously</param>
    /// <param name="endFound">Reference bool checking if end has been found</param>
    /// <returns>Return 1 if endFound, else 0</returns>
    bool CheckPlayabilityFindEndWithoutFruits(GameObject next, List<bool> visitedList, ref bool endFound)
    {
        if (next.tag == "Start") //Checks to see if the player can move from startPosition
            if (!next.GetComponentInChildren<RaycastPlayabilityStartPoint>().checkStartPossible())
                return true; //return true as if the map is shit :)

        GameObject current = next;

        for (int i = instantiatedObjects.Count - 1; i > 0; i--) //Gå bakåt i loopen, för att kolla om man kommer åt endpos direkt
        {
            if (endFound)
                return true;

            if (instantiatedObjects[i].tag != "Blade" && instantiatedObjects[i].tag != "Fruit" && !visitedList[i] && current.GetComponentInChildren<RaycastPlayability>().isRayHittingPlatform(instantiatedObjects[i]))
            {
                current.GetComponentInChildren<RaycastPlayability>().isCheckingPlayability = true; //Draws the rays hit
                if (instantiatedObjects[i].GetComponentInParent<Transform>().gameObject.tag == "End")
                {
                    if (instantiatedObjects[i].GetComponent<RaycastToEnd>().RetraceRaycast(current))
                    {
                        Debug.Log("Found End Box without vegetables");
                        endFound = true;
                        return true;

                    }
                    continue;
                }
                visitedList[i] = true;
                CheckPlayabilityFindEndWithoutFruits(instantiatedObjects[i], visitedList, ref endFound);

                if (endFound)
                    return true;

                instantiatedObjects[i].GetComponentInChildren<RaycastPlayability>().isCheckingPlayability = false; //stops drawing
                visitedList[i] = false; //Reset position if previous path didn't find end
            }
        }

        if (endFound)
            return true;
        return false;
    }

    /// <summary>
    /// Checks if it is possible to move from start to end with raycasting. DFS through all instatiated objects
    /// </summary>
    /// <param name="next"> Next instatiated object to raycast from</param>
    /// <param name="visitedList">List checking which locations have been visited previously</param>
    /// <param name="endFound">Reference bool checking if end has been found</param>
    /// <returns>Return 1 if endFound, else 0</returns>
    float CheckPlayabilityWithFruits(GameObject next, List<bool> visitedList, ref bool endFound)
    {
        if (next.tag == "Start") //Checks to see if the player can move from startPosition
            if (!next.GetComponentInChildren<RaycastPlayabilityStartPoint>().checkStartPossible())
                return 0;

        GameObject current = next;

        for (int i = instantiatedObjects.Count - 1; i > 0; i--) //Gå bakåt i loopen, för att kolla om man kommer åt endpos direkt
        {
            if (endFound)
                return 1;

            if (instantiatedObjects[i].tag != "Blade" && !visitedList[i] && current.GetComponentInChildren<RaycastPlayability>().isRayHittingPlatform(instantiatedObjects[i]))
            {
                current.GetComponentInChildren<RaycastPlayability>().isCheckingPlayability = true; //Draws the rays hit
                if (instantiatedObjects[i].GetComponentInParent<Transform>().gameObject.tag == "End")
                {
                    Debug.Log("Found End Platform");
                    if (instantiatedObjects[i].GetComponent<RaycastToEnd>().RetraceRaycast(current))
                    {
                        Debug.Log("Found End Box");
                        endFound = true;
                        return 1;

                    }
                    continue;
                }
                visitedList[i] = true;
                CheckPlayabilityWithFruits(instantiatedObjects[i], visitedList, ref endFound);

                if (endFound)
                    return 1;

                instantiatedObjects[i].GetComponentInChildren<RaycastPlayability>().isCheckingPlayability = false; //stops drawing
                visitedList[i] = false; //Reset position if previous path didn't find end
            }
        }
        if (endFound)
            return 1;
        return 0f;
    }

    /// <summary>
    /// Checks how many platforms needed to visit before end found
    /// </summary>
    /// <param name="visitedList">List that checks how many platforms visited in DFS</param>
    /// <returns>0.2f * platformsVisited</returns>
    float CheckPlatformsNeeded(List<bool> visitedList)
    {
        int platformsVisited = 0;
        for (int i = 0; i < visitedList.Count; i++)
        {
            if (visitedList[i])
                platformsVisited++;
        }

        float returnValue = (float)0.2 * platformsVisited;
        Debug.Log("CheckPlatformsNeeded fitness: " + (20 * returnValue) + "    Platforms: " + platformsVisited);

        return returnValue;
    }

    /// <summary>
    /// Checks how many vegetables used. The more vegetables needed, the more fitness. 
    /// More fitness if tomatos or bananas used.
    /// Vegetables not used are removed
    /// </summary>
    /// <param name="visitedList"></param>
    /// <returns>+1 * vegetables used</returns>
    float CheckVegetablesUsed(List<bool> visitedList)
    {
        int vegetablesUsed = 0;
        int notCarrotCount = 0; //+ more if tomatos and bananas

        for (int i = 0; i < instantiatedObjects.Count; i++)
        {
            if (instantiatedObjects[i].tag == "Fruit")
            {
                if (visitedList[i])
                {
                    vegetablesUsed++;
                    if (lhs[i] == 'T' || lhs[i] == 'B')
                        notCarrotCount++;
                    continue;
                }

                Destroy(instantiatedObjects[i]); //Removes unused vegetables
                instantiatedObjects.RemoveAt(i);
                visitedList.RemoveAt(i);
                lhs = lhs.Remove(i, 1);
                i--;
            }
        }

        float returnValue = 1 * vegetablesUsed + notCarrotCount * 0.2f;
        Debug.Log("CheckVegetablesUsed fitness: " + (100 * returnValue) + "      Vegetables used: " + vegetablesUsed);

        return returnValue;
    }

    /// <summary>
    /// Checks is vegetables are colliding with platform or other vegetables
    /// </summary>
    /// <returns>returns 1 if no vegetable is colliding with anything, else 0</returns>
    float CheckVegetablesColliding()
    {
        int vegetablesCount = 0;
        int vegetablesNotIntersected = 0;
        for (int i = 0; i < instantiatedObjects.Count; ++i)
        {
            if (lhs[i] == 'C')
            {
                vegetablesCount++;
                if (Physics2D.OverlapAreaAll(instantiatedObjects[i].GetComponent<Collider2D>().bounds.min, instantiatedObjects[i].GetComponent<Collider2D>().bounds.max).Length <= 2)
                    vegetablesNotIntersected++;
            }
            else if (lhs[i] == 'T')
            {
                vegetablesCount++;
                if (Physics2D.OverlapAreaAll(instantiatedObjects[i].GetComponent<Collider2D>().bounds.min, instantiatedObjects[i].GetComponent<Collider2D>().bounds.max).Length <= 1)
                    vegetablesNotIntersected++;
            }
            else if (lhs[i] == 'B')
            {
                vegetablesCount++;
                if (Physics2D.OverlapAreaAll(instantiatedObjects[i].GetComponent<Collider2D>().bounds.min, instantiatedObjects[i].GetComponent<Collider2D>().bounds.max).Length <= 4)
                    vegetablesNotIntersected++;
            }
        }

        if (vegetablesCount == 0)
        {
            Debug.Log("CheckVegetablesColliding fitness: " + 0);
            return 0;
        }

        if (vegetablesCount - vegetablesNotIntersected == 0)
        {
            Debug.Log("CheckVegetablesColliding fitness: " + (50 * 1));
            return 1f;
        }
        Debug.Log("CheckVegetablesColliding fitness: " + 0);
        return 0;
    }

    /// <summary>
    /// Checks how many blades used, add more fitness the more blades used
    /// </summary>
    /// <returns>+0.1 fitness/blade</returns>
    float CheckAmountofBlades()
    {
        int bladeCount = 0;
        for (int i = 0; i < instantiatedObjects.Count; i++)
        {
            if (instantiatedObjects[i].tag == "Blade")
            {
                bladeCount++;
            }
        }

        float returnValue = (float)(0 + (0.1 * bladeCount));
        Debug.Log("CheckAmountofBlades fitness: " + (10 * returnValue) + "    Blades: " + bladeCount);

        return returnValue;
    }

    float CalculateFitness()
    {
        bool endFound = false;
        List<bool> visited = new List<bool>();
        for (int i = 0; i < instantiatedObjects.Count; i++)
        {
            visited.Add(false);
            if (instantiatedObjects[i].tag == "Fruit") //removes fruits temporarily for check to see if level is playable without fruits
                instantiatedObjects[i].SetActive(false);
        }
        visited[0] = true;


        if (CheckPlayabilityFindEndWithoutFruits(instantiatedObjects[0], visited, ref endFound))
        {
            Debug.Log("Total fitness: " + 60);
            return 60; //return ok value, incase level would want to be modified manually.
        }

        for (int i = 1; i < visited.Count; i++) //reset list
        {
            visited[i] = false;
            if (instantiatedObjects[i].tag == "Fruit") //Re-add fruits
                instantiatedObjects[i].SetActive(true);
        }
        endFound = false;


        float fitness = 25 * CheckEndHeightPosition() + 10 * CheckStartEndDistance() + 10 * CanRaycastToEnd() +
         100 * CheckPlayabilityWithFruits(instantiatedObjects[0], visited, ref endFound) + 20 * CheckPlatformsNeeded(visited) + 100 * CheckVegetablesUsed(visited) + 200 * CheckVegetablesColliding();
        //+ 10 * CheckAmountofBlades();

        Debug.Log("Total fitness: " + fitness);
        return fitness;
    }

    void GiveFruitsToPlayer()
    {
        int carrots, tomatos, bananas;
        carrots = tomatos = bananas = 0;

        for (int i = 0; i < lhs.Length; i++)
        {
            if (lhs[i] == 'C')
            {
                Destroy(instantiatedObjects[i]);
                carrots++;
            }
            if (lhs[i] == 'T')
            {
                Destroy(instantiatedObjects[i]);
                tomatos++;
            }
            if (lhs[i] == 'B')
            {
                Destroy(instantiatedObjects[i]);
                bananas++;
            }
        }

        character.GetComponent<PlayerController>().GetVegetables(carrots, tomatos, bananas);
        GameObject.Find("LevelData").GetComponent<LevelData>().SetData(carrots, tomatos, bananas);
    }

    public Vector2 getObjectOffset(char index)
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

    public Vector3 getRandomInstatiatePosition(char type)
    {
        if (type == '2')
            return new Vector2(Random.Range(leftEdgeX, rightEdgeX - widthOffset), Random.Range(topEdgeY, bottomEdgeY + heightOffset)); //+ 1 for frame offset

        return new Vector2(Random.Range(leftEdgeX, rightEdgeX - widthOffset), Random.Range(topEdgeY - heightOffset, bottomEdgeY + 1));
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
