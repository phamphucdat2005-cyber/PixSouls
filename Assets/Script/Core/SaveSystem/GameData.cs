using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public PlayerData player;

    public CompanionData companion;

    public List<EnemyData> enemies = new();
}