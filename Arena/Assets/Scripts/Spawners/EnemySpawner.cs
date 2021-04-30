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
        if (message.EnemyType == EnemyType.Skeleton)
        {
            GameObject newMinion = Instantiate(_skeleton, transform);
            _minions.Add(message.Id, newMinion);
        }
        else if (message.EnemyType == EnemyType.MushroomMan)
        {
            GameObject newMinion = Instantiate(_mushroomMan, transform);
            _minions.Add(message.Id, newMinion);
        }
        else
        {
            Debug.LogError("Enemy type not supported: " + message.EnemyType);
        }
    }

    private void OnDestroyEnemy(DestroyEnemyMessage message)
    {
        for (int i = 0; i < _minions.Count; i++)
        {
            if (_minions.ContainsKey(message.EntityId))
            {
                Destroy(_minions[i].gameObject);
                _minions.Remove(message.EntityId);
            }
        }
    }
}
