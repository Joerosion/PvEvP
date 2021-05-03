using System.Collections.Generic;
using Frictionless;
using UnityEngine;

//PlayerDataService
//Functions:
//SpawnPlayer
//PlayerDeath
//AddGold
//RemoveGold
//AssignRole
//ApplyArmor
//ApplyBuff
//ProcessPlayerAttack

public class PlayerDataService
{
    private PlayerData _player;
    private List<PlayerData> _Players = new List<PlayerData>();

    private MessageRouter _messageRouter;
    private EntityService _entityService;

    public PlayerDataService()
    {
        _messageRouter = ServiceFactory.Instance.GetService<MessageRouter>();
        _entityService = ServiceFactory.Instance.GetService<EntityService>();
    }

    public void SpawnPlayer()
    {
        //Calling on enityService to create a new ID
        int entityId = _entityService.GetNewEntityId();

        _player = new PlayerData();
        _player.EntityId = entityId;
        SetInitialPlayerValues(_player);
        _Players.Add(_player);

        var spawnMessage = new SpawnPlayerMessage();
        spawnMessage.EntityId = entityId;
        _messageRouter.RaiseMessage(spawnMessage);
    }

    private void SetInitialPlayerValues(PlayerData player)
    {
        player.GoldAmount = 0;
        player.AttackDamage = 5;  //not yet set to default value defintion per role.  Temporary!!!
    }

    public int AddGold(int goldId, int playerId)
    {
        var goldService = ServiceFactory.Instance.GetService<GoldService>();
        var goldAmount = goldService.GetGoldAmount(goldId);
        for(int i=0; i < _Players.Count; i++)
        {
            if (_Players[i].EntityId == playerId)
            {
                _Players[i].GoldAmount += goldAmount;
            }
        }
        goldService.DestroyGold(goldId);
        return goldAmount;
    }

    public int GetPlayerDamage(int playerId)
    {
        foreach (PlayerData playerData in _Players)
        {
            if(playerData.EntityId == playerId)
            {
                return playerData.AttackDamage;
            }
        }


        Debug.LogError("No matching Player Found via GetPlayerDamage");
        return 0;
    }
}
