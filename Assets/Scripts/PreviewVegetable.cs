using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PreviewVegetable : MonoBehaviour
{
    [SerializeField]
    string[] vegetables;

    private int index;
    private int previousIndex;

    private bool unavailable;
    public bool colliding;

    private Color red = new Color(255, 0, 0, 0.9f);
    private Color pink = new Color(0.93f, 0.43f, 0.58f, 0.9f);
    private Color white = new Color(1, 1, 1, 0.9f);

    // Update is called once per frame
    void Update()
    {

        // ---------------  Get mouse position ----------------
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 2.0f;
        Vector3 objectPosition = Camera.main.ScreenToWorldPoint(mousePos);

        // ---------------  Moves all preview vegetables to mouse position ----------------
        for (int i = 0; i < vegetables.Length; ++i)
            GameObject.Find(vegetables[i]).GetComponent<SpriteRenderer>().transform.position = objectPosition;

        // ---------------  Gets value if preview vegetable is colliding with an object or not ----------------
        colliding = GameObject.Find(vegetables[index]).GetComponent<PreviewVegetableCollision>().IsColliding();

        // ---------------  Sets color to pink or white, if the vegetable is available during the level ----------------
        if (!unavailable)
        {
            GameObject.Find(vegetables[index]).GetComponent<SpriteRenderer>().color = colliding ? pink : white;
        }
    }

    /// <summary>
    /// Draws a previewImage on the screen.
    /// </summary>
    /// <param name="index">Parameter value for which vegetable.</param>
    /// <param name="unavailable">Parameter value. True draws red color, false white .</param>
    public void DrawImage(int index, bool unavailable)
    {
        this.previousIndex = this.index;
        GameObject.Find(vegetables[previousIndex]).GetComponent<SpriteRenderer>().enabled = false;

        this.index = index;

        if (unavailable)
        {
            this.unavailable = unavailable;
            GameObject.Find(vegetables[index]).GetComponent<SpriteRenderer>().color = red;
        }
        else
        {
            this.unavailable = unavailable;
            GameObject.Find(vegetables[index]).GetComponent<SpriteRenderer>().color = white;
        }
        
        GameObject.Find(vegetables[index]).GetComponent<SpriteRenderer>().enabled = true;
    }


    /// <summary>
    /// Hides the image being drawn on screen
    /// </summary>
    public void HideImage()
    {
        GameObject.Find(vegetables[index]).GetComponent<SpriteRenderer>().enabled = false;
    }
}
