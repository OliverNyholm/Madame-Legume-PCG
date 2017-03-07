using UnityEngine;
using System.Collections;

public class MovableCamera : MonoBehaviour
{
    private Vector3 startMousePos;

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            startMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            startMousePos.z = 0.0f;
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 nowMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            nowMousePos.z = 0.0f;
            transform.position += startMousePos - nowMousePos;
        }
    }
}
