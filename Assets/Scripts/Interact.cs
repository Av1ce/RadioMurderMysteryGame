using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class Interact : MonoBehaviour, IInteractable
{
    public string objectMessage;
    public string InteractMessage => objectMessage;
    public Dialogue dialogue;

    void IInteractable.Interact()
    {
        TriggerDialogue();
    }

    public void TriggerDialogue()
    {
        DialogueManager manager = FindObjectOfType<DialogueManager>();
        if (manager != null)
        {
            if (manager.isActive == false)
            {
                manager.StartDialogue(dialogue);
            }
            else
            {
                manager.DisplayNextSentence();
            }
        }
        else
        {
            Debug.LogWarning("No DialogueManager found in scene!");
        }
    }


}
