using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastToEnd : MonoBehaviour
{

    // Use this for initialization
    [SerializeField]
    bool isDrawAllRays;

    Vector3 left, right, middle, midLeft, midRight;
    void Awake()
    {
        left = getChildGameObject(gameObject, "Left").transform.position;
        right = getChildGameObject(gameObject, "Right").transform.position;
        middle = getChildGameObject(gameObject, "Middle").transform.position;
        midLeft = getChildGameObject(gameObject, "MidLeft").transform.position;
        midRight = getChildGameObject(gameObject, "MidRight").transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDrawAllRays)
            RetraceLeft(GameObject.Find("Start(Clone)"));
    }

    public bool RetraceRaycast(GameObject previous)
    {
        if (RetraceLeft(previous))
        {
            return true;
        }

        return false;
    }

    private bool RetraceLeft(GameObject previous)
    {
        Vector3 leftToEndDirection = previous.transform.FindChild("RaycastArea").FindChild("EndPoint").position - left;
        Vector2 leftToStartDirection = previous.transform.FindChild("RaycastArea").FindChild("StartPoint").position - left;

        RaycastHit2D leftToEnd = Physics2D.Raycast(left, leftToEndDirection);
        RaycastHit2D leftToStart = Physics2D.Raycast(left, leftToStartDirection);

        Debug.DrawRay(left, GetDrawDistance(leftToEndDirection, leftToEnd.distance), Color.white, 5f);
        Debug.DrawRay(left, GetDrawDistance(leftToStartDirection, leftToStart.distance), Color.white, 5f);

        Collider2D temp1 = previous.transform.FindChild("RaycastArea").FindChild("Collider").GetComponent<Collider2D>();

        if (leftToEnd.collider != null && leftToEnd.collider == temp1
            || leftToStart.collider != null && leftToStart.collider == temp1)
        {
            Vector3 leftToEndBoxDirection = transform.FindChild("End").position - left;
            RaycastHit2D leftToEndBox = Physics2D.Raycast(left, leftToEndBoxDirection, leftToEndBoxDirection.magnitude);
            Debug.DrawRay(left, GetDrawDistance(leftToEndBoxDirection, leftToEndBox.distance), Color.green, 5f);

            Collider2D temp = transform.FindChild("End").GetComponent<Collider2D>();
            Debug.Log(temp.bounds);
            if (leftToEndBox.collider != null && leftToEndBox.collider == temp)
            {
                return true;
            }
        }

        return false;
    }

    Vector3 GetDrawDistance(Vector3 direction, float distance)
    {
        if (!isDrawAllRays)
            return direction.normalized * distance;

        return direction;
    }


    static public GameObject getChildGameObject(GameObject fromGameObject, string withName)
    {
        //Author: Isaac Dart, June-13.
        Transform[] ts = fromGameObject.transform.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in ts) if (t.gameObject.name == withName) return t.gameObject;
        return null;
    }
}
