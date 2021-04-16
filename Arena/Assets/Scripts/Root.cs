using System.Collections;
using Frictionless;
using UnityEngine;

public class Root : MonoBehaviour
{
    private void Awake()
    {
        var messageRouter = new MessageRouter();
        ServiceFactory.Instance.RegisterSingleton(messageRouter);
        RegisterServices();
        StartCoroutine(SpawnEnemy());
        StartCoroutine(SpawnGoldDelayed());
    }

    private IEnumerator SpawnGoldDelayed()
    {
        yield return new WaitForSeconds(0.25f);
        ServiceFactory.Instance.GetService<PlayerDataService>().SpawnPlayer();
        ServiceFactory.Instance.GetService<GoldService>().SpawnGold(10);
    }

    private void RegisterServices()
    {
        ServiceFactory.Instance.RegisterSingleton(new EntityService());
        ServiceFactory.Instance.RegisterSingleton(new EnemyService());
        ServiceFactory.Instance.RegisterSingleton(new GoldService());
        ServiceFactory.Instance.RegisterSingleton(new PlayerDataService());
    }
    
    private IEnumerator SpawnEnemy()
    {
        while (true)
        {
            yield return new WaitForSeconds(5.0f);
            EnemyType enemyToSpawn = Random.value > 0.5f ? EnemyType.MushroomMan : EnemyType.Skeleton;
            ServiceFactory.Instance.GetService<EnemyService>().SpawnEnemy(enemyToSpawn);
        }
    }
}



