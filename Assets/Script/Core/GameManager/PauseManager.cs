using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PauseManager : MonoBehaviour
{
    [SerializeField]
    private GameObject settingsPanel;
    public static bool IsPaused { get; private set; }
   

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        IsPaused = !IsPaused;

        settingsPanel.SetActive(IsPaused);

        Time.timeScale = IsPaused ? 0f : 1f;
    }

    public void Resume()
    {
        IsPaused = false;

        settingsPanel.SetActive(false);

        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}