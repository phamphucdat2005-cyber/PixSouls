using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public string npcName;

    [TextArea]
    public string[] dialogue;

    PlayerInteraction player;

    private void Start()
    {
        player = FindAnyObjectByType<PlayerInteraction>();
    }

    public void StartDialogue()
    {
        DialogueManager.Instance.StartDialogue(npcName, dialogue);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player.SetCurrentNPC(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player.ClearCurrentNPC();
        }
    }
}