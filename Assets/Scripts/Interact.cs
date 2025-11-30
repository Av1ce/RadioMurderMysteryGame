using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class Interact : MonoBehaviour, IInteractable
{
    public string objectMessage;
    public string InteractMessage => objectMessage;

    void IInteractable.Interact()
    {
        
    }


}
