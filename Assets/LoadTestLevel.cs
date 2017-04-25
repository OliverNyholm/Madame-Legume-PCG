using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadTestLevel : MonoBehaviour {

    public string levelToLoad;
    public List<GameObject> platforms, fruits;
    public GameObject blade;

    string lhs;
    List<Transform> instantiateTransform;
    private GameObject start;

    // Use this for initialization
    void Awake () {
        LoadLevel();
        BuildLevel();
	}

    void LoadLevel()
    {
        instantiateTransform = ES2.LoadList<Transform>("instantiatedObjects" + levelToLoad);
        lhs = ES2.Load<string>("LHS" + levelToLoad);
        Debug.Log(lhs);
    }

    void BuildLevel()
    {
        start = Instantiate(platforms[0], new Vector2(0, 0), Quaternion.identity);
        start.GetComponentInChildren<SpriteRenderer>().color = Color.green;

        for (int i = 1; i < lhs.Length; i++)
        {
            if (lhs[i] == '1')
            {
                GameObject o = Instantiate(platforms[0], instantiateTransform[i].position, platforms[0].transform.rotation);
            }
            if (lhs[i] == '2')
            {
                GameObject o = Instantiate(platforms[0], instantiateTransform[i].position, platforms[0].transform.rotation);
            }
            if (lhs[i] == '3')
            {
                GameObject o = Instantiate(platforms[1], instantiateTransform[i].position, platforms[1].transform.rotation);
            }
            if (lhs[i] == '4')
            {
                GameObject o = Instantiate(platforms[1], instantiateTransform[i].position, platforms[1].transform.rotation);
            }
            if (lhs[i] == 'E')
            {
                GameObject o = Instantiate(platforms[2], instantiateTransform[i].position, platforms[2].transform.rotation);
            }
            if (lhs[i] == 'b')
            {
                GameObject o = Instantiate(blade, instantiateTransform[i].position, instantiateTransform[i].rotation);
            }
            if (lhs[i] == 'T')
            {
                GameObject o = Instantiate(fruits[0], instantiateTransform[i].position, instantiateTransform[i].rotation);
            }
            if (lhs[i] == 'B')
            {
                GameObject o = Instantiate(fruits[1], instantiateTransform[i].position, instantiateTransform[i].rotation);
            }
            if (lhs[i] == 'C')
            {
                GameObject o = Instantiate(fruits[2], instantiateTransform[i].position, instantiateTransform[i].rotation);
            }
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
