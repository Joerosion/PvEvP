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
    public ContactFilter2D playerFilter;
    public float aggroPercentOfDeltaForSwitch = .5f;
    public float aggroAttentionTime = 2f;
    public float enemyMovementOffset = .1f;
    public float attackRangeHeight = 1f;
    public float attackRangeWidth = 1f;
    public float attackRangeHeightOffest = .5f;
    public bool isAttacking = false;
    public float attackDelay = 1;
    public float attackWait = 1;


    private Animator animator;
    private int isWalkingHash;
    private int toAttackHash;
    private SpriteRenderer spriteRenderer;



    private int direction = 1;
    private float currentSpeed;
    private Vector3 spawnLocation;
    private Vector3 targetLocation;
    private Vector3 currentLocation;
    private bool isWait = false;
    private float waitTime = 0;
    private float tempSpeed;
    private bool isWandering = true;
    private Vector2 aggroRangeBottomLeft;
    private Vector2 aggroRangeTopRight;
    private Vector2 attackRangeTopLeft;
    private Vector2 attackRangeBottomRight;
    private Collider2D[] playersInAggroRadius;
    private GameObject currentPlayerTarget;
    private GameObject closestPLayerInAggroRange;
    private float currentPlayerTargetTimeLeftAggroRange;
    private bool inMotion = false;
    private float currentWanderTarget;
    private Collider2D hitBox;
    private Collider2D[] hitPlayers;
    private float currentAttackDelay = 0;
    private bool isAttackDelay = false;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking");
        toAttackHash = Animator.StringToHash("toAttack");
        spriteRenderer = GetComponent<SpriteRenderer>();
        spawnLocation = transform.parent.transform.position;
        currentLocation = transform.position;
        targetLocation = currentLocation;
        playersInAggroRadius = new Collider2D[6];
        hitPlayers = new Collider2D[6];
        hitBox = transform.Find("HitBox").gameObject.GetComponent<CapsuleCollider2D>();
        currentSpeed = speed;
        currentAttackDelay = attackDelay;
    }

    private void Update()
    {
        currentLocation = transform.position;
        DetectPlayersInAggroRange();


        //Finds and picks a player target in aggroRange
        if (playersInAggroRadius[0] != null)
        {
            if (currentPlayerTarget == null)
            {
                currentPlayerTarget = closestPLayerInAggroRange;
            }
            else if (closestPLayerInAggroRange != null)
            {
                //Picks clostest player only if closer to skeleton by % set
                if (Mathf.Abs(currentPlayerTarget.transform.position.x - transform.position.x) / Mathf.Abs(closestPLayerInAggroRange.transform.position.x - transform.position.x) <= aggroPercentOfDeltaForSwitch)
                {
                    currentPlayerTarget = closestPLayerInAggroRange;
                }
            }
            //everyframe there is a player target in aggro Range find time and set wandering to false
            currentPlayerTargetTimeLeftAggroRange = Time.time;
            isWandering = false;
            currentWanderTarget = 0;
        }
        //this is where he will wait for the aggroAttentionTime before he starts wandering
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

        if (isWait == false && isAttacking == false)
        {
            Direction();
        }

        //Stop waiting test
        if (isWait == true)
        {
            Wait(0f, 0f);
        }


        Attack();


        //Movement enacted
        if (Mathf.Abs (targetLocation.x - transform.position.x) > spriteRenderer.bounds.extents.x + enemyMovementOffset && isAttacking == false)
        {
            Vector3 currentposition = transform.position;
            currentposition.x += direction * currentSpeed * Time.deltaTime;
            transform.position = currentposition;
           
            if(isWait == false)
            {
                inMotion = true;
            }
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
            Quaternion currentRotation = transform.rotation;
            currentRotation.y = 0;
            transform.rotation = currentRotation;

        }
        
        if (direction < 0)
        {
            Quaternion currentRotation = transform.rotation;
            currentRotation.y = 180;
            transform.rotation = currentRotation;
        }
    }

    void WanderTargetLocation()
    {
        float deltaTargetLocation = Mathf.Abs(targetLocation.x - currentLocation.x);

        if(currentWanderTarget == 0f)
        {
            currentWanderTarget = spawnLocation.x + Random.Range(-wanderRange, wanderRange);
        }

        if(deltaTargetLocation <= spriteRenderer.bounds.extents.x + enemyMovementOffset)
        {
            Wait(wanderWaitMin, wanderWaitMax);

            currentWanderTarget = spawnLocation.x + Random.Range(-wanderRange, wanderRange);
        }

        targetLocation = new Vector3(currentWanderTarget, transform.position.y, 0f);
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
        }
        if (Time.time <= waitTime)
        {
            currentSpeed = 0;
            isWait = true;
            inMotion = false;
            return;
        }
        else
        {
            currentSpeed = speed;
            isWait = false;
            inMotion = true;
        }

    }

    void DetectPlayersInAggroRange()
    {
        aggroRangeBottomLeft = new Vector2 (transform.position.x - aggroRangeWidth, transform.position.y + aggroRangeHeightOffest);
        aggroRangeTopRight = new Vector2(transform.position.x + aggroRangeWidth, transform.position.y + aggroRangeHeightOffest + aggroRangeHeight);
        float closestPLayerDistance;

        //Collects a collider of players in the aggroRange if no players in range collider length is 0
        Collider2D[] detectedPlayers = Physics2D.OverlapAreaAll(aggroRangeBottomLeft, aggroRangeTopRight, playerLayer);

        closestPLayerInAggroRange = null;

        //resets the array to not double place targets if in range for more then one frame
        playersInAggroRadius = new Collider2D[playersInAggroRadius.Length];
       
        if(detectedPlayers.Length > 0)
        {
            if (isWandering)
            {
                closestPLayerDistance = aggroRangeWidth *2;
            }
            else
            {
                closestPLayerDistance = Mathf.Abs(targetLocation.x - transform.position.x);
            }

            //Debug.Log(detectedPlayers[0].name);
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

    void Attack()
    {

        attackRangeTopLeft = new Vector2(transform.position.x, transform.position.y + attackRangeHeight + attackRangeHeightOffest);
        attackRangeBottomRight = new Vector2(transform.position.x + (attackRangeWidth * direction), transform.position.y - attackRangeHeight + attackRangeHeightOffest);

        //creates an array of all players within the aggroRange and starts the attackDelay
        if (Physics2D.OverlapAreaAll(attackRangeTopLeft, attackRangeBottomRight, playerLayer).Length > 0 && isAttacking == false)
        {
            isAttackDelay = true;
        }
        if (isAttackDelay == true)
        {
            //this counts down the delay and forces the skeleton to wait for a frame
            currentAttackDelay -= Time.deltaTime;

            Wait(Time.deltaTime, Time.deltaTime);

            //When the Delay is over start the attack animation
            if (currentAttackDelay <= 0)
            {
                //sets the Trigger that starts the animation, the animation activates a bool isAttacking and enables/disables the collider hitBox
                animator.SetTrigger(toAttackHash);

                currentAttackDelay = attackDelay;

                //reset his attacking pahse
                isAttackDelay = false;

                //sets a wait period after the skeleton attacks only applies if there is no target in attackRange becuase wait only stops movement and direction change
                Wait(attackWait, attackWait);

            }
        }
       

        //find what players are in the hitBox collider while it is enabled
        float howManyPlayersHit = Physics2D.OverlapCollider(hitBox, playerFilter, hitPlayers);

        //this is where we would do whatever we will do to kill the player, currently can happen more then once per attack
        if (howManyPlayersHit > 0)
        {
            for (int i = 0; i < howManyPlayersHit; i++)
            {
                Debug.Log(hitPlayers[i].name + " was Hit!");
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

        // Display attackRangeArea
        Vector2 attackRangeBottomLeft = new Vector2(transform.position.x, transform.position.y - attackRangeHeight + attackRangeHeightOffest);
        Vector2 attackRangeTopRight = new Vector2(transform.position.x + (attackRangeWidth * direction), transform.position.y + attackRangeHeight + attackRangeHeightOffest);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(attackRangeBottomLeft, attackRangeTopLeft);
        Gizmos.DrawLine(attackRangeTopLeft, attackRangeTopRight);
        Gizmos.DrawLine(attackRangeTopRight, attackRangeBottomRight);
        Gizmos.DrawLine(attackRangeBottomRight, attackRangeBottomLeft);


    }

}
