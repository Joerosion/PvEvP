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
    private Player _player;
    
    public void SpawnPlayer()
    {
        _player = new Player();
        
        int instanceId = ServiceFactory.Instance.GetService<EntityService>().GetNewEntityId();
        var messageRouter = ServiceFactory.Instance.GetService<MessageRouter>();

        var spawnMessage = new SpawnPlayerMessage();
        spawnMessage.EntityId = instanceId;
        messageRouter.RaiseMessage(spawnMessage);
    }

    public void AddGold(int goldId, int playerId)
    {
        var goldService = ServiceFactory.Instance.GetService<GoldService>();
        var goldAmount = goldService.GetGoldAmount(goldId);
        _player.GoldAmount += goldAmount;
        goldService.DestroyGold(goldId);
    }
}

public class Player
{
    public int GoldAmount { get; set; }
}

public class SpawnPlayerMessage
{
    public int EntityId { get; set; }
}
