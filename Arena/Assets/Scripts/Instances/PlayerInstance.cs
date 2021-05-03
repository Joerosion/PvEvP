using Frictionless;
using UnityEngine;
using System.Collections.Generic;

public class PlayerInstance : EntityInstance
{
    public bool isAlive;
    public int health = 1;
    public float attackDamage;
    public int currentGold;
    public PlayerRoles currentRole;
    public ContactFilter2D eligibleTargets;

    private PlayerDataService _playerDataService;
    private MinionService _enemyService;
    private MessageRouter _messageRouter;
    private CapsuleCollider2D swingHitBox;
    private List<int> _entityIDsHit;

    private void Start()
    {
        _playerDataService = ServiceFactory.Instance.GetService<PlayerDataService>();
        _enemyService = ServiceFactory.Instance.GetService<MinionService>();
        _messageRouter = ServiceFactory.Instance.GetService<MessageRouter>();
        swingHitBox = GetComponent<CapsuleCollider2D>();
        _entityIDsHit = new List<int>();
    }

    public void OnChangeRole(PlayerRoles role, int entityID)
    {
        if(EntityId != entityID)
        {
            return;
        }
        
        //do a check to make sure you arent already that role

        currentRole = role;
    }

    public void processPlayerAttack()
    {
        //Check to see if we have a capsule collider
        if (swingHitBox.enabled == true)
        {
            //Create a temporary list, put all the colliders we touch with our swing in it. 
            List<Collider2D> tempHitList = new List<Collider2D>();

            int numTargetsHit = Physics2D.OverlapCollider(swingHitBox, eligibleTargets, tempHitList);

            if (numTargetsHit > 0)
            {
                foreach (Collider2D col in tempHitList)
                {
                    int entityIdHit = col.gameObject.GetComponent<EntityInstance>().EntityId;

                    if (!_entityIDsHit.Contains(entityIdHit))
                    {
                        _entityIDsHit.Add(entityIdHit);
                        reportPlayerAttackHit(EntityId, entityIdHit);
                    }
                }
            }

        }
    }

    public void reportPlayerAttackHit(int playerId, int minionId)
    {
        var message = new PlayerAttackMessage();
        message.playerId = EntityId;
        message.minionIdHit = minionId;
        message.Damage = _playerDataService.GetPlayerDamage(EntityId);
        _messageRouter.RaiseMessage(message);
    }

    public void clearPlayerAttackList()
    {
        _entityIDsHit.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("We've touched a trigger... ");

        // The master function for the player colliding with another object
        if (other.gameObject.layer == LayerMask.NameToLayer("Gold"))
        {
            Debug.Log("...and that trigger is gold!");
            var goldId = other.gameObject.GetComponent<EntityInstance>().EntityId;
            currentGold += _playerDataService.AddGold(goldId, EntityId);
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Minion"))
        {
            //Debug.Log("...and that trigger is a minion!");
            //var minionId = other.gameObject.GetComponent<EntityInstance>().EntityId;
            //_enemyService.DestroyEnemy(minionId);
        }
        else
        {
            
        }
    }
}
