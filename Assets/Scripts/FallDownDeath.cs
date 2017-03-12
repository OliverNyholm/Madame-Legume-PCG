using UnityEngine;
using System.Collections;

public class FallDownDeath : MonoBehaviour
{

    PlayerController player;
    GE_LevelGenerator levelGen;
    public Vector3 startPosition;

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        startPosition = player.transform.position;

        if (GameObject.Find("CoolGELevel"))
        {
            levelGen = GameObject.Find("CoolGELevel").GetComponent<GE_LevelGenerator>();
            startPosition = levelGen.GetStartPosition();
            
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.name == "Player")
        {
            player.transform.position = startPosition;
        }
    }
}
