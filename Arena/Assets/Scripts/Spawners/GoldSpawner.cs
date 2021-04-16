using System.Collections.Generic;
using Frictionless;
using UnityEngine;

public class GoldSpawner : MonoBehaviour
{
    [SerializeField] 
    private GameObject _goldPrefab;

    [SerializeField] 
    private List<Transform> _spawnPositions;

    private void Start()
    {
        var messageRouter = ServiceFactory.Instance.GetService<MessageRouter>();
        messageRouter.AddListener<SpawnGoldMessage>(OnSpawnGold);
        messageRouter.AddListener<DestroyGoldMessage>(OnDestroyGold);
    }

    private void OnDestroyGold(DestroyGoldMessage message)
    {
        
    }

    private void OnSpawnGold(SpawnGoldMessage message)
    {
        //var transformToSpawnAt = _spawnPositions[Random.Range(0, _spawnPositions.Count)];
        var transformToSpawnAt = _spawnPositions[1];
        
        var newGameObject = Instantiate(_goldPrefab, transformToSpawnAt);
        var goldInstance = newGameObject.GetComponent<GoldInstance>();
        goldInstance.EntityId = message.EntityId;
    }
}
