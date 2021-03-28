/////////////////////////////////////////////////////////////////////
///// Simple 2D Platform Controller for Unity
///// By David McQuillan
///// 23/11/2014
/////////////////////////////////////////////////////////////////////

using System.Collections;
/*  
 * This is a basic, but powerful barebones 2D platform controller for Unity
 *  It allows you to seperate Walls, Roofs and Platforms, and allows the dropping down through said platforms
 *  It does this using Layers and Raycasts. This does not rely on Unity's 2D Physics engine and does not require rigidbodies
 *  All level geometry need 2D coliders on them however. These can be any of the 2D colliders, including Edge Colliders.
 *  Edge Colliders are recommended for Platforms that you intend to drop down from.
*/
using UnityEngine;
using UnityEngine.InputSystem;

public class Simple2dPlatformController : MonoBehaviour
{
    private bool wasJumpPressed;
    private bool platformDropDown = false;
    private PlayerControls playerControls;
    private PlayerAnimationHandler playerAnimationHandler;
    private bool jumpHeld;
    private float timeLeftGround;
    float horizontalMovementInput = 0f;
    float verticalMovement = 0f;

    #region Public Variables, hidden from inspector
    /// <summary>
    /// Other scripts might want to access these at some point
    /// </summary>
    //[HideInInspector]
    public bool isGrounded = false;
    public bool wallSliding = false;

    //[HideInInspector]
    public Vector3 Velocity;
    #endregion

    #region Animator Variables

    #endregion

    #region Public Inspector Variables
    public bool DigitalKeyboardControls = false;
    public float VelocityClamp = 0.05f;
    public float HorizontalDeadZone = 0.1f;
    public float VerticalDeadZone = 0.3f;
    public float Gravity = -20f;
    public float JumpStrength = 10f;
    public float RunSpeed = 5f;
    public bool UseAcceleration = false;
    public float Acceleration = 10f;
    public float Deceleration = 10f;
    public float SkidAcceleration = 20f;
    public bool UseSkidAcceleration = false;
    public float MaximumHorizontalSpeed = 10f;
    public float MaximumVerticalSpeed = 8f;
    public float MaximumWallSlideSpeed = 3f;
    public float platformDropDownCoolDown = 0.2f;
    public float FallMultiplier = 2.5f;
    public float LowJumpMultipler = 2f;
    public float jumpGracePeriod = 0.2f;

    [Header("Collider Specs")]
    public float ColliderSizeVertical = 0.3f;
    public float ColliderSizeHorizontal = 0.15f;
    public float ColliderOffsetVertical = 0.0f;
    public float ColliderOffsetHorizontal = 0.0f;
    [Space(10)]

    /// <summary>
    /// At minimum you want 2 layers. One for your player and one for your geometry.
    /// To get the most out of this you want 4 layers. One for Walls, One for Roofs, One for Platforms and one for the player.
    /// </summary>
    [Header("Layer Assignments")]
    public LayerMask Walls;
    public LayerMask Roofs;
    public LayerMask Platforms;
    #endregion

