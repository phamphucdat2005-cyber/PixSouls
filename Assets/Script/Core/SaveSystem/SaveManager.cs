using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    private string savePath;

    [Header("Enemy")]
    [SerializeField] private GameObject defaultEnemyPrefab;



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            savePath = Path.Combine(Application.persistentDataPath, "save.json");

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameScene" && MainMenuManager.ContinueRequested)
        {
            MainMenuManager.ContinueRequested = false;
            LoadGame();
        }
    }
    private void Start()
    {
        if (MainMenuManager.ContinueRequested)
        {
            MainMenuManager.ContinueRequested = false;
            LoadGame();
        }
    }
    public void SaveGame()
    {
        GameData data = new GameData();

        ISaveable[] saveables = FindObjectsByType<MonoBehaviour>()
    .OfType<ISaveable>()
    .ToArray();

        foreach (ISaveable saveable in saveables)
        {
            saveable.Save(data);
        }

        string json = JsonUtility.ToJson(data, true);

        File.WriteAllText(savePath, json);

        Debug.Log("Game Saved");
        Debug.Log(savePath);
    }

    public void LoadGame()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("Save file not found.");
            return;
        }

        string json = File.ReadAllText(savePath);
        GameData data = JsonUtility.FromJson<GameData>(json);

        ISaveable[] saveables = FindObjectsByType<MonoBehaviour>()
    .OfType<ISaveable>()
    .ToArray();

        foreach (ISaveable saveable in saveables)
        {
            if (saveable is EnemyHealth)
                continue;

            saveable.Load(data);
        }

        EnemyHealth[] enemies = FindObjectsByType<EnemyHealth>();

        foreach (EnemyHealth enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }

        foreach (EnemyData enemyData in data.enemies)
        {
            if (enemyData.isDead)
                continue;

            GameObject prefab = EnemyDatabase.Instance != null
                ? EnemyDatabase.Instance.GetPrefab(enemyData.prefabID)
                : null;

            if (prefab == null)
                prefab = defaultEnemyPrefab;

            if (prefab == null)
            {
                Debug.LogWarning($"Enemy prefab not found for ID '{enemyData.prefabID}'. Skipping enemy.");
                continue;
            }

            GameObject enemy = Instantiate(
                prefab,
                new Vector3(
                    enemyData.posX,
                    enemyData.posY,
                    enemyData.posZ),
                Quaternion.identity);

            EnemyHealth health = enemy.GetComponent<EnemyHealth>();

            if (health != null)
            {
                health.enemyID = enemyData.enemyID;
                health.prefabID = enemyData.prefabID;
                health.CurrentHealth = enemyData.currentHealth;
            }
        }

        Debug.Log("Game Loaded");
    }

    public bool HasSave()
    {
        return File.Exists(savePath);
    }

    public void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Save Deleted");
        }
    }
}