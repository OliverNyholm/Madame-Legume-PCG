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
    string filePath;
    public System.IO.StreamReader textfile;

    string data;
    string lhs;
    List<Vector3> objectPositions;
    List<Quaternion> objectRotations;

    // Use this for initialization
    void Start()
    {
        objectPositions = new List<Vector3>();
        objectRotations = new List<Quaternion>();
        textfile = new System.IO.StreamReader("C:/Users/Datorlabbet/Documents/Madame-Legume-PCG/Assets/Levels/LevelTxt/" + filePath);
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

        BuildBestLevel();
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
        
    }
    void BuildBestLevel()
    {
        for (int i = 0; i < lhs.Length; i++)
        {
            if (lhs[i] == 'S')
            {
                GameObject o = Instantiate(platforms[0], objectPositions[i], platforms[0].transform.rotation);
                //startPosition = (objectPositions[i] - new Vector3(getObjectOffset(lhs[0]).x, 0, 0)) + new Vector3(1, 1, 0);
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
                GameObject o = Instantiate(fruits[0], objectPositions[i], fruits[0].transform.rotation);
            }
            if (lhs[i] == 'T')
            {
                GameObject o = Instantiate(fruits[1], objectPositions[i], fruits[1].transform.rotation);

            }
            if (lhs[i] == 'B')
            {
                GameObject o = Instantiate(fruits[2], objectPositions[i], fruits[2].transform.rotation);

            }
            #endregion

            if (lhs[i] == 'E')
            {
                GameObject o = Instantiate(platforms[3], objectPositions[i], platforms[3].transform.rotation);
            }


        }
    }
}
