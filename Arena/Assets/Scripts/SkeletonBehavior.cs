using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonBehavior : MonoBehaviour
{

    public float speed = 3f;
    public float wanderRange = 5f;
    public float wanderWaitTime = 1f;

    [SerializeField]
    private int direction = 1;

    private Animator animator;
    private int isWalkingHash;
    private SpriteRenderer spriteRenderer;
    private Vector3 spawnLocation;
    public Vector3 targetLocation;
    public Vector3 currentLocation;
    private bool isWait = false;
    private float waitTime = 0;
    private float tempSpeed;
        
    private void Awake()
    {
        animator = GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking");
        spriteRenderer = GetComponent<SpriteRenderer>();
        spawnLocation = transform.parent.transform.position;
        currentLocation = transform.position;
        targetLocation = currentLocation;
        
    }

    private void Update()
    {
        currentLocation = transform.position;
        NewTargetLocation();
        if(isWait == false)
        {
            WanderDirection();

        }
        if (isWait == true)
        {
            Wait();
        }

        //Movement enacted
        Vector3 currentposition = transform.position;
        currentposition.x += direction * speed * Time.deltaTime;
        transform.position = currentposition;


        //Animations
        if(speed != 0)
        {
            animator.SetBool(isWalkingHash, true);
        }
        else
        {
            animator.SetBool(isWalkingHash, false);
        }

        if (direction > 0)
        {
            spriteRenderer.flipX = false;
        }
        
        if (direction < 0)
        {
            spriteRenderer.flipX = true;
        }
    }

    void NewTargetLocation()
    {
        float targetDeadzone = .1f;
        float deltaTargetLocation = Mathf.Abs(targetLocation.x - currentLocation.x);

        if(deltaTargetLocation < targetDeadzone)
        {
            isWait = true;
            waitTime = Time.time + Random.Range(0, wanderWaitTime);
            tempSpeed = speed;
            Debug.Log("Waiting...");


            targetLocation.x = spawnLocation.x + Random.Range(-wanderRange, wanderRange);
            Debug.Log("Switch Direction");
        }
    }

    void WanderDirection()
    {
        if (targetLocation.x - currentLocation.x > 0)
        {
            direction = 1;
        }
        else if(targetLocation.x -currentLocation.x < 0)
        {
            direction = -1;
        }
    }


    void Wait()
    {
        

        if (Time.time < waitTime)
        {
            speed = 0;
            return;
        }
        else
        {
            speed = tempSpeed;
            isWait = false;
            Debug.Log("Done Waiting.");
        }
    }


    void OnDrawGizmosSelected()
    {
        // Display WanderTargetLocation
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(targetLocation, .1f);
    }

}
