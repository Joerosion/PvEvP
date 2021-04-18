using Frictionless;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _playerPrefab;
    private void Start()
    {
        var messageRouter = ServiceFactory.Instance.GetService<MessageRouter>();
        messageRouter.AddListener<SpawnPlayerMessage>(OnSpawnPlayer);
    }

    private void OnSpawnPlayer(SpawnPlayerMessage message)
    {
        var newGameObject = Instantiate(_playerPrefab, transform);
        newGameObject.GetComponent<PlayerInstance>().EntityId = message.EntityId;
    }
}
