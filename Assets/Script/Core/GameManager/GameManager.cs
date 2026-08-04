using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private AudioClip gameplayMusic;

    [SerializeField]
    private GameObject settingsPanel;

    private bool isPaused = false;

    private void Awake()
    {
        Instance = this;
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
    private void Start()
    {
        AudioManager.Instance.PlayMusic(gameplayMusic);
    }
}