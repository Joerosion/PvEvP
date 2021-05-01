using System.Collections.Generic;
public class PlayerAttackMessage
{
    public int playerId { get; set; }
    public List<int> enemyHitList = new List<int>();
}
