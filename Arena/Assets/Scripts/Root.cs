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
        ServiceFactory.Instance.GetService<GoldService>().SpawnGold(5);
    }

    private void RegisterServices()
    {
        ServiceFactory.Instance.RegisterSingleton(new EntityService());
        ServiceFactory.Instance.RegisterSingleton(new MinionService());
        ServiceFactory.Instance.RegisterSingleton(new GoldService());
        ServiceFactory.Instance.RegisterSingleton(new PlayerDataService());
    }
    
    private IEnumerator SpawnEnemy()
    {
        while (true)
        {
            yield return new WaitForSeconds(5.0f);
            MinionType enemyToSpawn = Random.value > 0.5f ? MinionType.MushroomMan : MinionType.Skeleton;
            ServiceFactory.Instance.GetService<MinionService>().SpawnMinion(enemyToSpawn);
        }
    }
}



