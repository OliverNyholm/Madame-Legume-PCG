using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastPlayability : MonoBehaviour
{
    private float playerHeightMin, playerJumpHeightMin;
    private Vector3 startTopMiddle, startTopRight, startTopLeft, startBottomRight, startBottomLeft, startPoint, endPoint, minPlayerHeightRight, minPlayerHeightLeft;
    private Vector3 endTopMiddle, endTopRight, endTopRightMiddle, endTopLeft, endTopLeftMiddle, endBottomMiddle, endBottomRight, endBottomRight2, endBottomLeft, endBottomLeft2;

    [SerializeField]
    public bool isCheckingPlayability;
    [SerializeField]
    public bool isDrawAllRays;

    private bool isPossibleToBeOnPlatform;
    // Use this for initialization
    void Awake()
    {
        playerHeightMin = 2f;
        playerJumpHeightMin = 4f;

        startTopMiddle = getChildGameObject(gameObject, "StartTopMiddle").transform.position;
        startTopRight = getChildGameObject(gameObject, "StartTopRight").transform.position;
        startTopLeft = getChildGameObject(gameObject, "StartTopLeft").transform.position;
        startBottomRight = getChildGameObject(gameObject, "StartBottomRight").transform.position;
        startBottomLeft = getChildGameObject(gameObject, "StartBottomLeft").transform.position;
        startPoint = getChildGameObject(gameObject, "StartPoint").transform.position;
        endPoint = getChildGameObject(gameObject, "EndPoint").transform.position;
        minPlayerHeightRight = getChildGameObject(gameObject, "MinPlayerHeightRight").transform.position;
        minPlayerHeightLeft = getChildGameObject(gameObject, "MinPlayerHeightLeft").transform.position;

        endTopMiddle = getChildGameObject(gameObject, "TopMiddle").transform.position;
        endTopRight = getChildGameObject(gameObject, "TopRight").transform.position;
        endTopRightMiddle = getChildGameObject(gameObject, "TopRightMiddle").transform.position;
        endTopLeft = getChildGameObject(gameObject, "TopLeft").transform.position;
        endTopLeftMiddle = getChildGameObject(gameObject, "TopLeftMiddle").transform.position;

        endBottomMiddle = getChildGameObject(gameObject, "BottomMiddle").transform.position;
        endBottomRight = getChildGameObject(gameObject, "BottomRight").transform.position;
        endBottomRight2 = getChildGameObject(gameObject, "BottomRight2").transform.position;
        endBottomLeft = getChildGameObject(gameObject, "BottomLeft").transform.position;
        endBottomLeft2 = getChildGameObject(gameObject, "BottomLeft2").transform.position;

        // isCheckingPlayability = true;
        isPossibleToBeOnPlatform = true;
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

        if (checkPlayerStandOnPlatform(otherCol))
            return true;

        if (!isPossibleToBeOnPlatform)
            return false;

        if (DrawRaysDownTopMiddle(otherCol))
            return true;

        if (isLeftSideColliding(otherCol))
                return true;
        if (isLeftSideTopClear())
            if (DrawRaysDownTopLeft(otherCol))
                return true;


        if (isRightSideColliding(otherCol))        
                return true;
        if (isRightSideTopClear())
            if (DrawRaysDownTopRight(otherCol))
                return true;

        if (DrawRaysTopMiddle(otherCol) || DrawRaysRight(otherCol) || DrawRaysLeft(otherCol))
        {
            return true;
        }

        return false;
    }

    bool isRightSideColliding(Collider2D other)
    {
        float minimunWidth = 1.2f;
        RaycastHit2D rightSideBottom = Physics2D.Raycast(endPoint, Vector2.right, minimunWidth);
        RaycastHit2D rightSideMid = Physics2D.Raycast(new Vector2(startTopRight.x - minimunWidth, startTopRight.y + playerHeightMin / 3), Vector2.right, minimunWidth * 2);
        RaycastHit2D rightSideTop = Physics2D.Raycast(new Vector2(minPlayerHeightRight.x - minimunWidth, minPlayerHeightRight.y), Vector2.right, minimunWidth + 0.8f);
        RaycastHit2D rightSideDown = Physics2D.Raycast(endPoint + new Vector3(minimunWidth, 0), Vector2.down, 0.8f);

        Debug.DrawRay(endPoint + new Vector3(minimunWidth, 0), GetDrawDistance(Vector3.down * 0.8f, rightSideDown.distance), Color.black);

        if (rightSideBottom.collider == null && rightSideMid.collider == null && rightSideTop.collider == null && rightSideDown.collider == null)
            if (DrawRaysBottomRight(other))
                return true;

        //if (rightSideMid.collider == null && rightSideTop.collider == null)
        //    if (DrawRaysTopRight(other))
        //        return true;

        return false;
    }

    bool isRightSideTopClear()
    {
        float minimunWidth = 1.2f;
        Vector3 startpos = new Vector2(startTopRight.x + 1, startTopRight.y + playerJumpHeightMin);
        Vector3 direction = endTopRight - startpos;
        Vector3 directionToStart = startpos - startTopRight;
        RaycastHit2D rightSideTop = Physics2D.Raycast(startpos, direction, direction.magnitude);
        RaycastHit2D rightSideMid = Physics2D.Raycast(minPlayerHeightRight, Vector2.right, minimunWidth);
        RaycastHit2D rightSideBot = Physics2D.Raycast(startTopRight, directionToStart, directionToStart.magnitude);

        Debug.DrawRay(startpos, GetDrawDistance(direction, rightSideTop.distance), Color.black);
        Debug.DrawRay(startTopRight, GetDrawDistance(directionToStart, rightSideBot.distance), Color.black);

        if (rightSideTop.collider == null && rightSideMid.collider == null && rightSideBot.collider == null)
            return true;

        return false;
    }

    bool isLeftSideColliding(Collider2D other)
    {
        float minimunWidth = 1.2f;
        RaycastHit2D leftSideBottom = Physics2D.Raycast(startPoint, Vector2.left, minimunWidth);
        RaycastHit2D leftSideMid = Physics2D.Raycast(new Vector2(startTopLeft.x + minimunWidth, startTopLeft.y + playerHeightMin / 3), Vector2.left, minimunWidth * 2);
        RaycastHit2D leftSideTop = Physics2D.Raycast(new Vector2(minPlayerHeightLeft.x + minimunWidth, minPlayerHeightLeft.y), Vector2.left, minimunWidth + 0.8f);
        RaycastHit2D leftSideDown = Physics2D.Raycast(startPoint - new Vector3(minimunWidth, 0), Vector2.down, 0.8f);

        Debug.DrawRay(startPoint - new Vector3(minimunWidth, 0), GetDrawDistance(Vector3.down * 0.8f, leftSideDown.distance), Color.black);

        if (leftSideBottom.collider == null && leftSideMid.collider == null && leftSideTop.collider == null && leftSideDown.collider == null)
            if (DrawRaysBottomLeft(other))
                return true;

        //if (leftSideMid.collider == null && leftSideTop.collider == null)
        //    if (DrawRaysTopLeft(other))
        //        return true;

        return false;
    }

    bool isLeftSideTopClear()
    {
        float minimunWidth = 1.2f;
        Vector3 startpos = new Vector2(startTopLeft.x - 1, startTopLeft.y + playerJumpHeightMin);
        Vector3 direction = endTopLeft - startpos;
        Vector3 directionToStart = startpos - startTopLeft;
        RaycastHit2D leftSideTop = Physics2D.Raycast(startpos, direction, direction.magnitude);
        RaycastHit2D leftSideBottom = Physics2D.Raycast(minPlayerHeightLeft, Vector2.left, minimunWidth);
        RaycastHit2D leftSideBot = Physics2D.Raycast(startTopLeft, directionToStart, directionToStart.magnitude);

        Debug.DrawRay(startpos, GetDrawDistance(direction, leftSideTop.distance), Color.black);
        Debug.DrawRay(startTopLeft, GetDrawDistance(directionToStart, leftSideBot.distance), Color.black);

        if (leftSideTop.collider == null && leftSideBottom.collider == null && leftSideBot.collider == null)
            return true;

        return false;
    }

    bool DrawRaysRight(Collider2D other)
    {
        float minimunWidth = 1.2f;
        RaycastHit2D rightSideBottom = Physics2D.Raycast(endPoint, Vector2.right, minimunWidth);
        RaycastHit2D rightSideBottom2 = Physics2D.Raycast(endPoint + new Vector3(0, 0.4f), Vector2.right, minimunWidth);
        RaycastHit2D rightSideMid = Physics2D.Raycast(new Vector2(startTopRight.x - minimunWidth, startTopRight.y + playerHeightMin / 3), Vector2.right, minimunWidth);
        RaycastHit2D rightSideTop = Physics2D.Raycast(new Vector2(minPlayerHeightRight.x - minimunWidth, minPlayerHeightRight.y), Vector2.right, minimunWidth + 0.8f);

        Debug.DrawRay(endPoint, GetDrawDistance(Vector2.right * minimunWidth, rightSideBottom.distance), Color.black);
        Debug.DrawRay(endPoint + new Vector3(0, 0.4f), GetDrawDistance(Vector2.right * minimunWidth, rightSideBottom.distance), Color.black);
        Debug.DrawRay(new Vector2(startTopRight.x - minimunWidth, startTopRight.y + playerHeightMin / 3), GetDrawDistance(Vector2.right * (minimunWidth * 2), rightSideMid.distance), Color.black);
        Debug.DrawRay(new Vector2(minPlayerHeightRight.x - minimunWidth, minPlayerHeightRight.y), GetDrawDistance(Vector2.right * (minimunWidth + 0.8f), rightSideTop.distance), Color.black);

        if (other == null)
            return false;

        if (rightSideBottom.collider != null && rightSideBottom.collider == other || rightSideBottom2.collider != null && rightSideBottom2.collider == other)// || rightSideMid.collider != null && rightSideMid.collider == other || rightSideTop.collider != null && rightSideTop.collider == other)
            return true;

        return false;
    }

    bool DrawRaysLeft(Collider2D other)
    {
        float minimunWidth = 1.2f;
        RaycastHit2D leftSideBottom = Physics2D.Raycast(startPoint, Vector2.left, minimunWidth);
        RaycastHit2D leftSideBottom2 = Physics2D.Raycast(startPoint + new Vector3(0, 0.4f), Vector2.left, minimunWidth);
        RaycastHit2D leftSideMid = Physics2D.Raycast(new Vector2(startTopLeft.x + minimunWidth, startTopLeft.y + playerHeightMin / 3), Vector2.left, minimunWidth * 2);
        RaycastHit2D leftSideTop = Physics2D.Raycast(new Vector2(minPlayerHeightLeft.x + minimunWidth, minPlayerHeightLeft.y), Vector2.left, minimunWidth + 0.8f);

        Debug.DrawRay(startPoint, GetDrawDistance(Vector2.left * minimunWidth, leftSideBottom.distance), Color.black);
        Debug.DrawRay(startPoint + new Vector3(0, 0.4f), GetDrawDistance(Vector2.left * minimunWidth, leftSideBottom.distance), Color.black);
        Debug.DrawRay(new Vector2(startTopLeft.x + minimunWidth, startTopLeft.y + playerHeightMin / 3), GetDrawDistance(Vector2.left * (minimunWidth * 2), leftSideMid.distance), Color.black);
        Debug.DrawRay(new Vector2(minPlayerHeightLeft.x + minimunWidth, minPlayerHeightLeft.y), GetDrawDistance(Vector2.left * (minimunWidth + 0.8f), leftSideTop.distance), Color.black);

        if (other == null)
            return false;

        if (leftSideBottom.collider != null && leftSideBottom.collider == other || leftSideBottom2.collider != null && leftSideBottom2.collider == other)// || leftSideMid.collider != null && leftSideMid.collider == other || leftSideTop.collider != null && leftSideTop.collider == other)
            return true;

        return false;
    }

    bool checkPlayerStandOnPlatform(Collider2D other)
    {
        float distance = startTopMiddle.x - startTopRight.x;

        RaycastHit2D topMiddle = Physics2D.Raycast(startTopMiddle, Vector2.up, playerHeightMin);
        RaycastHit2D topRight = Physics2D.Raycast(startTopRight, Vector2.up, playerHeightMin);
        RaycastHit2D topRight2 = Physics2D.Raycast(startTopRight + new Vector3(distance / 2, 0), Vector2.up, playerHeightMin);
        RaycastHit2D topLeft = Physics2D.Raycast(startTopLeft, Vector2.up, playerHeightMin);
        RaycastHit2D topLeft2 = Physics2D.Raycast(startTopLeft - new Vector3(distance / 2, 0), Vector2.up, playerHeightMin);

        Debug.DrawRay(startTopMiddle, GetDrawDistance(new Vector3(0, playerHeightMin), topMiddle.distance), Color.red);
        Debug.DrawRay(startTopRight, GetDrawDistance(new Vector3(0, playerHeightMin), topRight.distance), Color.red);
        Debug.DrawRay(startTopRight + new Vector3(distance / 2, 0), GetDrawDistance(new Vector3(0, playerHeightMin), topRight2.distance), Color.red);
        Debug.DrawRay(startTopLeft, GetDrawDistance(new Vector3(0, playerHeightMin), topLeft.distance), Color.red);
        Debug.DrawRay(startTopLeft - new Vector3(distance / 2, 0), GetDrawDistance(new Vector3(0, playerHeightMin), topLeft2.distance), Color.red);

        if (topMiddle.collider != null && topRight.collider != null && topRight2.collider != null && topLeft.collider != null && topLeft2.collider != null)
        {
            isPossibleToBeOnPlatform = false;
            return false;
        }
        isPossibleToBeOnPlatform = true;

        if (topMiddle.collider == null && topRight2.collider == null || topMiddle.collider == null && topLeft2.collider != null)
            if (DrawRaysDownTopMiddle(other))
                return true;

        if (topRight.collider == null)
            if (DrawRaysDownTopMiddleRight(other))
                return true;

        if (topLeft.collider == null)
            if (DrawRaysDownTopMiddleLeft(other))
                return true;


        return false;
    }

    bool DrawRaysTopRight(Collider2D other)
    {
        Vector3 topRightDirection = endTopRight - startTopRight;
        Vector3 topRightMiddleDirection = endTopRightMiddle - startTopRight;
        Vector3 topMiddleRightDirection = endTopRight - startTopMiddle;
        Vector3 topMiddleRightMiddleDirection = endTopRightMiddle - startTopMiddle;

        RaycastHit2D topRight = Physics2D.Raycast(startTopRight, topRightDirection, Vector3.Magnitude(topRightDirection));
        RaycastHit2D topRightMiddle = Physics2D.Raycast(startTopRight, topRightMiddleDirection, Vector3.Magnitude(topRightMiddleDirection));
        RaycastHit2D topMiddleRight = Physics2D.Raycast(startTopMiddle, topMiddleRightDirection, Vector3.Magnitude(topMiddleRightDirection));
        RaycastHit2D topMiddleRightMiddle = Physics2D.Raycast(startTopMiddle, topMiddleRightMiddleDirection, Vector3.Magnitude(topMiddleRightMiddleDirection));
        
        Debug.DrawRay(startTopRight, GetDrawDistance(topRightDirection, topRight.distance), Color.red);
        Debug.DrawRay(startTopRight, GetDrawDistance(topRightMiddleDirection, topRightMiddle.distance), Color.red);
        Debug.DrawRay(startTopMiddle, GetDrawDistance(topMiddleRightDirection, topMiddleRight.distance), Color.red);
        Debug.DrawRay(startTopMiddle, GetDrawDistance(topMiddleRightMiddleDirection, topMiddleRightMiddle.distance), Color.red);

        if (other == null)
            return false;

        if (topRight.collider != null && topRight.collider == other ||
            topRightMiddle.collider != null && topRightMiddle.collider == other ||
            topMiddleRight.collider != null && topMiddleRight.collider == other ||
            topMiddleRightMiddle.collider != null && topMiddleRightMiddle.collider == other)
            return true;

        return false;
    }

    bool DrawRaysTopMiddle(Collider2D other)
    {
        Vector3 topMiddleDirection = endTopMiddle - startTopMiddle;

        RaycastHit2D topMiddle = Physics2D.Raycast(startTopMiddle, topMiddleDirection, Vector3.Magnitude(topMiddleDirection));

        Debug.DrawRay(startTopMiddle, GetDrawDistance(topMiddleDirection, topMiddle.distance), Color.red);

        if (other == null)
            return false;

        if (topMiddle.collider != null && topMiddle.collider == other)
            return true;

        return false;
    }

    bool DrawRaysDownTopMiddle(Collider2D other)
    {
        Vector3 downDirection = startTopMiddle - endTopMiddle;
        Vector3 downRightDirection = startTopRight - endTopMiddle;
        Vector3 downLeftDirection = startTopLeft - endTopMiddle;

        RaycastHit2D downMiddle = Physics2D.Raycast(endTopMiddle, downDirection, downDirection.magnitude * 1.3f);
        RaycastHit2D downRight = Physics2D.Raycast(endTopMiddle, downRightDirection, downRightDirection.magnitude * 1.3f);
        RaycastHit2D downLeft = Physics2D.Raycast(endTopMiddle, downLeftDirection, downLeftDirection.magnitude * 1.3f);

        Debug.DrawRay(endTopMiddle, GetDrawDistance(downDirection, downMiddle.distance), Color.yellow);
        Debug.DrawRay(endTopMiddle, GetDrawDistance(downRightDirection, downRight.distance), Color.yellow);
        Debug.DrawRay(endTopMiddle, GetDrawDistance(downLeftDirection, downLeft.distance), Color.yellow);

        if (downMiddle.collider != null && downMiddle.collider == other || downRight.collider != null && downRight.collider == other || downLeft.collider != null && downLeft.collider == other)
            return true;

        return false;
    }

    bool DrawRaysDownTopMiddleRight(Collider2D other)
    {
        Vector3 startPos = endTopMiddle + new Vector3((startTopRight.x - startTopMiddle.x) / 1.5f, 0);
        Vector3 downLeftDirection = startTopMiddle - startPos;
        Vector3 downRightDirection = new Vector3(downLeftDirection.x * -1, downLeftDirection.y);

        RaycastHit2D downRight = Physics2D.Raycast(startPos, downRightDirection, downRightDirection.magnitude * 1.3f);
        RaycastHit2D downLeft = Physics2D.Raycast(startPos, downLeftDirection, downLeftDirection.magnitude * 1.3f);

        Debug.DrawRay(startPos, GetDrawDistance(downRightDirection, downRight.distance), Color.blue);
        Debug.DrawRay(startPos, GetDrawDistance(downLeftDirection, downLeft.distance), Color.blue);

        if (downLeft.collider != null && downLeft.collider == other || downRight.collider != null && downRight.collider == other)
            return true;

        return false;
    }

    bool DrawRaysDownTopMiddleLeft(Collider2D other)
    {
        Vector3 startPos = endTopMiddle - new Vector3((startTopMiddle.x - startTopLeft.x) / 1.5f, 0);
        Vector3 downLeftDirection = startTopMiddle - startPos;
        Vector3 downRightDirection = new Vector3(downLeftDirection.x * -1, downLeftDirection.y);

        RaycastHit2D downRight = Physics2D.Raycast(startPos, downRightDirection, downRightDirection.magnitude * 1.3f);
        RaycastHit2D downLeft = Physics2D.Raycast(startPos, downLeftDirection, downLeftDirection.magnitude * 1.3f);

        Debug.DrawRay(startPos, GetDrawDistance(downRightDirection, downRight.distance), Color.blue);
        Debug.DrawRay(startPos, GetDrawDistance(downLeftDirection, downLeft.distance), Color.blue);

        if (downLeft.collider != null && downLeft.collider == other || downRight.collider != null && downRight.collider == other)
            return true;

        return false;
    }

    bool DrawRaysTopLeft(Collider2D other)
    {
        Vector3 topLeftDirection = endTopLeft - startTopLeft;
        Vector3 topLeftMiddleDirection = endTopLeftMiddle - startTopLeft;
        Vector3 topMiddleLeftDirection = endTopLeft - startTopMiddle;
        Vector3 topMiddleLeftMiddleDirection = endTopLeftMiddle - startTopMiddle;

        RaycastHit2D topLeft = Physics2D.Raycast(startTopLeft, topLeftDirection, Vector3.Magnitude(topLeftDirection));
        RaycastHit2D topLeftMiddle = Physics2D.Raycast(startTopLeft, topLeftMiddleDirection, Vector3.Magnitude(topLeftMiddleDirection));
        RaycastHit2D topMiddleLeft = Physics2D.Raycast(startTopMiddle, topMiddleLeftDirection, Vector3.Magnitude(topMiddleLeftDirection));
        RaycastHit2D topMiddleLeftMiddle = Physics2D.Raycast(startTopMiddle, topMiddleLeftMiddleDirection, Vector3.Magnitude(topMiddleLeftMiddleDirection));

        Debug.DrawRay(startTopLeft, GetDrawDistance(topLeftDirection, topLeft.distance), Color.red);
        Debug.DrawRay(startTopLeft, GetDrawDistance(topLeftMiddleDirection, topLeftMiddle.distance), Color.red);
        Debug.DrawRay(startTopMiddle, GetDrawDistance(topMiddleLeftDirection, topMiddleLeft.distance), Color.red);
        Debug.DrawRay(startTopMiddle, GetDrawDistance(topMiddleLeftMiddleDirection, topMiddleLeftMiddle.distance), Color.red);

        if (other == null)
            return false;

        if (topLeft.collider != null && topLeft.collider == other ||
            topLeftMiddle.collider != null && topLeftMiddle.collider == other ||
            topMiddleLeft.collider != null && topMiddleLeft.collider == other ||
            topMiddleLeftMiddle.collider != null && topMiddleLeftMiddle.collider == other)
            return true;

        return false;
    }

    bool DrawRaysBottomRight(Collider2D other)
    {
        float distance = (endBottomRight2.x - endBottomMiddle.x) / 4;
        Vector3 bottomRightDirection1 = endBottomRight2 - new Vector3(distance * 1, 0) - startBottomRight;
        Vector3 bottomRightDirection2 = endBottomRight2 - new Vector3(distance * 2, 0) - startBottomRight;
        Vector3 bottomRightDirection3 = endBottomRight2 - new Vector3(distance * 3, 0) - startBottomRight;
        Vector3 bottomRightDirection4 = endBottomRight2 -(startTopRight + new Vector3(1.2f, 0)); //From top of platform
        Vector3 bottomMiddleDirection = endBottomMiddle - startBottomRight;

        RaycastHit2D bottomRight1 = Physics2D.Raycast(startBottomRight, bottomRightDirection1);
        RaycastHit2D bottomRight2 = Physics2D.Raycast(startBottomRight, bottomRightDirection2);
        RaycastHit2D bottomRight3 = Physics2D.Raycast(startBottomRight, bottomRightDirection3);
        RaycastHit2D bottomRight4 = Physics2D.Raycast(startTopRight + new Vector3(1.2f, 0), bottomRightDirection4);
        RaycastHit2D bottomMiddle = Physics2D.Raycast(startBottomRight, bottomMiddleDirection);

        Debug.DrawRay(startBottomRight, GetDrawDistance(bottomRightDirection1, bottomRight1.distance), Color.magenta);
        Debug.DrawRay(startBottomRight, GetDrawDistance(bottomRightDirection2, bottomRight2.distance), Color.magenta);
        Debug.DrawRay(startBottomRight, GetDrawDistance(bottomRightDirection3, bottomRight3.distance), Color.magenta);
        Debug.DrawRay(startTopRight + new Vector3(1.2f, 0), GetDrawDistance(bottomRightDirection4, bottomRight4.distance), Color.magenta);
        Debug.DrawRay(startBottomRight, GetDrawDistance(bottomMiddleDirection, bottomMiddle.distance), Color.magenta);

        if (other == null)
            return false;

        if (bottomRight1.collider != null && bottomRight1.collider == other ||
            bottomRight2.collider != null && bottomRight2.collider == other ||
            bottomRight3.collider != null && bottomRight3.collider == other ||
            bottomRight4.collider != null && bottomRight4.collider == other ||
            bottomMiddle.collider != null && bottomMiddle.collider == other)
            return true;

        return false;
    }

    bool DrawRaysDownTopRight(Collider2D other)
    {
        float distance = (endBottomRight.x - endBottomMiddle.x) / 5;
        Vector3 bottomRightDirection1 = endBottomMiddle + new Vector3(distance * 1, 0) - endTopRight;
        Vector3 bottomRightDirection2 = endBottomMiddle + new Vector3(distance * 2, 0) - endTopRight;
        Vector3 bottomRightDirection3 = endBottomMiddle + new Vector3(distance * 3, 0) - endTopRight;
        Vector3 bottomRightDirection4 = endBottomMiddle + new Vector3(distance * 4, 0) - endTopRight;
        Vector3 bottomRightDirection5 = endBottomRight - endTopRight;

        RaycastHit2D bottomRight1 = Physics2D.Raycast(endTopRight, bottomRightDirection1);
        RaycastHit2D bottomRight2 = Physics2D.Raycast(endTopRight, bottomRightDirection2);
        RaycastHit2D bottomRight3 = Physics2D.Raycast(endTopRight, bottomRightDirection3);
        RaycastHit2D bottomRight4 = Physics2D.Raycast(endTopRight, bottomRightDirection4);
        RaycastHit2D bottomRight5 = Physics2D.Raycast(endTopRight, bottomRightDirection5);

        Debug.DrawRay(endTopRight, GetDrawDistance(bottomRightDirection1, bottomRight1.distance), Color.cyan);
        Debug.DrawRay(endTopRight, GetDrawDistance(bottomRightDirection2, bottomRight2.distance), Color.cyan);
        Debug.DrawRay(endTopRight, GetDrawDistance(bottomRightDirection3, bottomRight3.distance), Color.cyan);
        Debug.DrawRay(endTopRight, GetDrawDistance(bottomRightDirection4, bottomRight4.distance), Color.cyan);
        Debug.DrawRay(endTopRight, GetDrawDistance(bottomRightDirection5, bottomRight5.distance), Color.cyan);

        if (other == null)
            return false;

        if (bottomRight1.collider != null && bottomRight1.collider == other || bottomRight2.collider != null && bottomRight2.collider == other ||
            bottomRight3.collider != null && bottomRight3.collider == other || bottomRight4.collider != null && bottomRight4.collider == other || bottomRight5.collider != null && bottomRight5.collider == other)
            return true;

        return false;
    }

    bool DrawRaysBottomLeft(Collider2D other)
    {
        float distance = (endBottomLeft2.x - endBottomMiddle.x) / 4;
        Vector3 bottomLeftDirection1 = endBottomLeft2 - new Vector3(distance * 1, 0) - startBottomLeft;
        Vector3 bottomLeftDirection2 = endBottomLeft2 - new Vector3(distance * 2, 0) - startBottomLeft;
        Vector3 bottomLeftDirection3 = endBottomLeft2 - new Vector3(distance * 3, 0) - startBottomLeft;
        Vector3 bottomLeftDirection4 = endBottomLeft2 - (startTopLeft - new Vector3(1.2f, 0)); //From top of platform
        Vector3 bottomMiddleDirection = endBottomMiddle - startBottomLeft;

        RaycastHit2D bottomLeft1 = Physics2D.Raycast(startBottomLeft, bottomLeftDirection1);
        RaycastHit2D bottomLeft2 = Physics2D.Raycast(startBottomLeft, bottomLeftDirection2);
        RaycastHit2D bottomLeft3 = Physics2D.Raycast(startBottomLeft, bottomLeftDirection3);
        RaycastHit2D bottomLeft4 = Physics2D.Raycast(startTopLeft - new Vector3(1.2f, 0), bottomLeftDirection4);
        RaycastHit2D bottomMiddle = Physics2D.Raycast(startBottomLeft, bottomMiddleDirection);

        Debug.DrawRay(startBottomLeft, GetDrawDistance(bottomLeftDirection1, bottomLeft1.distance), Color.magenta);
        Debug.DrawRay(startBottomLeft, GetDrawDistance(bottomLeftDirection2, bottomLeft2.distance), Color.magenta);
        Debug.DrawRay(startBottomLeft, GetDrawDistance(bottomLeftDirection3, bottomLeft3.distance), Color.magenta);
        Debug.DrawRay(startTopLeft - new Vector3(1.2f, 0), GetDrawDistance(bottomLeftDirection4, bottomLeft4.distance), Color.magenta);
        Debug.DrawRay(startBottomLeft, GetDrawDistance(bottomMiddleDirection, bottomMiddle.distance), Color.magenta);

        if (other == null)
            return false;

        if (bottomLeft1.collider != null && bottomLeft1.collider == other ||
            bottomLeft2.collider != null && bottomLeft2.collider == other ||
            bottomLeft3.collider != null && bottomLeft3.collider == other ||
            bottomLeft4.collider != null && bottomLeft4.collider == other ||
            bottomMiddle.collider != null && bottomMiddle.collider == other)
            return true;

        return false;
    }

    bool DrawRaysDownTopLeft(Collider2D other)
    {
        float distance = (endBottomMiddle.x - endBottomLeft.x) / 5;
        Vector3 bottomLeftDirection1 = endBottomMiddle - new Vector3(distance * 1, 0) - endTopLeft;
        Vector3 bottomLeftDirection2 = endBottomMiddle - new Vector3(distance * 2, 0) - endTopLeft;
        Vector3 bottomLeftDirection3 = endBottomMiddle - new Vector3(distance * 3, 0) - endTopLeft;
        Vector3 bottomLeftDirection4 = endBottomMiddle - new Vector3(distance * 4, 0) - endTopLeft;
        Vector3 bottomLeftDirection5 = endBottomLeft - endTopLeft;

        RaycastHit2D bottomLeft1 = Physics2D.Raycast(endTopLeft, bottomLeftDirection1);
        RaycastHit2D bottomLeft2 = Physics2D.Raycast(endTopLeft, bottomLeftDirection2);
        RaycastHit2D bottomLeft3 = Physics2D.Raycast(endTopLeft, bottomLeftDirection3);
        RaycastHit2D bottomLeft4 = Physics2D.Raycast(endTopLeft, bottomLeftDirection4);
        RaycastHit2D bottomLeft5 = Physics2D.Raycast(endTopLeft, bottomLeftDirection5);

        Debug.DrawRay(endTopLeft, GetDrawDistance(bottomLeftDirection1, bottomLeft1.distance), Color.cyan);
        Debug.DrawRay(endTopLeft, GetDrawDistance(bottomLeftDirection2, bottomLeft2.distance), Color.cyan);
        Debug.DrawRay(endTopLeft, GetDrawDistance(bottomLeftDirection3, bottomLeft3.distance), Color.cyan);
        Debug.DrawRay(endTopLeft, GetDrawDistance(bottomLeftDirection4, bottomLeft4.distance), Color.cyan);
        Debug.DrawRay(endTopLeft, GetDrawDistance(bottomLeftDirection5, bottomLeft5.distance), Color.cyan);

        if (other == null)
            return false;

        if (bottomLeft1.collider != null && bottomLeft1.collider == other || bottomLeft2.collider != null && bottomLeft2.collider == other ||
            bottomLeft3.collider != null && bottomLeft3.collider == other || bottomLeft4.collider != null && bottomLeft4.collider == other || bottomLeft5.collider != null && bottomLeft5.collider == other)
            return true;

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
