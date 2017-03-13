using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastPlayability : MonoBehaviour
{

    private RoomEndPoint roomEndPoint;

    private float playerHeightMin;
    private Vector3 startTopMiddle, startTopRight, startTopLeft, startBottomRight, startBottomLeft;
    private Vector3 endTopMiddle, endTopRight, endTopRightMiddle, endTopLeft, endTopLeftMiddle, endBottomMiddle, endBottomRight, endBottomLeft;

    public bool isCheckingPlayability;
    // Use this for initialization
    void Start()
    {
        roomEndPoint = gameObject.GetComponentInParent<RoomEndPoint>();

        playerHeightMin = 3f;

        startTopMiddle = getChildGameObject(gameObject, "StartTopMiddle").transform.position;
        startTopRight = getChildGameObject(gameObject, "StartTopRight").transform.position;
        startTopLeft = getChildGameObject(gameObject, "StartTopLeft").transform.position;
        startBottomRight = getChildGameObject(gameObject, "StartBottomRight").transform.position;
        startBottomLeft = getChildGameObject(gameObject, "StartBottomLeft").transform.position;

        endTopMiddle = getChildGameObject(gameObject, "TopMiddle").transform.position;
        endTopRight = getChildGameObject(gameObject, "TopRight").transform.position;
        endTopRightMiddle = getChildGameObject(gameObject, "TopRightMiddle").transform.position;
        endTopLeft = getChildGameObject(gameObject, "TopLeft").transform.position;
        endTopLeftMiddle = getChildGameObject(gameObject, "TopLeftMiddle").transform.position;

        endBottomMiddle = getChildGameObject(gameObject, "BottomMiddle").transform.position;
        endBottomRight = getChildGameObject(gameObject, "BottomRight").transform.position;
        endBottomLeft = getChildGameObject(gameObject, "BottomLeft").transform.position;

        isCheckingPlayability = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isCheckingPlayability)
        {
            if (CheckLeftSide())
                DrawRaysBottomLeft();
            if (CheckRightSide())
                DrawRaysBottomRight();
            else //Något fel med GetEndPosition :(((
                DrawRaysBottomRight();
            DrawRaysUp();
            DrawRaysTopRight();
            DrawRaysTopMiddle();
            DrawRaysTopLeft();
        }
    }

    bool CheckRightSide()
    {
        float minimunWidth = 1.2f;
        RaycastHit2D rightSide = Physics2D.Raycast(roomEndPoint.GetEndPosition(), Vector2.right, minimunWidth);

        Debug.DrawRay(rightSide.point, Vector2.right * minimunWidth, Color.red);

        if (rightSide.collider != null)
            return true;

        return false;
    }

    bool CheckLeftSide()
    {
        float minimunWidth = 1.2f;
        RaycastHit2D leftSide = Physics2D.Raycast(roomEndPoint.GetStartPosition(), Vector2.left, minimunWidth);

        Debug.DrawRay(leftSide.point, Vector2.left * minimunWidth, Color.red);

        if (leftSide.collider != null)
            return true;

        return false;
    }

    void DrawRaysUp()
    {

        RaycastHit2D topMiddle = Physics2D.Raycast(startTopMiddle, Vector2.up, playerHeightMin);
        RaycastHit2D topRight = Physics2D.Raycast(startTopRight, Vector2.up, playerHeightMin);
        RaycastHit2D topLeft = Physics2D.Raycast(startTopLeft, Vector2.up, playerHeightMin);

        Debug.DrawRay(startTopMiddle, Vector2.up * playerHeightMin, Color.red);
        Debug.DrawRay(startTopRight, Vector2.up * playerHeightMin, Color.red);
        Debug.DrawRay(startTopLeft, Vector2.up * playerHeightMin, Color.red);
    }

    void DrawRaysTopRight()
    {
        Vector3 topRightDirection = endTopRight - startTopRight;
        Vector3 topRightMiddleDirection = endTopRightMiddle - startTopRight;

        RaycastHit2D topRight = Physics2D.Raycast(startTopRight, Vector2.up, Vector3.Magnitude(topRightDirection));
        RaycastHit2D topRightMiddle = Physics2D.Raycast(startTopRight, Vector2.up, Vector3.Magnitude(topRightMiddleDirection));

        Debug.DrawRay(startTopRight, topRightDirection, Color.red);
        Debug.DrawRay(startTopRight, topRightMiddleDirection, Color.red);
    }

    void DrawRaysTopMiddle()
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

        Debug.DrawRay(startTopMiddle, topMiddleDirection, Color.red);
        Debug.DrawRay(startTopMiddle, topMiddleRightDirection, Color.red);
        Debug.DrawRay(startTopMiddle, topMiddleRightMiddleDirection, Color.red);
        Debug.DrawRay(startTopMiddle, topMiddleLeftDirection, Color.red);
        Debug.DrawRay(startTopMiddle, topMiddleLeftMiddleDirection, Color.red);
    }

    void DrawRaysTopLeft()
    {
        Vector3 topMiddleLeftDirection = endTopLeft - startTopLeft;
        Vector3 topMiddleLeftMiddleDirection = endTopLeftMiddle - startTopLeft;

        RaycastHit2D topLeft = Physics2D.Raycast(startTopLeft, Vector2.up, Vector3.Magnitude(topMiddleLeftDirection));
        RaycastHit2D topLeftMiddle = Physics2D.Raycast(startTopLeft, Vector2.up, Vector3.Magnitude(topMiddleLeftMiddleDirection));

        Debug.DrawRay(startTopLeft, topMiddleLeftDirection, Color.red);
        Debug.DrawRay(startTopLeft, topMiddleLeftMiddleDirection, Color.red);
    }

    void DrawRaysBottomRight()
    {
        Vector3 bottomRightDirection = endBottomRight - startBottomRight;
        Vector3 bottomMiddleDirection = endBottomMiddle - startBottomRight;
        Vector3 topRightDirection = endBottomRight - endTopRight;

        RaycastHit2D bottomRight = Physics2D.Raycast(startBottomRight, bottomRightDirection, Vector3.Magnitude(bottomRightDirection));
        RaycastHit2D bottomMiddle = Physics2D.Raycast(startBottomRight, bottomMiddleDirection, Vector3.Magnitude(bottomMiddleDirection));
        RaycastHit2D topRight = Physics2D.Raycast(endTopLeft, topRightDirection, Vector3.Magnitude(topRightDirection));

        Debug.DrawRay(startBottomRight, bottomRightDirection, Color.red);
        Debug.DrawRay(startBottomRight, bottomMiddleDirection, Color.red);
        Debug.DrawRay(endTopRight, topRightDirection, Color.red);
    }

    void DrawRaysBottomLeft()
    {
        Vector3 bottomLeftDirection = endBottomLeft - startBottomLeft;
        Vector3 bottomMiddleDirection = endBottomMiddle - startBottomLeft;
        Vector3 topLeftDirection = endBottomLeft - endTopLeft;

        RaycastHit2D bottomLeft = Physics2D.Raycast(startBottomLeft, bottomLeftDirection, Vector3.Magnitude(bottomLeftDirection));
        RaycastHit2D bottomMiddle = Physics2D.Raycast(startBottomLeft, bottomMiddleDirection, Vector3.Magnitude(bottomMiddleDirection));
        RaycastHit2D topLeft = Physics2D.Raycast(endTopLeft, topLeftDirection, Vector3.Magnitude(topLeftDirection));

        Debug.DrawRay(startBottomLeft, bottomLeftDirection, Color.red);
        Debug.DrawRay(startBottomLeft, bottomMiddleDirection, Color.red);
        Debug.DrawRay(endTopLeft, topLeftDirection, Color.red);
    }

    static public GameObject getChildGameObject(GameObject fromGameObject, string withName)
    {
        //Author: Isaac Dart, June-13.
        Transform[] ts = fromGameObject.transform.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in ts) if (t.gameObject.name == withName) return t.gameObject;
        return null;
    }
}
