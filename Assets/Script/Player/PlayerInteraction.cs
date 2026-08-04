using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    NPCInteraction currentNPC;

    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (DialogueManager.Instance.IsTalking)
                return;
            if (currentNPC != null)
            {
               
                currentNPC.StartDialogue();
            }
           
        }
    }

    public void SetCurrentNPC(NPCInteraction npc)
    {
        currentNPC = npc;
     

    }

    public void ClearCurrentNPC()
    {
        currentNPC = null;
    }
}