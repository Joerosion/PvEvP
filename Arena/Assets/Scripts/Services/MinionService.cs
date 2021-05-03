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
        List<MinionData> tempMinionList = _activeMinions;

        for (int i=0; i < message.enemyHitList.Count; i++)
        {
            foreach(MinionData minion in tempMinionList)
            {
                if (minion.EntityId == message.enemyHitList[i])
                {
                    minion.currentHP -= message.Damage;
                    if(minion.currentHP <= 0)
                    {
                        DestroyMinion(minion.EntityId);
                    }
                }
            }
        }
    }
}
