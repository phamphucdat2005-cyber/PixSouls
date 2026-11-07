using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject settingsPanel;
    [SerializeField] private AudioClip menuMusic;
    public static bool ContinueRequested = false;
    public void NewGame()
    {
        SceneManager.LoadScene("GameScene");
    }
    private void Start()
    {
        AudioManager.Instance.PlayMusic(menuMusic);
    }
    public void ContinueGame()
    {
        ContinueRequested = true;
        SceneManager.LoadScene("GameScene");
    }
    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}