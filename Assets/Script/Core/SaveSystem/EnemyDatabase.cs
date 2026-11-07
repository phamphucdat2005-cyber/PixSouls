using UnityEngine;

[System.Serializable]
public class EnemyPrefab
{
    public string prefabID;
    public GameObject prefab;
}

public class EnemyDatabase : MonoBehaviour
{
    public static EnemyDatabase Instance;

    public EnemyPrefab[] enemies;

    private void Awake()
    {
        Instance = this;
    }

    public GameObject GetPrefab(string id)
    {
        foreach (EnemyPrefab enemy in enemies)
        {
            if (enemy.prefabID == id)
                return enemy.prefab;
        }

        return null;
    }
}