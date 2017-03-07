using UnityEngine;
using System.Collections;

public class Tomato : MonoBehaviour
{

    [SerializeField]
    private float impulse;

    PlayerController controller;

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.name == "Player")
        {
            controller = col.gameObject.GetComponent<PlayerController>();
            controller.hitTomato = true;
            controller.onGround = false;

            ContactPoint2D contact = col.contacts[0];

            Vector2 jumpForce = -contact.normal;
            jumpForce *= impulse;
            col.gameObject.GetComponent<Rigidbody2D>().AddForce(jumpForce, ForceMode2D.Impulse);
        }

    }
}
