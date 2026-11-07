using UnityEngine;

[System.Serializable]
public class EnemyData
{
    public string enemyID;
    public string prefabID;

    public float posX;
    public float posY;
    public float posZ;

    public int currentHealth;

    public bool isDead;
}