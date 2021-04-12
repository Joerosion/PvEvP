/// <summary>
/// A message for spawning an enemy
/// </summary>
public class SpawnEnemyMessage
{
   public SpawnEnemyMessage(int id, EnemyType enemyType)
   {
      Id = id;
      EnemyType = enemyType;
   }
   
   public int Id { get; set; }
   public EnemyType EnemyType { get; set; }
}
