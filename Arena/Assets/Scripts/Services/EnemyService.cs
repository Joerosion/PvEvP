using Frictionless;

/// <summary>
/// An example service which can be invoked from anywhere that
/// broadcasts message to spawn enemies
/// Also keeps track of the current ID of enemies
/// </summary>
public class EnemyService
{
    private int currentId = 0;
    
    public void SpawnEnemy(EnemyType enemyType)
    {
        var messageRouter = ServiceFactory.Instance.GetService<MessageRouter>();
        messageRouter.RaiseMessage(new SpawnEnemyMessage(currentId, enemyType));
        currentId++;
    }
}
