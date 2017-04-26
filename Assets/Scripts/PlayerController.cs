using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float maxSpeed = 5;
    [SerializeField]
    private float jumpHeight = 4;
    [SerializeField]
    private float timeToJumpApex = .4f;

    private float gravity;
    private float jumpVelocity;
    private float velocityXSmoothing;

    private Rigidbody2D rigi;
    private Animator anim;
    private Camera camera;
    private Vector3 cameraOffset;

    [SerializeField]
    private LayerMask ground;
    [SerializeField]
    private LayerMask banana;

    bool facingRight = true;
    public bool onGround = false;
    public bool onBanana = false;
    public bool leftBanana = false;
    private float bananaTimer = 0.3f;
    public bool hitTomato = false;
    private float tomatoTimer = 0.2f;

    [SerializeField]
    private Object[] vegetable;
    private int vegetableIndex;
    public int[] vegetableCount;
    LevelData levelData;

    PreviewVegetable platformRender;
    private bool showPlatform;

    private Text vegetableText;


    void Start()
    {
        rigi = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        camera = GameObject.Find("PlayerCamera").GetComponent<Camera>();
        cameraOffset = transform.position - camera.transform.position;
        cameraOffset.z = -10.16469f;
        platformRender = transform.parent.gameObject.GetComponent<PreviewVegetable>();
        vegetableText = GameObject.Find("VegetableText").GetComponent<Text>();
        levelData = GameObject.Find("LevelData").GetComponent<LevelData>();
        setUpVegetables();

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // ----------- Move left and right -----------

        float move = Input.GetAxis("Horizontal");
        if (move != 0)
        {
            camera.transform.position = transform.position + cameraOffset; // Camera.main.ScreenToWorldPoint(transform.position);
        }

        onGround = isOnGround();
        onBanana = isOnBanana();

        // ----------- Timers for impulse speed on banana and tomato -----------
        if (leftBanana)
        {
            bananaTimer -= Time.deltaTime;

            if (bananaTimer < 0)
            {
                leftBanana = false;
                bananaTimer = 0.3f;
            }
        }

        if (hitTomato)
        {
            tomatoTimer -= Time.deltaTime;

            if (tomatoTimer < 0)
            {
                hitTomato = false;
                tomatoTimer = 0.2f;
            }
        }

        // ----------- Delay movement depending if on banana or tomato -----------
        if (onBanana || leftBanana)
        {
            float targetVelocityX = move * maxSpeed;
            float temp = Mathf.SmoothDamp(rigi.velocity.x, targetVelocityX, ref velocityXSmoothing, 0.8f);
            rigi.velocity = new Vector2(temp, rigi.velocity.y);
        }
        else if (hitTomato)
        {
            float targetVelocityX = move * maxSpeed;
            float temp = Mathf.SmoothDamp(rigi.velocity.x, targetVelocityX, ref velocityXSmoothing, 0.8f);
            rigi.velocity = new Vector2(temp, rigi.velocity.y);
        }
        else
            rigi.velocity = new Vector2(move * maxSpeed, rigi.velocity.y);

        anim.SetFloat("Speed", Mathf.Abs(move));

        // ----------- Flip the sprite depending on which way moving -----------
        if (move > 0 && !facingRight)
            Flip();
        else if (move < 0 && facingRight)
            Flip();
    }

    void Update()
    {

        #region Jump
        if (Input.GetKeyDown(KeyCode.Space)) //Jump
        {
            if (onGround)
            {
                //
                rigi.velocity = new Vector2(rigi.velocity.x, jumpVelocity);
                onGround = false;
            }
        }
        #endregion

        // ----------- Place vegetable if platform is visible at mouse position -----------
        #region Left Mouse
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (!showPlatform)
            {
                //showPlatform = true;
                //platformRender.draw = true;
            }
            else
            {
                Vector3 mousePos = Input.mousePosition;
                mousePos.z = 2.0f;
                Vector3 objectPosition = Camera.main.ScreenToWorldPoint(mousePos);

                if (vegetableCount[vegetableIndex] > 0 && !platformRender.colliding)
                {
                    vegetableCount[vegetableIndex]--;
                    changeVegetableCountUI();
                    vegetable[vegetableIndex] = Instantiate(vegetable[vegetableIndex], objectPosition, Quaternion.identity);
                    DrawVegetables();
                }
            }

        }
        #endregion

        // ----------- If vegetable visible, hide it with right click. -----------
        #region Right Mouse
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            showPlatform = false;
            platformRender.HideImage();
        }
        #endregion

        #region NumberClicks
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            vegetableIndex = 0;
            DrawVegetables();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            vegetableIndex = 1;
            DrawVegetables();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            vegetableIndex = 2;
            DrawVegetables();
        }
        #endregion
    }

    public void setUpVegetables()
    {
        vegetableCount = new int[3];
        vegetableCount[0] = levelData.carrots;
        vegetableCount[1] = levelData.tomatos;
        vegetableCount[2] = levelData.bananas;
    }

    public void GetVegetables(int carrots, int tomatos, int bananas)
    {
        vegetableCount[0] = carrots;
        vegetableCount[1] = tomatos;
        vegetableCount[2] = bananas;
    }


    /// <summary>
    /// ----------- Check if on ground -----------
    /// </summary>
    private bool isOnGround()
    {
        RaycastHit2D hitCenter = Physics2D.Raycast(transform.position, Vector2.down, 1.2f, ground);
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position - new Vector3(.3f, 0, 0), Vector2.down, 1.2f, ground);
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position + new Vector3(.3f, 0, 0), Vector2.down, 1.2f, ground);

        if (hitCenter.collider != null || hitLeft.collider != null || hitRight.collider != null)
        {
            return true;
        }

        return false;
    }
    /// <summary>
    /// ----------- Check if on banana. Could be merged into isOnGround() -----------
    /// </summary>
    private bool isOnBanana()
    {
        RaycastHit2D hitCenter = Physics2D.Raycast(transform.position, Vector2.down, 1.5f, banana);
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position - new Vector3(.3f, 0, 0), Vector2.down, 1.5f, banana);
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position + new Vector3(.3f, 0, 0), Vector2.down, 1.5f, banana);

        if (hitCenter.collider != null || hitLeft.collider != null || hitRight.collider != null)
        {
            return true;
        }
        else
        {
            if (onBanana)
                leftBanana = true;
            return false;
        }
    }

    // ----------- Flip sprite -----------
    void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    /// <summary>
    /// Draws the preview image of the vegetable you want to place on the screen.
    /// </summary>
    void DrawVegetables()
    {
        showPlatform = true;
        bool unavailable = false;

        if (vegetableCount[vegetableIndex] == 0)
            unavailable = true;

        platformRender.DrawImage(vegetableIndex, unavailable);
    }


    /// <summary>
    /// Changes the text on the UI for how many vegetables are available
    /// </summary>
    private void changeVegetableCountUI()
    {
        vegetableText.text = vegetableText.text = "Carrots: " + vegetableCount[0] + "   Tomatos: " + vegetableCount[1] + "   Bananas: " + vegetableCount[2];
    }
}
