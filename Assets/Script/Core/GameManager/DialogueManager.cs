using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public GameObject panel;

    public TMP_Text npcNameText;
    public TMP_Text dialogueText;

    string[] currentDialogue;

    private PlayerMovement playerMovement;


    int index;

    bool isTalking;
    public bool IsTalking => isTalking;
    void Awake()
    {
        Instance = this;
        playerMovement = FindAnyObjectByType<PlayerMovement>();
    }

    void Update()
    {
        if (!isTalking)
            return;

        if (Keyboard.current.eKey.wasPressedThisFrame ||
            Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            NextLine();
        }
    }

    public void StartDialogue(string npcName, string[] dialogue)
    {
        panel.SetActive(true);

        playerMovement.enabled = false;

        npcNameText.text = npcName;

        currentDialogue = dialogue;

        index = 0;

        dialogueText.text = currentDialogue[index];

        isTalking = true;
    }

    void NextLine()
    {
        index++;

        if (index >= currentDialogue.Length)
        {
            EndDialogue();
            return;
        }

        dialogueText.text = currentDialogue[index];
    }

    void EndDialogue()
    {
        panel.SetActive(false);
        playerMovement.enabled = true;
        isTalking = false;
    }
}