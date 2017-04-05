using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastToEnd : MonoBehaviour
{

    // Use this for initialization
    [SerializeField]
    bool isDrawAllRays;

    Vector3 left, right, middle, midLeft, midRight;
    Vector3 offsetBoxHeight = new Vector3(0, 0.5f);
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
        if (!isEndBoxReachable())
            return false;

        if (RetraceRight(previous) || RetraceMidRight(previous) || RetraceMiddle(previous) || RetraceLeft(previous) || RetraceMidLeft(previous))
        {
            return true;
        }

        return false;
    }

    private bool isEndBoxReachable()
    {
        Vector3 offset = new Vector3(1.1f, 0);
        RaycastHit2D raycastUpRight = Physics2D.Raycast(right, Vector2.up, 1.1f);
        RaycastHit2D raycastUpLeft = Physics2D.Raycast(right - offset, Vector2.up, 1.1f);

        //Debug.DrawRay(right, Vector2.up * 1.1f, Color.cyan, .5f);
        //Debug.DrawRay(right - offset, Vector2.up * 1.1f, Color.cyan, .5f);

        if (raycastUpRight.collider == null && raycastUpLeft.collider == null)
            return true;

        return false;
    }

    private bool RetraceRight(GameObject previous)
    {
        Vector3 rightToEndDirection = previous.transform.FindChild("RaycastArea").FindChild("EndPoint").position - right;
        Vector2 rightToStartDirection = previous.transform.FindChild("RaycastArea").FindChild("StartPoint").position - right;

        RaycastHit2D rightToEnd = Physics2D.Raycast(right, rightToEndDirection);
        RaycastHit2D rightToStart = Physics2D.Raycast(right, rightToStartDirection);

        //Debug.DrawRay(right, GetDrawDistance(rightToEndDirection, rightToEnd.distance), Color.white, .5f);
        //Debug.DrawRay(right, GetDrawDistance(rightToStartDirection, rightToStart.distance), Color.white, .5f);

        Collider2D hitBox = previous.GetComponent<Collider2D>();
        Collider2D movementBox = previous.transform.FindChild("MovementArea").GetComponent<Collider2D>();

        if (rightToEnd.collider != null && (rightToEnd.collider == hitBox || rightToEnd.collider == movementBox)
            || rightToStart.collider != null && (rightToStart.collider == hitBox || rightToStart.collider == movementBox))
        {
            Vector3 rightToEndBoxDirection = transform.FindChild("End").position - right;
            RaycastHit2D rightToEndBox = Physics2D.Raycast(right, rightToEndBoxDirection, rightToEndBoxDirection.magnitude);
            Debug.DrawRay(right, GetDrawDistance(rightToEndBoxDirection, rightToEndBox.distance), Color.green, .5f);

            Collider2D endBox = transform.FindChild("End").GetComponent<Collider2D>();
            if (rightToEndBox.collider != null && rightToEndBox.collider == endBox)
            {
                return true;
            }
        }
        return false;
    }

    private bool RetraceMidRight(GameObject previous)
    {
        Vector3 midRightToEndDirection = previous.transform.FindChild("RaycastArea").FindChild("EndPoint").position - midRight;
        Vector2 midRightToStartDirection = previous.transform.FindChild("RaycastArea").FindChild("StartPoint").position - midRight;

        RaycastHit2D midRightToEnd = Physics2D.Raycast(midRight, midRightToEndDirection);
        RaycastHit2D midRightToStart = Physics2D.Raycast(midRight, midRightToStartDirection);

        //Debug.DrawRay(midRight, GetDrawDistance(midRightToEndDirection, midRightToEnd.distance), Color.white, .5f);
        //Debug.DrawRay(midRight, GetDrawDistance(midRightToStartDirection, midRightToStart.distance), Color.white, .5f);

        Collider2D hitBox = previous.GetComponent<Collider2D>();
        Collider2D movementBox = previous.transform.FindChild("MovementArea").GetComponent<Collider2D>();

        if (midRightToEnd.collider != null && (midRightToEnd.collider == hitBox || midRightToEnd.collider == movementBox)
            || midRightToStart.collider != null && (midRightToStart.collider == hitBox || midRightToStart.collider == movementBox))
        {
            Vector3 midRightToEndBoxDirection = transform.FindChild("End").position - midRight;
            RaycastHit2D midRightToEndBox = Physics2D.Raycast(midRight, midRightToEndBoxDirection, midRightToEndBoxDirection.magnitude);
            Debug.DrawRay(midRight, GetDrawDistance(midRightToEndBoxDirection, midRightToEndBox.distance), Color.green, .5f);

            Collider2D endBox = transform.FindChild("End").GetComponent<Collider2D>();
            if (midRightToEndBox.collider != null && midRightToEndBox.collider == endBox)
            {
                return true;
            }
        }
        return false;
    }

    private bool RetraceMiddle(GameObject previous)
    {
        Vector3 middleToEndDirection = previous.transform.FindChild("RaycastArea").FindChild("EndPoint").position - middle;
        Vector2 middleToStartDirection = previous.transform.FindChild("RaycastArea").FindChild("StartPoint").position - middle;

        RaycastHit2D middleToEnd = Physics2D.Raycast(middle, middleToEndDirection);
        RaycastHit2D middleToStart = Physics2D.Raycast(middle, middleToStartDirection);

        //Debug.DrawRay(middle, GetDrawDistance(middleToEndDirection, middleToEnd.distance), Color.white, .5f);
        //Debug.DrawRay(middle, GetDrawDistance(middleToStartDirection, middleToStart.distance), Color.white, .5f);

        Collider2D hitBox = previous.GetComponent<Collider2D>();
        Collider2D movementBox = previous.transform.FindChild("MovementArea").GetComponent<Collider2D>();

        if (middleToEnd.collider != null && (middleToEnd.collider == hitBox || middleToEnd.collider == movementBox)
            || middleToStart.collider != null && (middleToStart.collider == hitBox || middleToStart.collider == movementBox))
        {
            Vector3 middleToEndBoxDirection = transform.FindChild("End").position - middle;
            RaycastHit2D middleToEndBox = Physics2D.Raycast(middle, middleToEndBoxDirection, middleToEndBoxDirection.magnitude);
            Debug.DrawRay(middle, GetDrawDistance(middleToEndBoxDirection, middleToEndBox.distance), Color.green, .5f);

            Collider2D endBox = transform.FindChild("End").GetComponent<Collider2D>();
            if (middleToEndBox.collider != null && middleToEndBox.collider == endBox)
            {
                return true;
            }
        }
        return false;
    }

    private bool RetraceMidLeft(GameObject previous)
    {
        Vector3 midLeftToEndDirection = previous.transform.FindChild("RaycastArea").FindChild("EndPoint").position - midLeft;
        Vector2 midLeftToStartDirection = previous.transform.FindChild("RaycastArea").FindChild("StartPoint").position - midLeft;

        RaycastHit2D midLeftToEnd = Physics2D.Raycast(midLeft, midLeftToEndDirection);
        RaycastHit2D midLeftToStart = Physics2D.Raycast(midLeft, midLeftToStartDirection);

        //Debug.DrawRay(midLeft, GetDrawDistance(midLeftToEndDirection, midLeftToEnd.distance), Color.white, .5f);
        //Debug.DrawRay(midLeft, GetDrawDistance(midLeftToStartDirection, midLeftToStart.distance), Color.white, .5f);

        Collider2D hitBox = previous.GetComponent<Collider2D>();
        Collider2D movementBox = previous.transform.FindChild("MovementArea").GetComponent<Collider2D>();

        if (midLeftToEnd.collider != null && (midLeftToEnd.collider == hitBox || midLeftToEnd.collider == movementBox)
            || midLeftToStart.collider != null && (midLeftToStart.collider == hitBox || midLeftToStart.collider == movementBox))
        {
            Vector3 midLeftToEndBoxDirection = transform.FindChild("End").position - midLeft;
            RaycastHit2D midLeftToEndBox = Physics2D.Raycast(midLeft, midLeftToEndBoxDirection, midLeftToEndBoxDirection.magnitude);
            Debug.DrawRay(midLeft, GetDrawDistance(midLeftToEndBoxDirection, midLeftToEndBox.distance), Color.green, .5f);

            Collider2D endBox = transform.FindChild("End").GetComponent<Collider2D>();
            if (midLeftToEndBox.collider != null && midLeftToEndBox.collider == endBox)
            {
                return true;
            }
        }
        return false;
    }

    private bool RetraceLeft(GameObject previous)
    {
        Vector3 leftToEndDirection = previous.transform.FindChild("RaycastArea").FindChild("EndPoint").position - left;
        Vector2 leftToStartDirection = previous.transform.FindChild("RaycastArea").FindChild("StartPoint").position - left;

        RaycastHit2D leftToEnd = Physics2D.Raycast(left, leftToEndDirection);
        RaycastHit2D leftToStart = Physics2D.Raycast(left, leftToStartDirection);

        //Debug.DrawRay(left, GetDrawDistance(leftToEndDirection, leftToEnd.distance), Color.white, .5f);
        //Debug.DrawRay(left, GetDrawDistance(leftToStartDirection, leftToStart.distance), Color.white, .5f);

        Collider2D hitBox = previous.GetComponent<Collider2D>();
        Collider2D movementBox = previous.transform.FindChild("MovementArea").GetComponent<Collider2D>();

        if (leftToEnd.collider != null && (leftToEnd.collider == hitBox || leftToEnd.collider == movementBox)
            || leftToStart.collider != null && (leftToStart.collider == hitBox || leftToStart.collider == movementBox))
        {
            Vector3 leftToEndBoxDirection = transform.FindChild("End").position - left;
            RaycastHit2D leftToEndBox = Physics2D.Raycast(left, leftToEndBoxDirection, leftToEndBoxDirection.magnitude);
            Debug.DrawRay(left, GetDrawDistance(leftToEndBoxDirection, leftToEndBox.distance), Color.green, .5f);

            Collider2D endBox = transform.FindChild("End").GetComponent<Collider2D>();
            if (leftToEndBox.collider != null && leftToEndBox.collider == endBox)
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
