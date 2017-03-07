using UnityEngine;
using System.Collections;

public class Banana : MonoBehaviour
{

    [SerializeField]
    private float impulse;

    //PlayerController controller;

    void OnCollisionEnter2D(Collision2D col)
    {
        //controller = col.gameObject.GetComponent<PlayerController>();
        //controller.onGround = false;
        if (col.gameObject.name == "Player")
        {
            ContactPoint2D contact = col.contacts[0];
            Vector3 tangent = Vector3.Cross(contact.normal, Vector3.back);

            Vector2 jumpForce = tangent * impulse;
            col.gameObject.GetComponent<Rigidbody2D>().AddForce(jumpForce, ForceMode2D.Impulse);

            Debug.DrawRay(contact.point, tangent, Color.red, 3);

        }

    }
}
