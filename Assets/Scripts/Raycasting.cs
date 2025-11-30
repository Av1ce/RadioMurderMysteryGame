using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Raycasting : MonoBehaviour
{
    public Camera playerCamera;

    public TextMeshProUGUI interactionText;

    IInteractable currentTarget;

    float interactionDistance = 5f;

    void Update()
    {
        CheckInteractionKey();
        UpdateInteractable();
        UpdateInteractionText();

        
    }

    void UpdateInteractable()
    {
        var ray = playerCamera.ViewportPointToRay(new Vector2(0.5f, 0.5f));

        Physics.Raycast(ray, out var hit, interactionDistance);

        currentTarget = hit.collider?.GetComponent<IInteractable>();
    }

    void UpdateInteractionText()
    {
        if (currentTarget != null) {
            interactionText.text = currentTarget.InteractMessage;
            return;
        }
        interactionText.text = string.Empty;

    }

    void CheckInteractionKey()
    {
        if (Input.GetKeyDown(KeyCode.E) && currentTarget != null) {
            currentTarget.Interact();
  
        }
    }


}
