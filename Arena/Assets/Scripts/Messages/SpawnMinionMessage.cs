/// <summary>
/// A message for spawning an enemy
/// </summary>
public class SpawnMinionMessage
{
   public SpawnMinionMessage(int id, MinionType enemyType)
   {
      Id = id;
      MinionType = enemyType;
   }
   
   public int Id { get; set; }
   public MinionType MinionType { get; set; }
}
