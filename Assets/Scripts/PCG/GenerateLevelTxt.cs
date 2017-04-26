using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateLevelTxt : MonoBehaviour
{

    [SerializeField]
    private GameObject[] platforms;

    [SerializeField]
    private GameObject[] fruits;

    [SerializeField]
    private GameObject spike;

    [SerializeField]
    private GameObject character;

    [SerializeField]
    private GameObject[] frames;

    private GameObject boardHolder;

    [SerializeField]
    string filePath;
    public System.IO.StreamReader textfile;

    string data;
    string lhs;
    List<Vector3> objectPositions;
    List<Quaternion> objectRotations;

    Vector3 startPosition;
    int carrots, bananas, tomatos;
    bool isVegetablesSetUp;

    float leftEdgeX;
    float rightEdgeX;
    float bottomEdgeY;
    float topEdgeY;
    private int width, height;

    // Use this for initialization
    void Start()
    {
        boardHolder = new GameObject("BoardHolder");
        width = 30;
        height = 25;
        objectPositions = new List<Vector3>();
        objectRotations = new List<Quaternion>();
        textfile = new System.IO.StreamReader(Application.dataPath + "/Levels/LevelTxt/" + filePath + ".txt");
        data = textfile.ReadToEnd();

        int index = 0;
        while (data[index] != ',')
            index++;
        lhs = data.Substring(0, index);


        while (data[index + 3] != '|') //+ 3 to skip \r \n
        {

            while (data[index] != '(')
                index++;

            int startOfVector = index;
            int endOfVector = 0;
            while (data[index] != ')')
            {
                endOfVector++;
                index++;
            }
            endOfVector++;

            string subVectorString = data.Substring(startOfVector, endOfVector);
            objectPositions.Add(StringToVector3(subVectorString));
        }

        while (index + 3 != data.Length) //+ 3 to skip \r \n
        {
            while (data[index] != '(')
                index++;

            int startOfVector = index;
            int endOfVector = 0;
            while (data[index] != ')')
            {
                endOfVector++;
                index++;
            }
            endOfVector++;

            string subVectorString = data.Substring(startOfVector, endOfVector);
            objectRotations.Add(StringToQuaternion(subVectorString));
        }

        InstantiateOuterWalls();
        BuildBestLevel();

        character.transform.position = startPosition;
        Camera camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        camera.enabled = false;
    }

    public static Vector3 StringToVector3(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        // split the items
        string[] sArray = sVector.Split(',');

        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]));

        return result;
    }

    public static Quaternion StringToQuaternion(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        // split the items
        string[] sArray = sVector.Split(',');

        // store as a Vector3
        Quaternion result = new Quaternion(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]),
            float.Parse(sArray[3]));

        return result;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isVegetablesSetUp) //gives vegetables to player
        {
            GameObject.Find("LevelData").GetComponent<LevelData>().SetData(carrots, tomatos, bananas);
            GameObject.Find("Player").GetComponentInChildren<PlayerController>().GetVegetables(carrots, tomatos, bananas);

            isVegetablesSetUp = true;
        }
    }
    void BuildBestLevel()
    {
        for (int i = 0; i < lhs.Length; i++)
        {
            if (lhs[i] == 'S')
            {
                GameObject o = Instantiate(platforms[0], objectPositions[i], platforms[0].transform.rotation);
                startPosition = (objectPositions[i] - new Vector3(platforms[0].GetComponent<RoomEndPoint>().GetObjectWidth() / 2, 0) + new Vector3(1, 1, 0));
            }
            #region Platforms
            if (lhs[i] == '1')
            {
                GameObject o = Instantiate(platforms[1], objectPositions[i], platforms[1].transform.rotation);

            }
            if (lhs[i] == '2')
            {
                GameObject o = Instantiate(platforms[2], objectPositions[i], platforms[2].transform.rotation);

            }
            if (lhs[i] == 'b') //blade
            {
                GameObject o = Instantiate(spike, objectPositions[i], objectRotations[i]);
            }
            #endregion

            #region Fruits
            if (lhs[i] == 'C')
            {
                carrots++;
            }
            if (lhs[i] == 'T')
            {
                tomatos++;

            }
            if (lhs[i] == 'B')
            {
                bananas++;
            }
            #endregion

            if (lhs[i] == 'E')
            {
                GameObject o = Instantiate(platforms[3], objectPositions[i], platforms[3].transform.rotation);
            }


        }
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
                GameObject bladeInstance = Instantiate(spike, new Vector3(currentX, yCoord + 1, 0), Quaternion.Euler(0, 0, -90), boardHolder.transform) as GameObject;
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
}
