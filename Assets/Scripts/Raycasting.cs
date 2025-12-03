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

    void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
            if (playerCamera == null)
            {
                Debug.LogWarning("Raycasting: playerCamera not assigned and no Camera.main found. Assign a camera to Raycasting.playerCamera.");
            }
            else
            {
                Debug.Log("Raycasting: assigned playerCamera to Camera.main");
            }
        }
    }

    void Update()
    {
        UpdateInteractable();
        UpdateInteractionText();
        CheckInteractionKey();


    }

    void UpdateInteractable()
    {
        var ray = playerCamera.ViewportPointToRay(new Vector2(0.5f, 0.5f));

        Physics.Raycast(ray, out var hit, interactionDistance);

        currentTarget = hit.collider?.GetComponent<IInteractable>();
    }

    void UpdateInteractionText()
    {
        if (currentTarget != null)
        {
            // Show basic message
            string msg = currentTarget.InteractMessage;

            // If the target is an Interact and has radio enabled, show the Q hint
            var concrete = currentTarget as Interact;
            if (concrete != null && concrete.enableRadioInteraction && (concrete.radioDialogue != null || concrete.radioObject != null))
            {
                msg += "  <color=#AAAAAA>(E to talk, Q to pull out radio)</color>";
            }

            interactionText.text = msg;
            interactionText.transform.parent.gameObject.SetActive(true); // show panel
            return;
        }
        interactionText.transform.parent.gameObject.SetActive(false);

        interactionText.text = string.Empty;

    }

    void CheckInteractionKey()
    {
        if (Input.GetKeyDown(KeyCode.E) && currentTarget != null)
        {
            currentTarget.Interact();

        }

        if (Input.GetKeyDown(KeyCode.Q) && currentTarget != null)
        {
            Debug.Log("Raycasting: Q pressed. currentTarget=" + (currentTarget == null ? "null" : currentTarget.ToString()));
            var concrete = currentTarget as Interact;
            if (concrete != null)
            {
                Debug.Log("Raycasting: currentTarget is Interact. enableRadioInteraction=" + concrete.enableRadioInteraction + ", radioObject=" + (concrete.radioObject != null) + ", radioPrefab=" + (concrete.radioPrefab != null));
            }

            if (concrete != null && concrete.enableRadioInteraction)
            {
                concrete.InteractWithRadio();
            }
            else
            {
                // fallback to normal interaction
                Debug.Log("Raycasting: falling back to normal Interact");
                currentTarget.Interact();
            }
        }
    }


}
