using Frictionless;
using UnityEngine;

public class PlayerInstance : EntityInstance
{
    public bool isAlive;
    public int health = 1;
    public float attackDamage;
    public int currentGold;
    public PlayerRoles currentRole;
    public int _entityID;

    private PlayerDataService _playerDataService;

    private void Start()
    {
        _playerDataService = ServiceFactory.Instance.GetService<PlayerDataService>();
    }

    public void OnChangeRole(PlayerRoles role, int entityID)
    {
        if(_entityID != entityID)
        {
            return;
        }
        
        //do a check to make sure you arent already that role

        currentRole = role;

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // The master function for the player colliding with another object
        if (other.gameObject.layer == LayerMask.NameToLayer("Gold"))
        {
            var goldId = other.gameObject.GetComponent<EntityInstance>().EntityId;
            _playerDataService.AddGold(goldId, EntityId);
        }
        else
        {
            
        }
    }
}