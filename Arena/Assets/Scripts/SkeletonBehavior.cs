using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * 
 * this script finds a target location for this enemy, either to wander around is spawn point or to go to the closest player
 * enactes this movement
 * and draws the gizmos for these thing when selecting this enemy
 * 
 */




public class SkeletonBehavior : MonoBehaviour
{

    public float speed = 3f;
    public float wanderRange = 5f;
    public float wanderWaitMin = 0f;
    public float wanderWaitMax = 1f;
    public float aggroRangeHeight = 1f;
    public float aggroRangeWidth = 5f;
    public float aggroRangeHeightOffest = -.3f;
    public LayerMask playerLayer;
    public float aggroPercentOfDeltaForSwitch = .5f;
    public float aggroAttentionTime = 2f;
    public float enemyMovementOffset = .1f;

    private Animator animator;
    private int isWalkingHash;
    private SpriteRenderer spriteRenderer;



    private int direction = 1;
    private Vector3 spawnLocation;
    private Vector3 targetLocation;
    private Vector3 currentLocation;
    private bool isWait = false;
    private float waitTime = 0;
    private float tempSpeed;
    private bool isWandering = true;
    private Vector2 aggroRangeBottomLeft;
    private Vector2 aggroRangeTopRight;
    private Collider2D[] playersInAggroRadius;
    private GameObject currentPlayerTarget;
    private GameObject closestPLayerInAggroRange;
    private float currentPlayerTargetTimeLeftAggroRange;
    private bool inMotion = true;
        
    private void Awake()
    {
        animator = GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking");
        spriteRenderer = GetComponent<SpriteRenderer>();
        spawnLocation = transform.parent.transform.position;
        currentLocation = transform.position;
        targetLocation = currentLocation;
        playersInAggroRadius = new Collider2D[6];
        
    }

    private void Update()
    {
        currentLocation = transform.position;
        DetectPlayersInAggroRange();

        if (playersInAggroRadius[0] != null)
        {
            if (currentPlayerTarget == null)
            {
                currentPlayerTarget = closestPLayerInAggroRange;
            }
            else
            {
                if (Mathf.Abs(currentPlayerTarget.transform.position.x - transform.position.x) / Mathf.Abs(closestPLayerInAggroRange.transform.position.x - transform.position.x) <= aggroPercentOfDeltaForSwitch)
                {
                    currentPlayerTarget = closestPLayerInAggroRange;
                }
            }
            //everyframe there is a player target in aggro Range find time
            currentPlayerTargetTimeLeftAggroRange = Time.time;
            isWandering = false;
        }

        if(Time.time > currentPlayerTargetTimeLeftAggroRange + aggroAttentionTime)
        {
            currentPlayerTarget = null;
            isWandering = true;
        }
        else if (currentPlayerTarget != null)
        {
            targetLocation = currentPlayerTarget.GetComponent<Transform>().position;
        }

        if (isWandering == true)
        {
            WanderTargetLocation();
        }

        if (isWait == false)
        {
            Direction();
        }

        //Stop waiting test
        if (isWait == true)
        {
            Wait(0f, 0f);
        }

        //Movement enacted
        if (Mathf.Abs (targetLocation.x - transform.position.x) > spriteRenderer.bounds.extents.x + enemyMovementOffset)
        {
        Vector3 currentposition = transform.position;
        currentposition.x += direction * speed * Time.deltaTime;
        transform.position = currentposition;

            inMotion = true;
        }
        else
        {
            inMotion = false;
        }

        //Animations
        if(inMotion == true)
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

    void WanderTargetLocation()
    {
        float deltaTargetLocation = Mathf.Abs(targetLocation.x - currentLocation.x);

        if(deltaTargetLocation <= spriteRenderer.bounds.extents.x + enemyMovementOffset)
        {
            Wait(wanderWaitMin, wanderWaitMax);

            targetLocation.x = spawnLocation.x + Random.Range(-wanderRange, wanderRange);
            Debug.Log("Switch Direction");
        }
    }

    void Direction()
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


    void Wait(float minWaitTime, float maxWaitTime)
    {

        if (isWait == false)
        {
            waitTime = Time.time + Random.Range(minWaitTime, maxWaitTime);
            tempSpeed = speed;
            Debug.Log("Start waiting...");
        }
        if (Time.time <= waitTime)
        {
            speed = 0;
            isWait = true;
            inMotion = false;
            return;
        }
        else
        {
            speed = tempSpeed;
            isWait = false;
            inMotion = true;
            Debug.Log("Done waiting...");
        }

    }

    void DetectPlayersInAggroRange()
    {
        aggroRangeBottomLeft = new Vector2 (transform.position.x - aggroRangeWidth, transform.position.y + aggroRangeHeightOffest);
        aggroRangeTopRight = new Vector2(transform.position.x + aggroRangeWidth, transform.position.y + aggroRangeHeightOffest + aggroRangeHeight);

        Collider2D[] detectedPlayers = Physics2D.OverlapAreaAll(aggroRangeBottomLeft, aggroRangeTopRight, playerLayer);

        closestPLayerInAggroRange = null;

        playersInAggroRadius = new Collider2D[playersInAggroRadius.Length];
       
        if(detectedPlayers.Length > 0)
        {
            float closestPLayerDistance = aggroRangeWidth;

           Debug.Log(detectedPlayers[0].name);
           for (int i =0; i < detectedPlayers.Length; i++)
            {
                playersInAggroRadius[i] = detectedPlayers[i];

                if(Mathf.Abs(detectedPlayers[i].transform.position.x - transform.position.x) < closestPLayerDistance)
                {
                    closestPLayerDistance = Mathf.Abs(detectedPlayers[i].transform.position.x - transform.position.x);
                    closestPLayerInAggroRange = detectedPlayers[i].gameObject;
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // Display WanderTargetLocation
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(targetLocation, enemyMovementOffset);



        // Display aggroRangeArea
        Vector2 aggroRangeTopLeft = new Vector2 (transform.position.x - aggroRangeWidth, transform.position.y + aggroRangeHeightOffest + aggroRangeHeight);
        Vector2 aggroRangeBottomRight = new Vector2 (transform.position.x + aggroRangeWidth, transform.position.y + aggroRangeHeightOffest);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(aggroRangeBottomLeft, aggroRangeTopLeft);
        Gizmos.DrawLine(aggroRangeTopLeft, aggroRangeTopRight);
        Gizmos.DrawLine(aggroRangeTopRight, aggroRangeBottomRight);
        Gizmos.DrawLine(aggroRangeBottomRight, aggroRangeBottomLeft);


    }

}
