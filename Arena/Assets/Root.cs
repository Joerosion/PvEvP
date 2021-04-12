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
    }

    private void RegisterServices()
    {
        ServiceFactory.Instance.RegisterSingleton(new EnemyService());
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
