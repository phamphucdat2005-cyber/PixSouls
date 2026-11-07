using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private AudioClip gameplayMusic;

    [SerializeField] private GameObject settingsPanel;

    private bool isPaused = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void OnEnable()
    {
        // nothing here, placeholder for lifecycle clarity
    }

    private void StartContinueHandler()
    {
        StartCoroutine(HandleContinueLoad());
    }

    private void Start()
    {
        AudioManager.Instance.PlayMusic(gameplayMusic);
        StartCoroutine(HandleContinueLoad());
    }

    private IEnumerator HandleContinueLoad()
    {
        if (MainMenuManager.ContinueRequested)
        {
            MainMenuManager.ContinueRequested = false;
            float waitTime = 0f;
            float timeout = 3f;
            while (SaveManager.Instance == null && waitTime < timeout)
            {
                waitTime += Time.deltaTime;
                yield return null;
            }

            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.LoadGame();
            }
            else
            {
                Debug.LogWarning("SaveManager not found after loading GameScene; cannot load save.");
            }
        }
    }

    public void ToggleSettings()
    {
        isPaused = !isPaused;

        settingsPanel.SetActive(isPaused);

        Time.timeScale = isPaused ? 0f : 1f;
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}