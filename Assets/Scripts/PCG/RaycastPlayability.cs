using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastPlayability : MonoBehaviour
{
    private float playerHeightMin;
    private Vector3 startTopMiddle, startTopRight, startTopLeft, startBottomRight, startBottomLeft, startPoint, endPoint;
    private Vector3 endTopMiddle, endTopRight, endTopRightMiddle, endTopLeft, endTopLeftMiddle, endBottomMiddle, endBottomRight, endBottomLeft;

    [SerializeField]
    public bool isCheckingPlayability;
    // Use this for initialization
    void Awake()
    {
        playerHeightMin = 3f;

        startTopMiddle = getChildGameObject(gameObject, "StartTopMiddle").transform.position;
        startTopRight = getChildGameObject(gameObject, "StartTopRight").transform.position;
        startTopLeft = getChildGameObject(gameObject, "StartTopLeft").transform.position;
        startBottomRight = getChildGameObject(gameObject, "StartBottomRight").transform.position;
        startBottomLeft = getChildGameObject(gameObject, "StartBottomLeft").transform.position;
        startPoint = getChildGameObject(gameObject, "StartPoint").transform.position;
        endPoint = getChildGameObject(gameObject, "EndPoint").transform.position;

        endTopMiddle = getChildGameObject(gameObject, "TopMiddle").transform.position;
        endTopRight = getChildGameObject(gameObject, "TopRight").transform.position;
        endTopRightMiddle = getChildGameObject(gameObject, "TopRightMiddle").transform.position;
        endTopLeft = getChildGameObject(gameObject, "TopLeft").transform.position;
        endTopLeftMiddle = getChildGameObject(gameObject, "TopLeftMiddle").transform.position;

        endBottomMiddle = getChildGameObject(gameObject, "BottomMiddle").transform.position;
        endBottomRight = getChildGameObject(gameObject, "BottomRight").transform.position;
        endBottomLeft = getChildGameObject(gameObject, "BottomLeft").transform.position;

        isCheckingPlayability = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isCheckingPlayability)
        {
            isRayHittingPlatform(null);
        }
    }

    public bool isRayHittingPlatform(GameObject other)
    {
        //isCheckingPlayability = true;

        // other.GetComponentInChildren<Collider2D>()
        Collider2D otherCol;
        if (other)
            otherCol = other.transform.FindChild("MovementArea").GetComponent<Collider2D>();
        else
            otherCol = null;

        if (isLeftSideClear())
            if (DrawRaysBottomLeft(otherCol))
                return true;
        if (isRightSideClear())
            if (DrawRaysBottomRight(otherCol))
                return true;

        if (DrawRaysUp(otherCol) || DrawRaysTopRight(otherCol) || DrawRaysTopMiddle(otherCol) || DrawRaysTopLeft(otherCol) || DrawRaysRight(otherCol) || DrawRaysLeft(otherCol))
        {
            return true;
        }

        return false;
    }

    bool isRightSideClear()
    {
        float minimunWidth = 1.2f;
        RaycastHit2D rightSide = Physics2D.Raycast(endPoint, Vector2.right, minimunWidth);


        //Debug.DrawRay(endPoint, Vector2.right * minimunWidth, Color.red);

        if (rightSide.collider == null)
            return true;

        return false;
    }

    bool isLeftSideClear()
    {
        float minimunWidth = 1.2f;
        RaycastHit2D leftSide = Physics2D.Raycast(startPoint, Vector2.left, minimunWidth);

        //Debug.DrawRay(startPoint, Vector2.left * minimunWidth, Color.red);

        if (leftSide.collider == null)
            return true;

        return false;
    }

    bool DrawRaysRight(Collider2D other)
    {
        float minimunWidth = 1.2f;
        RaycastHit2D rightSide = Physics2D.Raycast(endPoint, Vector2.right, minimunWidth);

        Debug.DrawRay(endPoint, Vector2.right * rightSide.distance, Color.red);

        if (other == null)
            return false;

        if (rightSide.collider != null && rightSide.collider == other)
            return true;

        return false;
    }

    bool DrawRaysLeft(Collider2D other)
    {
        float minimunWidth = 1.2f;
        RaycastHit2D leftSide = Physics2D.Raycast(startPoint, Vector2.left, minimunWidth);

        Debug.DrawRay(startPoint, Vector2.left * leftSide.distance, Color.red);

        if (other == null)
            return false;

        if (leftSide.collider != null && leftSide.collider == other)
            return true;

        return false;
    }


    bool DrawRaysUp(Collider2D other)
    {

        RaycastHit2D topMiddle = Physics2D.Raycast(startTopMiddle, Vector2.up, playerHeightMin);
        RaycastHit2D topRight = Physics2D.Raycast(startTopRight, Vector2.up, playerHeightMin);
        RaycastHit2D topLeft = Physics2D.Raycast(startTopLeft, Vector2.up, playerHeightMin);


        Debug.DrawRay(startTopMiddle, Vector2.up  * topMiddle.distance, Color.red);
        Debug.DrawRay(startTopRight, Vector2.up * topRight.distance, Color.red);
        Debug.DrawRay(startTopLeft, Vector2.up * topLeft.distance, Color.red);

        if (other == null)
            return false;

        if (topMiddle.collider != null && topMiddle.collider == other ||
            topRight.collider != null && topRight.collider == other ||
            topLeft.collider != null && topLeft.collider == other)
            return true;

        return false;
    }

    bool DrawRaysTopRight(Collider2D other)
    {
        Vector3 topRightDirection = endTopRight - startTopRight;
        Vector3 topRightMiddleDirection = endTopRightMiddle - startTopRight;

        RaycastHit2D topRight = Physics2D.Raycast(startTopRight, topRightDirection, Vector3.Magnitude(topRightDirection));
        RaycastHit2D topRightMiddle = Physics2D.Raycast(startTopRight, topRightMiddleDirection, Vector3.Magnitude(topRightMiddleDirection));

        Debug.DrawRay(startTopRight, topRightDirection.normalized * topRight.distance, Color.red);
        Debug.DrawRay(startTopRight, topRightMiddleDirection.normalized * topRightMiddle.distance, Color.red);

        if (other == null)
            return false;

        if (topRight.collider != null && topRight.collider == other ||
            topRightMiddle.collider != null && topRightMiddle.collider == other)
            return true;

        return false;
    }

    bool DrawRaysTopMiddle(Collider2D other)
    {
        Vector3 topMiddleDirection = endTopMiddle - startTopMiddle;
        Vector3 topMiddleRightDirection = endTopRight - startTopMiddle;
        Vector3 topMiddleRightMiddleDirection = endTopRightMiddle - startTopMiddle;
        Vector3 topMiddleLeftDirection = endTopLeft - startTopMiddle;
        Vector3 topMiddleLeftMiddleDirection = endTopLeftMiddle - startTopMiddle;

        RaycastHit2D topMiddle = Physics2D.Raycast(startTopMiddle, topMiddleDirection, Vector3.Magnitude(topMiddleDirection));
        RaycastHit2D topMiddleRight = Physics2D.Raycast(startTopMiddle, topMiddleRightDirection, Vector3.Magnitude(topMiddleRightDirection));
        RaycastHit2D topMiddleRightMiddle = Physics2D.Raycast(startTopMiddle, topMiddleRightMiddleDirection, Vector3.Magnitude(topMiddleRightMiddleDirection));
        RaycastHit2D topMiddleLeft = Physics2D.Raycast(startTopMiddle, topMiddleLeftDirection, Vector3.Magnitude(-topMiddleRightDirection));
        RaycastHit2D topMiddleLeftMiddle = Physics2D.Raycast(startTopMiddle, topMiddleLeftMiddleDirection, Vector3.Magnitude(-topMiddleRightMiddleDirection));

        Debug.DrawRay(startTopMiddle, topMiddleDirection.normalized * topMiddle.distance, Color.red);
        Debug.DrawRay(startTopMiddle, topMiddleRightDirection.normalized * topMiddleRight.distance, Color.red);
        Debug.DrawRay(startTopMiddle, topMiddleRightMiddleDirection.normalized * topMiddleRightMiddle.distance, Color.red);
        Debug.DrawRay(startTopMiddle, topMiddleLeftDirection.normalized * topMiddleLeft.distance, Color.red);
        Debug.DrawRay(startTopMiddle, topMiddleLeftMiddleDirection.normalized * topMiddleLeftMiddle.distance, Color.red);


        if (other == null)
            return false;

        if (topMiddle.collider != null && topMiddle.collider == other ||
            topMiddleRight.collider != null && topMiddleRight.collider == other
            || topMiddleRightMiddle.collider != null && topMiddleRightMiddle.collider == other
            || topMiddleLeft.collider != null && topMiddleLeft.collider == other ||
            topMiddleLeftMiddle.collider != null && topMiddleLeftMiddle.collider == other)
            return true;

        return false;
    }

    bool DrawRaysTopLeft(Collider2D other)
    {
        Vector3 topMiddleLeftDirection = endTopLeft - startTopLeft;
        Vector3 topMiddleLeftMiddleDirection = endTopLeftMiddle - startTopLeft;

        RaycastHit2D topLeft = Physics2D.Raycast(startTopLeft, topMiddleLeftDirection, Vector3.Magnitude(topMiddleLeftDirection));
        RaycastHit2D topLeftMiddle = Physics2D.Raycast(startTopLeft, topMiddleLeftMiddleDirection, Vector3.Magnitude(topMiddleLeftMiddleDirection));

        Debug.DrawRay(startTopLeft, topMiddleLeftDirection.normalized * topLeft.distance, Color.red);
        Debug.DrawRay(startTopLeft, topMiddleLeftMiddleDirection.normalized * topLeftMiddle.distance, Color.red);

        if (other == null)
            return false;

        if (topLeft.collider != null && topLeft.collider == other ||
            topLeftMiddle.collider != null && topLeftMiddle.collider == other)
            return true;

        return false;
    }

    bool DrawRaysBottomRight(Collider2D other)
    {
        Vector3 bottomRightDirection = endBottomRight - startBottomRight;
        Vector3 bottomMiddleDirection = endBottomMiddle - startBottomRight;
        Vector3 topRightDirection = endBottomRight - endTopRight;

        RaycastHit2D bottomRight = Physics2D.Raycast(startBottomRight, bottomRightDirection, Vector3.Magnitude(bottomRightDirection));
        RaycastHit2D bottomMiddle = Physics2D.Raycast(startBottomRight, bottomMiddleDirection, Vector3.Magnitude(bottomMiddleDirection));
        RaycastHit2D topRight = Physics2D.Raycast(endTopRight, topRightDirection, Vector3.Magnitude(topRightDirection));

        Debug.DrawRay(startBottomRight, bottomRightDirection.normalized * bottomRight.distance, Color.red);
        Debug.DrawRay(startBottomRight, bottomMiddleDirection.normalized * bottomMiddle.distance, Color.red);
        Debug.DrawRay(endTopRight, topRightDirection.normalized * topRight.distance, Color.red);

        if (other == null)
            return false;

        if (bottomRight.collider != null && bottomRight.collider == other ||
            bottomMiddle.collider != null && bottomMiddle.collider == other ||
            topRight.collider != null && topRight.collider == other)
            return true;

        return false;
    }

    bool DrawRaysBottomLeft(Collider2D other)
    {


        Vector3 bottomLeftDirection = endBottomLeft - startBottomLeft;
        Vector3 bottomMiddleDirection = endBottomMiddle - startBottomLeft;
        Vector3 topLeftDirection = endBottomLeft - endTopLeft;

        RaycastHit2D bottomLeft = Physics2D.Raycast(startBottomLeft, bottomLeftDirection, Vector3.Magnitude(bottomLeftDirection));
        RaycastHit2D bottomMiddle = Physics2D.Raycast(startBottomLeft, bottomMiddleDirection, Vector3.Magnitude(bottomMiddleDirection));
        RaycastHit2D topLeft = Physics2D.Raycast(endTopLeft, topLeftDirection, Vector3.Magnitude(topLeftDirection));

        Debug.DrawRay(startBottomLeft, bottomLeftDirection.normalized * bottomLeft.distance, Color.red);
        Debug.DrawRay(startBottomLeft, bottomMiddleDirection.normalized * bottomMiddle.distance, Color.red);
        Debug.DrawRay(endTopLeft, topLeftDirection.normalized * topLeft.distance, Color.red);

        if (other == null)
            return false;

        if (bottomLeft.collider != null && bottomLeft.collider == other ||
            bottomMiddle.collider != null && bottomMiddle.collider == other ||
            topLeft.collider != null && topLeft.collider == other)
            return true;

        return false;
    }

    static public GameObject getChildGameObject(GameObject fromGameObject, string withName)
    {
        //Author: Isaac Dart, June-13.
        Transform[] ts = fromGameObject.transform.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in ts) if (t.gameObject.name == withName) return t.gameObject;
        return null;
    }
}
