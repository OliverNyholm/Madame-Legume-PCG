using UnityEngine;
using System.Collections;

public class MovableBanana : MonoBehaviour
{
    Vector3 offset;
    Rigidbody2D rigi;
    PolygonCollider2D collider;
    EdgeCollider2D childCollider;
    SpriteRenderer renderer;
    private int previewLayer = 2; //Ingore Raycast layer
    public PolygonCollider2D polyColl;
    bool moving;
    bool colliding;

    void Start()
    {
        rigi = GetComponent<Rigidbody2D>();
        collider = GetComponent<PolygonCollider2D>();
        childCollider = transform.GetChild(0).GetComponent<EdgeCollider2D>();
        renderer = GetComponent<SpriteRenderer>();
        rigi.isKinematic = false;
    }

    void OnMouseDown()
    {
        offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));

        if (!moving)
        {
            moving = true;
            collider.isTrigger = true;
            childCollider.enabled = false;
            rigi.isKinematic = true;
        }
        else if (moving && !colliding)
        {
            moving = false;
            collider.isTrigger = false;
            childCollider.enabled = true;
            rigi.isKinematic = false;
        }

    }

    void Update()
    {
        if (moving)
        {
            Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
            Vector3 newposition = Camera.main.ScreenToWorldPoint(mousePos) + offset;
            polyColl.enabled = false;
            transform.position = newposition;
        }
        //else
            //polyColl.enabled = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer != previewLayer && moving)
        {
            renderer.color = new Color(0.93f, 0.43f, 0.58f, 0.9f);
            colliding = true;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer != previewLayer && moving)
        {
            renderer.color = new Color(0.93f, 0.43f, 0.58f, 0.9f);
            colliding = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer != previewLayer && moving)
        {
            renderer.color = new Color(1, 1, 1, 1);
            colliding = false;
        }
    }
}
