using Frictionless;
using UnityEngine;

/// <summary>
/// A Unity MonoBehaviour which listens for messages about which enemies
/// to spawn and despawn.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] 
    private GameObject _skeleton;
    
    [SerializeField] 
    private GameObject _mushroomMan;

    private void Start()
    {
        var messageRouter = ServiceFactory.Instance.GetService<MessageRouter>();
        messageRouter.AddListener<SpawnEnemyMessage>(OnSpawnEnemy);
    }

    private void OnSpawnEnemy(SpawnEnemyMessage message)
    {
        if (message.EnemyType == EnemyType.Skeleton)
        {
            Instantiate(_skeleton);
        }
        else if (message.EnemyType == EnemyType.MushroomMan)
        {
            Instantiate(_mushroomMan);
        }
        else
        {
            Debug.LogError("Enemy type not supported: " + message.EnemyType);
        }
    }
}
