using System.Collections.Generic;
using Frictionless;

/// <summary>
/// An example service which can be invoked from anywhere that
/// broadcasts message to spawn enemies
/// Also keeps track of the current ID of enemies
/// </summary>
public class EnemyService
{
    private List<MinionData> _activeMinions = new List<MinionData>();
    private EntityService _entityService;
    private MessageRouter _messageRouter;

    public EnemyService()
    {
        _messageRouter = ServiceFactory.Instance.GetService<MessageRouter>();
        _entityService = ServiceFactory.Instance.GetService<EntityService>();
    }

    public void SpawnEnemy(EnemyType enemyType)
    {
        var currentId = _entityService.GetNewEntityId();
        var messageRouter = ServiceFactory.Instance.GetService<MessageRouter>();
        messageRouter.RaiseMessage(new SpawnEnemyMessage(currentId, enemyType));
        RegisterEnemy(currentId, enemyType);
    }

    public void DestroyEnemy(int enemyID)
    {
        for (int i = 0; i < _activeMinions.Count; ++i)
        {
            if (_activeMinions[i].EntityId == enemyID)
            {
                var message = new DestroyEnemyMessage();
                message.EntityId = _activeMinions[i].EntityId;
                _messageRouter.RaiseMessage(message);
                _activeMinions.Remove(_activeMinions[i]);
                break;
            }
        }
    }

    public void RegisterEnemy(int enemyID, EnemyType enemyType)
    {
        MinionData newMinion = new MinionData();
        newMinion.EntityId = enemyID;
        newMinion.enemyType = enemyType;

        _activeMinions.Add(newMinion);
    }
}
