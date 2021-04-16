using System.Collections.Generic;
using Frictionless;
using UnityEngine;

public class GoldSpawner : MonoBehaviour
{
    [SerializeField] 
    private GameObject _goldPrefab;

    [SerializeField] 
    private List<Transform> _spawnPositions;

    [SerializeField]
    private List<GoldInstance> _goldInstances;

    private void Start()
    {
        var messageRouter = ServiceFactory.Instance.GetService<MessageRouter>();
        messageRouter.AddListener<SpawnGoldMessage>(OnSpawnGold);
        messageRouter.AddListener<DestroyGoldMessage>(OnDestroyGold);
    }

    private void OnDestroyGold(DestroyGoldMessage message)
    {
        for(int i = 0; i < _goldInstances.Count; i++)
        {
            if(message.EntityId == _goldInstances[i].EntityId)
            {
                Destroy(_goldInstances[i].gameObject);
            }
        }
    }

    private void OnSpawnGold(SpawnGoldMessage message)
    {
        //var transformToSpawnAt = _spawnPositions[Random.Range(0, _spawnPositions.Count)];
        var transformToSpawnAt = _spawnPositions[1];
        
        var newGameObject = Instantiate(_goldPrefab, transformToSpawnAt);
        var goldInstance = newGameObject.GetComponent<GoldInstance>();
        goldInstance.EntityId = message.EntityId;
        _goldInstances.Add(goldInstance);
    }
}
