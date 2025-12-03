using System.Collections;
using UnityEngine;

public class Interact : MonoBehaviour, IInteractable
{
    [Header("Interaction")]
    public string objectMessage;
    public string InteractMessage => objectMessage;
    public Dialogue dialogue;

    [Header("Radio (optional)")]
    public Dialogue radioDialogue;
    public GameObject radioObject; // assign an instance or prefab; recommended: a disabled instance in scene
    // If you prefer to provide a prefab instead of a scene instance, assign it here.
    // The script will instantiate it under `radioParent` (or the main camera if radioParent is null).
    public GameObject radioPrefab;
    public Transform radioParent;
    GameObject spawnedRadio;
    public bool enableRadioInteraction = true;
    [Header("Auto placement (used if no radioParent set)")]
    public Vector3 radioLocalPosition = new Vector3(0.2f, -0.15f, 0.5f);
    public Vector3 radioLocalEuler = new Vector3(0f, 180f, 0f);
    [Header("Investigation")]
    [Tooltip("Tick this on the one NPC that is the killer. The radio will generate slightly different 'mind-reading' lines for them.")]
    public bool isKiller = false;

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

    // Called when player presses the radio key (Q)
    public void InteractWithRadio()
    {
        if (!enableRadioInteraction)
        {
            // fallback to normal interaction
            TriggerDialogue();
            return;
        }

        Debug.Log("Interact: InteractWithRadio called on " + gameObject.name + ". enableRadioInteraction=" + enableRadioInteraction + ", radioObject=" + (radioObject != null) + ", radioPrefab=" + (radioPrefab != null));

        DialogueManager manager = FindObjectOfType<DialogueManager>();
        if (manager == null)
        {
            Debug.LogWarning("No DialogueManager found in scene!");
            return;
        }

        // Show radio object if assigned (scene instance)
        if (radioObject != null)
        {
            radioObject.SetActive(true);
            Debug.Log("Interact: activated radioObject instance for " + gameObject.name);
        }
        else if (radioPrefab != null)
        {
            // Instantiate prefab under radioParent (or main camera) so it appears in player's hands
            if (spawnedRadio == null)
            {
                Transform parent = radioParent != null ? radioParent : (Camera.main != null ? Camera.main.transform : null);
                if (parent != null)
                {
                    spawnedRadio = GameObject.Instantiate(radioPrefab, parent);
                    spawnedRadio.transform.localPosition = Vector3.zero;
                    spawnedRadio.transform.localRotation = Quaternion.identity;
                    Debug.Log("Interact: instantiated radioPrefab under parent " + parent.name + " for " + gameObject.name);
                }
                else
                {
                    // Try to find a camera to parent to, and apply default local offset so it appears in player's view
                    Transform parentFallback = Camera.main != null ? Camera.main.transform : null;
                    if (parentFallback != null)
                    {
                        spawnedRadio = GameObject.Instantiate(radioPrefab, parentFallback);
                        spawnedRadio.transform.localPosition = radioLocalPosition;
                        spawnedRadio.transform.localEulerAngles = radioLocalEuler;
                        Debug.Log("Interact: instantiated radioPrefab under Camera.main with default offset for " + gameObject.name);
                    }
                    else
                    {
                        spawnedRadio = GameObject.Instantiate(radioPrefab);
                        Debug.Log("Interact: instantiated radioPrefab at world origin for " + gameObject.name);
                    }
                }
            }
            else
            {
                spawnedRadio.SetActive(true);
                Debug.Log("Interact: re-activated previously spawned radio for " + gameObject.name);
            }
        }

        // Generate a radio-specific dialogue dynamically so it sounds like the radio is reading their mind.
        // We create placeholder lines for now. This always overrides existing dialogue during radio interaction.
        Dialogue radioGenerated = new Dialogue();
        radioGenerated.name = "Radio";
        string subject = string.IsNullOrEmpty(gameObject.name) ? "them" : gameObject.name;
        // Create several subtle variants so different NPCs sound different.
        // Use the GameObject instance id to pick a variant deterministically per NPC.
        string[][] killerVariants = new string[][] {
            new string[] {
                "...tuning... picking up faint thoughts...",
                $"A cold fragment in {subject}'s mind... a flash of hands, a slammed door...",
                "They replay the scene: a step too many, a breath held too long...",
                $"A voice trails off: 'I kept thinking, if they hadn't pushed me, maybe...'",
                "The signal warbles. Static swallows the thought."
            },
            new string[] {
                "Static at first, then a memory surfaces...",
                $"{subject} pictures a knife, then shakes their head at the image...",
                "A hurried whisper: 'It wasn't supposed to happen like that'...",
                "They catch themselves smiling nervously, as if rehearsing an excuse...",
                "The broadcast crinkles and then goes quiet."
            },
            new string[] {
                "Soft interference... then a flash of motion...",
                $"A fragment: a stumble, a hand grabbing, {subject} counting heartbeats...",
                "They mutter to themselves: 'I didn't mean—' and stop mid-thought...",
                "An image fades: the room, the lamp, the sudden silence...",
                "Then only static remains."
            }
        };

        string[][] nonKillerVariants = new string[][] {
            new string[] {
                "...tuning... stray impressions surfacing...",
                $"Faint memory in {subject}'s head: a shout, a lamp overturned...",
                "They remember arguing, then leaving the room in a hurry...",
                $"A passing thought: 'I saw someone run; I thought it was over then.'",
                "Static. The memory dims and drifts away."
            },
            new string[] {
                "The frequency catches a nervous recollection...",
                $"{subject} recalls footsteps in the hallway and a slammed door...",
                "They mention someone else leaving the house early that night...",
                "A distracted line: 'It was him, I just... I couldn't tell for sure'...",
                "The signal flickers and peters out."
            },
            new string[] {
                "A faint hum, then a memory slips through...",
                $"{subject} thinks about an argument and a thrown glass...",
                "They say: 'I ran away because I was scared'—more shocked than guilty...",
                "The impression is of panic, not planning...",
                "Static covers the rest."
            }
        };

        string[] chosen;
        if (isKiller)
        {
            int idx = Mathf.Abs(gameObject.GetInstanceID()) % killerVariants.Length;
            chosen = killerVariants[idx];
        }
        else
        {
            int idx = Mathf.Abs(gameObject.GetInstanceID()) % nonKillerVariants.Length;
            chosen = nonKillerVariants[idx];
        }

        // Replace subject placeholders in chosen lines (they already include subject via interpolation), assign
        radioGenerated.sentences = chosen;

        Dialogue toUse = radioGenerated;

        if (manager.isActive == false)
        {
            manager.StartDialogue(toUse);
        }
        else
        {
            manager.DisplayNextSentence();
        }

        // If no radioObject or radioPrefab assigned, attempt to find a scene Radio object automatically
        if (radioObject == null && radioPrefab == null)
        {
            // Try find by name first
            GameObject sceneRadio = GameObject.Find("Radio");
            if (sceneRadio == null)
            {
                // fallback to any object whose name contains "radio" (case-insensitive)
                var all = GameObject.FindObjectsOfType<GameObject>();
                foreach (var go in all)
                {
                    if (go.name.ToLower().Contains("radio"))
                    {
                        sceneRadio = go;
                        break;
                    }
                }
            }

            if (sceneRadio != null)
            {
                // parent under camera if possible so it appears in view
                Transform parent = radioParent != null ? radioParent : (Camera.main != null ? Camera.main.transform : null);
                if (parent != null)
                {
                    sceneRadio.transform.SetParent(parent, false);
                    sceneRadio.transform.localPosition = radioLocalPosition;
                    sceneRadio.transform.localEulerAngles = radioLocalEuler;
                }
                radioObject = sceneRadio; // use it as the radioObject
                Debug.Log("Interact: auto-found scene radio '" + sceneRadio.name + "' and assigned as radioObject for " + gameObject.name);
            }
        }

        // Start coroutine to hide/destroy whichever radio was shown
        if (radioObject != null || radioPrefab != null)
        {
            StartCoroutine(HideRadioWhenDialogueEnds(manager));
        }
    }

    IEnumerator HideRadioWhenDialogueEnds(DialogueManager manager)
    {
        while (manager != null && manager.isActive)
        {
            yield return null;
        }

        if (spawnedRadio != null)
        {
            Debug.Log("Interact: destroying spawned radio for " + gameObject.name);
            GameObject.Destroy(spawnedRadio);
            spawnedRadio = null;
        }
        else if (radioObject != null)
        {
            Debug.Log("Interact: deactivating radioObject instance for " + gameObject.name);
            radioObject.SetActive(false);
        }
    }

}