    /// <summary>
    /// Show a cube in the scene view showing the bounds of the collider.
    /// </summary>
    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(new Vector3(this.transform.position.x + ColliderOffsetHorizontal, this.transform.position.y + ColliderOffsetVertical, 0f),
                            new Vector3(ColliderSizeHorizontal * 2, ColliderSizeVertical * 2, 0f));
    }

    private void Awake()
    {
        playerControls = new PlayerControls();
        playerAnimationHandler = GetComponent<PlayerAnimationHandler>();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    void Update()
    {
        //Player Input (Old Code)
        //var horizontalMovement = DigitalKeyboardControls ? Input.GetAxisRaw("Horizontal") : Input.GetAxis("Horizontal");
        //var verticalMovement = DigitalKeyboardControls ? Input.GetAxisRaw("Vertical") : Input.GetAxis("Vertical");

        //Player Input

        horizontalMovementInput = 0f;
        verticalMovement = 0f;

        if(Mathf.Abs(playerControls.InGame.Horizontal.ReadValue<float>()) > HorizontalDeadZone)
        {
            horizontalMovementInput = playerControls.InGame.Horizontal.ReadValue<float>();
        }
        else
        {
            horizontalMovementInput = 0f;
        }

        if (Mathf.Abs(playerControls.InGame.Vertical.ReadValue<float>()) > VerticalDeadZone)
        {
            verticalMovement = playerControls.InGame.Vertical.ReadValue<float>();
        }
        else
        {
            verticalMovement = 0f;
        }

        var canJump = GetCanJump();

        //Simple Jump, ignored if you're pressing the DOWN key
        if (verticalMovement >= 0 && (isGrounded || Time.time < timeLeftGround + jumpGracePeriod) && canJump)
        {
            isGrounded = false;
            playerAnimationHandler.TriggerJump();
            Velocity.y = JumpStrength;
        }

        //Drop down from a platform, using DOWN and Jump
        if (!platformDropDown && verticalMovement < 0 && isGrounded && canJump)
        {
            StartCoroutine(DropDown()); //We use a co-routine for this (explained below)
        }

        //Simple Horizontal Movement (Same in the air as on the ground)
        if (horizontalMovementInput != 0)
        {
            if (Velocity.x == 0 || Mathf.Sign(horizontalMovementInput) == Mathf.Sign(Velocity.x))
            {

                //Added a Custom Toggle here for Velocity
                if (UseAcceleration == true)
                {
                    Velocity.x += horizontalMovementInput * Acceleration * RunSpeed * Time.deltaTime;
                }
                else
                {
                    Velocity.x = horizontalMovementInput * RunSpeed;
                }

            }
            else //If we're moving in the opposite direction, skid.
            {

                //Added a Custom Toggle here for Skid Velocity.
                if (UseSkidAcceleration == true)
                {
                    Velocity.x += horizontalMovementInput * SkidAcceleration * RunSpeed * Time.deltaTime;
                }
                else
                {
                    Velocity.x = horizontalMovementInput * RunSpeed;
                }

            }
        }
        else if (Velocity.x != 0)
        {
            //Velocity.x -= Mathf.Sign(Velocity.x) * Deceleration * Time.deltaTime;
            Velocity.x -= Velocity.x * Deceleration * Time.deltaTime;
            Velocity.x = Velocity.x < VelocityClamp ? (Velocity.x > -VelocityClamp ? 0 : Velocity.x) : Velocity.x; //set to 0 if it's close enough
        }

        //Send horizontal movement to the animation handler.
        //Eventually, we want to expand this functionality to send many inputs to animation handler.

        //Clamp to maximum
        Velocity.x = Mathf.Clamp(Velocity.x, -MaximumHorizontalSpeed, MaximumHorizontalSpeed);
        Velocity.y = Mathf.Clamp(Velocity.y, -MaximumVerticalSpeed, MaximumVerticalSpeed);
        if (wallSliding)
        {
            Velocity.y = Mathf.Clamp(Velocity.y, -MaximumWallSlideSpeed, MaximumWallSlideSpeed);
        }
    }

    /// <summary>
    /// Smooth Jump Button detection
    /// </summary>
    /// <returns>Whether or not the jump button is pressed AND you can jump</returns>
    private bool GetCanJump()
    {
        //Input.GetButtonDown tends to be quite 'sticky' and sometimes doesn't fire. 
        //This is a smoother way of doing things

        //Old Input System
        //var jumpButtonDown = Input.GetButton("Jump");

        bool jumpButtonDown = playerControls.InGame.Jump.triggered;

        //If we have previously pressed the jump button and the jump button has been released
        if (wasJumpPressed && !jumpButtonDown)
        {
            wasJumpPressed = false; //Re-enable jumping
        }
        if ((isGrounded || Time.time < timeLeftGround + jumpGracePeriod) && jumpButtonDown && !wasJumpPressed)
        {
            wasJumpPressed = true; //Disable jumping
            return true; //tell the parent that we've jumped
        }

        return false; //Can't Jump, Won't Jump

    }

    /// <summary>
    /// Drop Down from certain platforms. Call as CoRoutine
    /// </summary>
    /// <returns></returns>
    private IEnumerator DropDown()
    {
        //A Naive WaitForSeconds is used here. You could, perhaps, get the current ground and just remove that from the calculations if you wanted to be smart about it.
        platformDropDown = true; //Activate the dropdown flag
        isGrounded = false; //tell the engine we're not grounded any more
        yield return new WaitForSeconds(platformDropDownCoolDown); //wait x seconds (so that we don't just pop back up onto the platform we're on)
        platformDropDown = false;  //Deactivate the dropdown flag

    }

    /// <summary>
    /// Our physics loop
    /// </summary>
    void FixedUpdate()
    {
        //Add gravity if we're off the ground
        if (!isGrounded)
        {
            if(Velocity.y < 0) //Adopted code from the "Better Jumps" philosophy. Right now, we start applying stronger gravity when the player releases the jump button.
            {
                if (wallSliding == false)
                {
                    Velocity += Vector3.up * Gravity * (FallMultiplier) * Time.deltaTime;
                }
                else
                {
                    Velocity.y += Gravity * Time.deltaTime * .3f; // This is using a temporary, hard-coded variable for wallSlide acceleration
                }
            }
            else if (Velocity.y > 0 && !InputSystem.GetDevice<Gamepad>().buttonSouth.isPressed) //Right now, we're checking the GamePad directly. This is not flexible. Let's change this at some point.
            {
                Velocity += Vector3.up * Gravity * (LowJumpMultipler) * Time.deltaTime;
            }
            else
            {
                Velocity.y += Gravity * Time.fixedDeltaTime;
            }
        }

        //Move the player
        this.transform.position += Velocity * Time.fixedDeltaTime;

        //Collision tests!
        UpTest();
        DownTest(!platformDropDown); //Down tests, pass in whether or not we want to do the platform drop-down tests or not
        WallTest();
        playerAnimationHandler.UpdateAnimatorValues(horizontalMovementInput, Velocity.y, wallSliding, isGrounded);
    }

    /// <summary>
    /// Used in Raycast shortcut
    /// </summary>
    RaycastHit2D lastHitResult = new RaycastHit2D();
    /// <summary>
    /// Raycast shortcut
    /// </summary>
    /// <param name="Direction"></param>
    /// <param name="mask"></param>
    /// <returns>True on hit, False on not hit</returns>
    bool RaycastVertical(Vector2 Direction, LayerMask mask)
    {
        lastHitResult = Physics2D.Raycast(new Vector2(this.transform.position.x, this.transform.position.y + ColliderOffsetVertical), 
                                                                                                Direction, ColliderSizeVertical, mask);
        if (lastHitResult != null && lastHitResult.collider != null) return true;
        return false;
    }


    bool RaycastHorizontal(Vector2 Direction, LayerMask mask)
    {
        lastHitResult = Physics2D.Raycast(new Vector2(this.transform.position.x + ColliderOffsetHorizontal, this.transform.position.y),
                                                                                              Direction, ColliderSizeHorizontal, mask);
        if (lastHitResult != null && lastHitResult.collider != null) return true;
        return false;
    }
    /// <summary>
    /// Test to see if we hit the ceiling
    /// </summary>
    void UpTest()
    {
        if (Velocity.y < 0) return; //Don't bother unless we're moving upwards
        if (RaycastVertical(this.transform.up, Roofs))
        {
            this.transform.position = new Vector3(this.transform.position.x, lastHitResult.point.y - ColliderSizeVertical, this.transform.position.z);
            Velocity.y = 0;
        }
    }
    /// <summary>
    /// Test to see if we hit the ground
    /// </summary>
    /// <param name="TestAllColliders">If false, ignores "Platforms" mask to allow dropping down</param>
    void DownTest(bool TestAllColliders = true)
    {
        //Only test if we're moving downwards, or not moving vertically at all
        if ((Velocity.y <= 0) && RaycastVertical(-this.transform.up, TestAllColliders ? (Platforms | Walls | Roofs) : (Walls | Roofs)))
        {
            this.transform.position = new Vector3(this.transform.position.x, lastHitResult.point.y + ColliderSizeVertical, this.transform.position.z);
            Velocity.y = 0;
            isGrounded = true;
            wallSliding = false;
        }
        else //otherwise we're not grounded
        {
            if(isGrounded == true)
            {
                timeLeftGround = Time.time;
            }

            isGrounded = false;
        }
    }
    /// <summary>
    /// Test to see if we hit a wall
    /// </summary>
    void WallTest()
    {
        if (Velocity.x < 0 && RaycastHorizontal(-this.transform.right, Walls)) //Only test Left if we're moving or holding Left
        {
            this.transform.position = new Vector3(lastHitResult.point.x + ColliderSizeHorizontal, this.transform.position.y, this.transform.position.z);
            Velocity.x = 0;

            if (horizontalMovementInput < 0 && isGrounded == false && Velocity.y < 0)
            {
                if (wallSliding == false)
                {
                    Velocity.y = 0f;
                }

                Debug.Log("We're wall sliding!");
                wallSliding = true;
            }
        }
        else if (Velocity.x > 0 && RaycastHorizontal(this.transform.right, Walls)) //Only test Right if we're moving or holding Right
        {
            this.transform.position = new Vector3(lastHitResult.point.x - ColliderSizeHorizontal, this.transform.position.y, this.transform.position.z);
            Velocity.x = 0;
            if (horizontalMovementInput > 0 && isGrounded == false && Velocity.y < 0)
            {
                //Velocity.y = 0f;
                wallSliding = true;
            }
        }
        else
        {
            wallSliding = false;
        }

    }
}