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

    public void UpdateAnimatorValues(float velX)
    {
        animator.SetFloat("Horizontal", Mathf.Abs(velX));

        //determine direction and flip sprite
        if (velX > 0)
        {
            spriteRenderer.flipX = false;
        }
        
        if (velX < 0)
        {
            spriteRenderer.flipX = true;
        }
    }
}
