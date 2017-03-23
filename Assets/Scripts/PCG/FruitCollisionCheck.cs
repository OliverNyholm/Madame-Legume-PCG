using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitCollisionCheck : MonoBehaviour {

    public bool colliding { get; set; }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Intersect!");

        colliding = true;
    }
}
