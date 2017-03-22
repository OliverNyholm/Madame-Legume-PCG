using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastPlayabilityStartPoint : MonoBehaviour
{

    Vector3 startPoint;
    // Use this for initialization
    void Awake()
    {
        startPoint = transform.FindChild("StartTopLeft").position;
        startPoint.x += 0.55f;
    }

    public bool checkStartPossible()
    {

        RaycastHit2D leftBottom = Physics2D.Raycast(startPoint + new Vector3(0, 0.37f * 1), Vector2.right, 1);
        RaycastHit2D leftMid1 = Physics2D.Raycast(startPoint + new Vector3(0, 0.37f * 2), Vector2.right, 1);
        RaycastHit2D leftMid2 = Physics2D.Raycast(startPoint + new Vector3(0, 0.37f * 3), Vector2.right, 1);
        RaycastHit2D leftMid3 = Physics2D.Raycast(startPoint + new Vector3(0, 0.37f * 4), Vector2.right, 2);
        RaycastHit2D leftTopJumpMax = Physics2D.Raycast(startPoint + new Vector3(0, 0.42f * 5), Vector2.right, 2);

        Debug.DrawRay(startPoint + new Vector3(0, 0.37f * 1), Vector3.right * 1, Color.gray, 2);
        Debug.DrawRay(startPoint + new Vector3(0, 0.37f * 2), Vector3.right * 1, Color.gray, 2);
        Debug.DrawRay(startPoint + new Vector3(0, 0.37f * 3), Vector3.right * 1, Color.gray, 2);
        Debug.DrawRay(startPoint + new Vector3(0, 0.37f * 4), Vector3.right * 2, Color.gray, 2);
        Debug.DrawRay(startPoint + new Vector3(0, 0.42f * 5), Vector3.right * 2, Color.gray, 2);

        if (leftBottom.collider == null && leftMid1.collider == null && leftMid2.collider == null && leftMid3.collider == null)
            return true;

        if (leftTopJumpMax.collider == null) //If others blocked, but can jump over it
            return true;

        Debug.Log("Impossible to play this level");
        return false;
    }
}
