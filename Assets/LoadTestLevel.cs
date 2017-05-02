using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadTestLevel : MonoBehaviour
{

    public string levelToLoad;
    public List<GameObject> platforms, fruits;
    [SerializeField]
    private GameObject[] frames;
    public GameObject blade;

    string lhs;
    string fruitLHS;
    List<Transform> instantiateTransform;
    List<bool> activePlatforms;
    private GameObject start;

    private GameObject boardHolder;
    float leftEdgeX;
    float rightEdgeX;
    float bottomEdgeY;
    float topEdgeY;
    private int width, height;

    bool isFruitSetUp;

    private int bananaCount, tomatoCount, carrotCount;

    // Use this for initialization
    void Awake()
    {
        InstantiateOuterWalls();
        LoadLevel();
        BuildLevel();
    }

    void LoadLevel()
    {
        instantiateTransform = ES2.LoadList<Transform>("instantiatedObjects" + levelToLoad);
        lhs = ES2.Load<string>("LHS" + levelToLoad);
        fruitLHS = ES2.Load<string>("fruitLHS" + levelToLoad);
        activePlatforms = ES2.LoadList<bool>("activePlatforms" + levelToLoad);
        Debug.Log(lhs);
    }

    void BuildLevel()
    {
        start = Instantiate(platforms[0], new Vector2(0, 0), Quaternion.identity);
        start.GetComponentInChildren<SpriteRenderer>().color = Color.green;

        for (int i = 0; i < lhs.Length; i++)
        {
            if (lhs[i] == '1' && activePlatforms[i + 1])
            {
                GameObject o = Instantiate(platforms[0], instantiateTransform[i + 1].position, platforms[0].transform.rotation);
            }
            if (lhs[i] == '2' && activePlatforms[i + 1])
            {
                GameObject o = Instantiate(platforms[0], instantiateTransform[i + 1].position, platforms[0].transform.rotation);
            }
            if (lhs[i] == '3' && activePlatforms[i + 1])
            {
                GameObject o = Instantiate(platforms[1], instantiateTransform[i + 1].position, platforms[1].transform.rotation);
            }
            if (lhs[i] == '4' && activePlatforms[i + 1])
            {
                GameObject o = Instantiate(platforms[1], instantiateTransform[i + 1].position, platforms[1].transform.rotation);
            }
            if (lhs[i] == 'E' && activePlatforms[i + 1])
            {
                GameObject o = Instantiate(platforms[2], instantiateTransform[i + 1].position, platforms[2].transform.rotation);
            }
            if (lhs[i] == 'b' && activePlatforms[i + 1])
            {
                GameObject o = Instantiate(blade, instantiateTransform[i + 1].position, instantiateTransform[i + 1].rotation);
            }            
        }

        for (int i = 0; i < fruitLHS.Length; i++)
        {
            if (fruitLHS[i] == 'T')
            {
                tomatoCount++;
            }
            if (fruitLHS[i] == 'B')
            {
                bananaCount++;
            }
            if (fruitLHS[i] == 'C')
            {
                carrotCount++;
            }
        }
    }

    void InstantiatePlayer()
    {
        GameObject.Find("LevelData").GetComponent<LevelData>().SetData(carrotCount, tomatoCount, bananaCount);
        GameObject.Find("Player").GetComponent<PlayerController>().GetVegetables(carrotCount, tomatoCount, bananaCount);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isFruitSetUp)
        {
            InstantiatePlayer();
            isFruitSetUp = true;
        }
    }

    #region OuterWallInstantiations
    void InstantiateOuterWalls()
    {
        boardHolder = new GameObject("BoardHolder");
        width = 30;
        height = 25;

        // The outer walls are one unit left, right, up and down from the board.
        leftEdgeX = transform.position.x - (platforms[0].GetComponentInChildren<SpriteRenderer>().bounds.size.x / 2) - (frames[0].GetComponent<SpriteRenderer>().bounds.size.x / 2);
        rightEdgeX = (width + platforms[0].GetComponentInChildren<SpriteRenderer>().bounds.size.x) - (frames[0].GetComponent<SpriteRenderer>().bounds.size.x);
        bottomEdgeY = transform.position.y - height / 2;
        topEdgeY = height / 2;

        // Instantiate both vertical walls (one on each side).
        InstantiateVerticalOuterWall(leftEdgeX, bottomEdgeY, topEdgeY);
        InstantiateVerticalOuterWall(rightEdgeX, bottomEdgeY, topEdgeY);

        // Instantiate both horizontal walls, these are one in left and right from the outer walls.
        InstantiateHorizontalOuterWall(leftEdgeX + 1f, rightEdgeX - 1f, bottomEdgeY, true);
        InstantiateHorizontalOuterWall(leftEdgeX + 1f, rightEdgeX - 1f, topEdgeY, false);
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

    void InstantiateHorizontalOuterWall(float startingX, float endingX, float yCoord, bool blades)
    {
        // Start the loop at the starting value for X.
        float currentX = startingX;

        // While the value for X is less than the end value...
        while (currentX <= endingX)
        {
            // ... instantiate an outer wall tile at the y coordinate and the current x coordinate.
            InstantiateFromArray(frames, currentX, yCoord);
            if (blades)
            {
                GameObject bladeInstance = Instantiate(blade, new Vector3(currentX, yCoord + 1, 0), Quaternion.Euler(0, 0, -90), boardHolder.transform) as GameObject;
            }
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
