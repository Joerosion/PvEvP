using Frictionless;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A Unity MonoBehaviour which listens for messages about which enemies
/// to spawn and despawn.
/// </summary>
public class MinionSpawner : MonoBehaviour
{
    private Dictionary<int, GameObject> _minions = new Dictionary<int, GameObject>();

    [SerializeField] 
    private GameObject _skeleton;
    
    [SerializeField] 
    private GameObject _mushroomMan;

    private void Start()
    {
        var messageRouter = ServiceFactory.Instance.GetService<MessageRouter>();
        messageRouter.AddListener<SpawnMinionMessage>(OnSpawnMinion);
        messageRouter.AddListener<DestroyMinionMessage>(OnDestroyMinion);
    }

    private void OnSpawnMinion(SpawnMinionMessage message)
    {
        GameObject newMinion;

        if (message.MinionType == MinionType.Skeleton)
        {
            newMinion = Instantiate(_skeleton, transform);
        }
        else if (message.MinionType == MinionType.MushroomMan)
        {
            newMinion = Instantiate(_mushroomMan, transform);
        }
        else
        {
            Debug.LogError("Minion type not supported: " + message.MinionType);
            return;
        }

        newMinion.GetComponent<EntityInstance>().EntityId = message.Id;
        _minions.Add(message.Id, newMinion);

    }

    private void OnDestroyMinion(DestroyMinionMessage message)
    {
        if (_minions.ContainsKey(message.EntityId))
        {
            Destroy(_minions[message.EntityId].gameObject);
            _minions.Remove(message.EntityId);
        }
    }
}
