using System.Collections.Generic;
using Frictionless;

/// <summary>
/// An example service which can be invoked from anywhere that
/// broadcasts message to spawn enemies
/// Also keeps track of the current ID of enemies
/// </summary>
public class MinionService
{
    private List<MinionData> _activeMinions = new List<MinionData>();
    private EntityService _entityService;
    private MessageRouter _messageRouter;

    public MinionService()
    {
        _messageRouter = ServiceFactory.Instance.GetService<MessageRouter>();
        _entityService = ServiceFactory.Instance.GetService<EntityService>();
        _messageRouter.AddListener<PlayerAttackMessage>(OnPlayerAttack);

    }

    public void SpawnMinion(MinionType minionType)
    {
        var currentId = _entityService.GetNewEntityId();
        var messageRouter = ServiceFactory.Instance.GetService<MessageRouter>();
        messageRouter.RaiseMessage(new SpawnMinionMessage(currentId, minionType));
        RegisterMinion(currentId, minionType);
    }

    public void DestroyMinion(int minionID)
    {
        for (int i = 0; i < _activeMinions.Count; ++i)
        {
            if (_activeMinions[i].EntityId == minionID)
            {
                var message = new DestroyMinionMessage();
                message.EntityId = _activeMinions[i].EntityId;
                _messageRouter.RaiseMessage(message);
                _activeMinions.Remove(_activeMinions[i]);
                break;
            }
        }
    }

    public void RegisterMinion(int minionID, MinionType minionType)
    {
        MinionData newMinion = new MinionData();
        newMinion.EntityId = minionID;
        newMinion.minionType = minionType;
        if (minionType == MinionType.Skeleton)
        {
            newMinion.maxHP = 5;
        }
        else if (minionType == MinionType.MushroomMan)
        {
            newMinion.maxHP = 10;
        }

        newMinion.SetInitialValues();

        _activeMinions.Add(newMinion);
    }

    private void OnPlayerAttack(PlayerAttackMessage message)
    {
        //Create a copy of the active minion list
        List<MinionData> tempMinionList = _activeMinions;

        //Check each minion in this temp list to see if it matches the ID from the player
        foreach(MinionData minion in tempMinionList)
        {
            if (minion.EntityId == message.minionIdHit)
            {
                //Reduce the minion's HP by the damage value on the message.
                minion.currentHP -= message.Damage;
                if (minion.currentHP <= 0)
                {
                    //If the minion is dead, destroy the minion with that ID.
                    DestroyMinion(minion.EntityId);
                }
            }
        }
    }
}
