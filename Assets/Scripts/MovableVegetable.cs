using UnityEngine;
using System.Collections;

public class MovableVegetable : MonoBehaviour
{
    Vector3 offset;
    Rigidbody2D rigi;
    private int previewLayer = 2; //Ingore Raycast layer
    bool moving;
    bool colliding;

    void Start()
    {
        rigi = GetComponent<Rigidbody2D>();
        rigi.isKinematic = false;
    }

    void OnMouseDown()
    {
        offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));

        if (!moving)
        {
            moving = true;
            GetComponent<PolygonCollider2D>().isTrigger = true;
            rigi.isKinematic = true;
        }
        else if(moving && !colliding)
        {
            moving = false;
            GetComponent<PolygonCollider2D>().isTrigger = false;
            rigi.isKinematic = false;
        }

    }

    void Update()
    {
        if (moving)
        {
            Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
            Vector3 newposition = Camera.main.ScreenToWorldPoint(mousePos) + offset;

            transform.position = newposition;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer != previewLayer && moving)
        {
            GetComponent<SpriteRenderer>().color = new Color(0.93f, 0.43f, 0.58f, 0.9f);
            colliding = true;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer != previewLayer && moving)
        {
            GetComponent<SpriteRenderer>().color = new Color(0.93f, 0.43f, 0.58f, 0.9f);
            colliding = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer != previewLayer && moving)
        {
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            colliding = false;
        }
    }
}
