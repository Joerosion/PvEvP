using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{

    private Simple2dPlatformController playerController;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<Simple2dPlatformController>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //float velX = playerController.Velocity.x;
        //float velY = 0f;

        ////Send the stick value to the animator to begin run if applicable.
        //animator.SetFloat("Horizontal", Mathf.Abs(velX));

        //    //determine direction and flip sprite
        //    if (velX > 0)
        //    {
        //        spriteRenderer.flipX = false;
        //    }
        //    else
        //    {
        //        spriteRenderer.flipX = true;
        //    }

    }

    public void TriggerJump()
    {
        animator.SetTrigger("jumpTriggered");
    }

    public void SetAttack(bool attacking)
    {
        animator.SetBool("attacking", attacking);
    }

    public void UpdateAnimatorValues(float inputX, float velY, bool isWallSliding, bool isGrounded)
    {
        animator.SetFloat("Horizontal", Mathf.Abs(inputX));
        animator.SetFloat("Vertical", velY);
        animator.SetBool("wallSliding", isWallSliding);
        animator.SetBool("isGrounded", isGrounded);

        //determine direction and flip sprite
        if (inputX > 0)
        {
            spriteRenderer.flipX = false;
            if(playerController.ColliderOffsetHorizontal > 0)
            {
                playerController.ColliderOffsetHorizontal *= -1f;
            }
        }
        
        if (inputX < 0)
        {
            spriteRenderer.flipX = true;
            if (playerController.ColliderOffsetHorizontal < 0)
            {
                playerController.ColliderOffsetHorizontal *= -1f;
            }
        }

    }
}
