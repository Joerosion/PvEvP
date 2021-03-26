using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    GlobalVariables globalVariables;
    PlayerControls playerControls;
    SpriteRenderer spriteRenderer;

    [SerializeField] private LayerMask groundLayerMask;

    public float speed = 5.0f;
    private float horizonalDeadZone = 0.05f;
    private Animator animator;

    private void Awake()
    {
        globalVariables = new GlobalVariables();
        playerControls = new PlayerControls();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        float inputX = playerControls.InGame.Horizontal.ReadValue<float>();
        float velY = 0f;

        //Check to see if the stick is pressed in either horizontal direction by more than the deadzone.
        if (Mathf.Abs(inputX) > horizonalDeadZone)
        {
            //Send the stick value to the animator to begin run if applicable.
            animator.SetFloat("Horizontal", Mathf.Abs(inputX));

            //determine direction and flip sprite
            if(inputX > 0)
            {
                spriteRenderer.flipX = false;
            }
            else
            {
                spriteRenderer.flipX = true;
            }

            
        }
        else
        {
            animator.SetFloat("Horizontal", 0f);
            inputX = 0f;
        }

        if (!CheckIfGrounded())
        {
            //velY = -globalVariables.Gravity * Time.deltaTime;
        }

        Vector2 moveVector = new Vector2(inputX, velY);
        Move(moveVector);

    }

    void Move(Vector2 moveVector)
    {
        float velX = moveVector.x * speed * Time.deltaTime;

        transform.position += new Vector3(velX, moveVector.y, 0f);
    }

    private bool CheckIfGrounded()
    {
        Vector2 position = spriteRenderer.bounds.center;
        Vector2 direction = Vector2.down;
        float distance = spriteRenderer.bounds.extents.y;

        Debug.DrawLine(position, new Vector2(position.x, position.y - distance), Color.red);

        RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, groundLayerMask);

        if (hit.collider != null)
        {
            Debug.Log("We're grounded!");
            return true;
        }

        Debug.Log("We're NOT grounded!");

        return false;
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }
}
