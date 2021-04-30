using Frictionless;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A Unity MonoBehaviour which listens for messages about which enemies
/// to spawn and despawn.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    private Dictionary<int, GameObject> _minions = new Dictionary<int, GameObject>();

    [SerializeField] 
    private GameObject _skeleton;
    
    [SerializeField] 
    private GameObject _mushroomMan;

    private void Start()
    {
        var messageRouter = ServiceFactory.Instance.GetService<MessageRouter>();
        messageRouter.AddListener<SpawnEnemyMessage>(OnSpawnEnemy);
        messageRouter.AddListener<DestroyEnemyMessage>(OnDestroyEnemy);
    }

    private void OnSpawnEnemy(SpawnEnemyMessage message)
    {
        GameObject newMinion;

        if (message.EnemyType == EnemyType.Skeleton)
        {
            newMinion = Instantiate(_skeleton, transform);
        }
        else if (message.EnemyType == EnemyType.MushroomMan)
        {
            newMinion = Instantiate(_mushroomMan, transform);
        }
        else
        {
            Debug.LogError("Enemy type not supported: " + message.EnemyType);
            return;
        }

        newMinion.GetComponent<EntityInstance>().EntityId = message.Id;
        _minions.Add(message.Id, newMinion);

    }

    private void OnDestroyEnemy(DestroyEnemyMessage message)
    {
        if (_minions.ContainsKey(message.EntityId))
        {
            Destroy(_minions[message.EntityId].gameObject);
            _minions.Remove(message.EntityId);
        }
    }
}
