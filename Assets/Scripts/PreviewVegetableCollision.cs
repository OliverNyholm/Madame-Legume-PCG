using UnityEngine;
using System.Collections;

public class PreviewVegetableCollision : MonoBehaviour
{
    private int selfLayer = 2; //Ingore Raycast layer
    private bool colliding;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer != selfLayer)
            colliding = true;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer != selfLayer)
            colliding = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        colliding = false;
    }

    /// <summary>
    /// <returns>Returns a bool if preview vegetable is colliding with an object or not.</returns>
    /// </summary>
    public bool IsColliding()
    {
        return colliding;
    }

}
