using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEndPoint : MonoBehaviour {

    //[SerializeField]
    //private Vector2 startPosition;

    //[SerializeField]
    //private Vector2 endPosition;

    // Use this for initialization
    void Start () {
        //startPosition = transform.Find("StartPoint").transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public Vector2 GetStartPosition()
    {
        return transform.Find("StartPoint").transform.position;
    }

    public Vector2 GetEndPosition()
    {
        return transform.Find("EndPoint").transform.position;
    }

    public float GetObjectWidth()
    {
        
        return Mathf.Abs(transform.Find("EndPoint").transform.position.x - transform.Find("StartPoint").transform.position.x); 
    }

    public float GetObjectHeight()
    {
        return Mathf.Abs(transform.Find("EndPoint").transform.position.y - transform.Find("StartPoint").transform.position.y);
    }
}
