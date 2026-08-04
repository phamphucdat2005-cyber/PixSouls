using UnityEngine;

public class TriggerNpcDialog : MonoBehaviour
{
    [SerializeField]
    private GameObject interactionPrompt;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            interactionPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            interactionPrompt.SetActive(false);
        }
    }
}
