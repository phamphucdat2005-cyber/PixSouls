using UnityEngine;

public class SaveUI : MonoBehaviour
{
    public void SaveGame()
    {
        SaveManager.Instance.SaveGame();
    }

    public void LoadGame()
    {
        SaveManager.Instance.LoadGame();
    }
}