using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Interact : MonoBehaviour, IInteractable
{
    [Header("Interaction")]
    public string objectMessage;
    public string InteractMessage => objectMessage;

    public string GetName => dialogue.name;

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

    // Per-character radio dialogues: each character maps to multiple variant dialogues (each variant is an array of sentences)
    static readonly System.Collections.Generic.Dictionary<string, string[][]> perCharacterRadio = new System.Collections.Generic.Dictionary<string, string[][]>()
    {
        { "Evelyn", new string[][] {
            new string[] {
                "...tuning... sift through the quiet corners of a mourning heart...",
                "Evelyn remembers a lullaby turned argument, a teacup left in haste...",
                "A shadow of regret passes — not malice, but an ache that lingers...",
                "She thinks of locked drawers and promises she couldn't keep...",
                "The signal trembles, soft and sorrowful."
            },
           
        }},
        { "Lily", new string[][] {
            new string[] {
                "...static, then the hush of careful steps...",
                "Lily sees linens folded, a stain she can't yet name...",
                "Her mind skips: doors, footsteps, the echo of hurried apologies...",
                "She clutches a memory of a light turned out too soon...",
                "The broadcast fades, leaving uncertainty in its wake."
            },
            new string[] {
                "A whisper of fabric brushing wood...",
                "She remembers a hand that lingered on a sleeve, then pulled away...",
                "There is a scent she can't place, and a neighbor's misplaced glance...",
                "She repeats 'I didn't see' like a charm against being suspected...",
                "The frequency stumbles and dies."
            },
            new string[] {
                "The signal finds a small, nervous voice...",
                "Lily counts the cups she set on that table, and whether one was out of place...",
                "She imagines footsteps echoing toward the garden and then stopping...",
                "The thought is fragile, honest or easily broken...",
                "Static. The memory drifts."
            }
        }},
        { "Thomas", new string[][] {
            new string[] {
                "...frequency picks up an assertive recollection...",
                "Thomas rehearses a retort, a practiced defense and a clipped laugh...",
                "A ledger of grievances flickers through his head like ledger paper...",
                "He pauses on one late invoice and a name crossed out in haste...",
                "Static eats the rest; the thought is sharper than it should be."
            },
            new string[] {
                "Muted interference, then a tidy memory...",
                "He returns to a phrase he likes to use in anger, testing its edge...",
                "There is a list he keeps—debts, slights, scores yet unpaid...",
                "A flash: doors slammed and a silhouette gone too fast...",
                "The broadcast blurs and moves on."
            },
            new string[] {
                "A clipped laugh, then ledger-paper thoughts...",
                "Thomas imagines catching someone in a lie, the delicious certainty of being right...",
                "He narrows his eyes at a name and almost smiles...",
                "The impression is calculation, wound with wounded pride...",
                "Static. The channel closes."
            }
        }},
        { "Marcus", new string[][] {
            new string[] {
                "...interference, then a prickling defensiveness...",
                "Marcus counts the ways he was slighted, and the ways he struck back...",
                "An image: hands clenched, then letting go — or pretending to...",
                "He imagines others watching, waiting for him to falter...",
                "The frequency coughs and leaves an aftertaste of anger."
            },
            new string[] {
                "A low hum, then a flash of temper...",
                "He paces through a memory of a door slammed and the heat in his chest...",
                "There is a bruise he insists wasn't his, or a joke that went too far...",
                "He keeps replaying a look that felt like a challenge...",
                "Static masks the next thought."
            },
            new string[] {
                "The station picks up a resentful beat...",
                "Marcus imagines the perfect comeback, then feels the sting when it fails...",
                "He pictures hands grabbing, then an awkward release...",
                "The memory tastes like rust and quick apologies...",
                "Then only white noise."
            }
        }},
        { "Clara", new string[][] {
            new string[] {
                "...a tension under the breath, an almost-smile in memory...",
                "Clara recalls a deliberate step, a light adjusted to conceal...",
                "A thought that tastes like opportunity, not regret...",
                "Something she tells herself to remember, or to forget...",
                "Static smothers a trailing confession."
            },
            new string[] {
                "Quiet signal—then the measured click of a plan rehearsed...",
                "She counts exits, times, the exact tilt of a lamp to hide a wet patch...",
                "There is no tremor in her voice when she imagines what she did...",
                "Only a calm, practiced certainty that unnerves the receiver...",
                "The channel goes thin and ends."
            },
            new string[] {
                "A clinical hush, then a tidy memory...",
                "Clara smooths an apron in her mind and arranges the scene as if setting a table...",
                "She frames an alibi like a photograph—angle, light, witness placement...",
                "The thought is efficient, almost proud...",
                "Static. The rest is a blank."
            }
        }}
    };

    void IInteractable.Interact()
    {
        TriggerDialogue();
    }

    void IInteractable.Accuse()
    {
        AccuseKiller();
    }

    public void AccuseKiller()
    {
        if (isKiller)
        {
            SceneManager.LoadScene("WinScene");
            Debug.LogWarning("you won");
        }
        else
        {
            SceneManager.LoadScene("LoseScene");
            Debug.LogWarning("you lose");
        }

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

        // Show radio: always create a fresh instance (or clone the assigned object) and parent it to the camera/radioParent.
        // This avoids the radio becoming 'stuck' at a previous transform or only appearing the first time.
        Transform parentTransform = radioParent != null ? radioParent : (Camera.main != null ? Camera.main.transform : null);

        // Attempt to auto-find a scene radio template if nothing provided (do this BEFORE instantiation)
        GameObject templateRadio = null;
        if (radioObject == null && radioPrefab == null)
        {
            GameObject sceneRadio = GameObject.Find("Radio");
            if (sceneRadio == null)
            {
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
                templateRadio = sceneRadio;
                Debug.Log("Interact: auto-found scene radio template '" + sceneRadio.name + "' for " + gameObject.name);
            }
        }
        else if (radioObject != null)
        {
            templateRadio = radioObject;
        }

        // Clean up any previously spawned radio instance to avoid duplicates or stale transforms
        if (spawnedRadio != null)
        {
            Debug.Log("Interact: destroying previous spawned radio before creating a new one for " + gameObject.name);
            GameObject.Destroy(spawnedRadio);
            spawnedRadio = null;
        }

        // Instantiate either a provided prefab or a clone of the template radio (if available)
        if (radioPrefab != null)
        {
            if (parentTransform != null)
            {
                spawnedRadio = GameObject.Instantiate(radioPrefab, parentTransform);
                spawnedRadio.transform.localPosition = radioLocalPosition;
                spawnedRadio.transform.localEulerAngles = radioLocalEuler;
                Debug.Log("Interact: instantiated radioPrefab under parent " + parentTransform.name + " for " + gameObject.name);
            }
            else
            {
                spawnedRadio = GameObject.Instantiate(radioPrefab);
                spawnedRadio.transform.position = radioLocalPosition;
                spawnedRadio.transform.eulerAngles = radioLocalEuler;
                Debug.Log("Interact: instantiated radioPrefab at world position for " + gameObject.name);
            }
            spawnedRadio.SetActive(true);
        }
        else if (templateRadio != null)
        {
            if (parentTransform != null)
            {
                spawnedRadio = GameObject.Instantiate(templateRadio, parentTransform);
                spawnedRadio.transform.localPosition = radioLocalPosition;
                spawnedRadio.transform.localEulerAngles = radioLocalEuler;
                Debug.Log("Interact: instantiated clone of template radio under " + parentTransform.name + " for " + gameObject.name);
            }
            else
            {
                spawnedRadio = GameObject.Instantiate(templateRadio);
                spawnedRadio.transform.position = radioLocalPosition;
                spawnedRadio.transform.eulerAngles = radioLocalEuler;
                Debug.Log("Interact: instantiated clone of template radio at world position for " + gameObject.name);
            }
            spawnedRadio.SetActive(true);
        }

        // Generate a radio-specific dialogue dynamically so it sounds like the radio is reading their mind.
        // We create placeholder lines for now. This always overrides existing dialogue during radio interaction.
        Dialogue radioGenerated = new Dialogue();
        radioGenerated.name = "Radio";
        string subject = string.IsNullOrEmpty(gameObject.name) ? "them" : gameObject.name;
        // If developer assigned a radioDialogue in the inspector, prefer it and don't override.
        if (radioDialogue != null && radioDialogue.sentences != null && radioDialogue.sentences.Length > 0)
        {
            if (manager.isActive == false)
            {
                manager.StartDialogue(radioDialogue);
            }
            else
            {
                manager.DisplayNextSentence();
            }

            if (spawnedRadio != null)
            {
                StartCoroutine(HideRadioWhenDialogueEnds(manager));
            }
            return;
        }
        // First, check per-character explicit map (each entry contains multiple variant arrays)
        if (perCharacterRadio.TryGetValue(gameObject.name, out var explicitVariants) && explicitVariants != null && explicitVariants.Length > 0)
        {
            int variantIndex = 0;
            if (!string.IsNullOrEmpty(gameObject.name))
            {
                variantIndex = Mathf.Abs(gameObject.name.GetHashCode()) % explicitVariants.Length;
            }
            else
            {
                variantIndex = Mathf.Abs(gameObject.GetInstanceID()) % explicitVariants.Length;
            }

            radioGenerated.sentences = explicitVariants[variantIndex];
            Debug.Log($"Interact: selected per-character variant {variantIndex} for '{gameObject.name}'");

            if (manager.isActive == false)
            {
                manager.StartDialogue(radioGenerated);
            }
            else
            {
                manager.DisplayNextSentence();
            }

            if (spawnedRadio != null)
            {
                StartCoroutine(HideRadioWhenDialogueEnds(manager));
            }
            return;
        }
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
        // Prefer a stable mapping per-character using the GameObject name hash.
        // Fall back to InstanceID when name is empty to guarantee variability.
        int stableHash;
        if (!string.IsNullOrEmpty(gameObject.name))
        {
            stableHash = Mathf.Abs(gameObject.name.GetHashCode());
        }
        else
        {
            stableHash = Mathf.Abs(gameObject.GetInstanceID());
        }

        if (isKiller)
        {
            int idx = stableHash % killerVariants.Length;
            chosen = killerVariants[idx];
            Debug.Log($"Interact: selected killer radio variant {idx} for '{gameObject.name}' (hash={stableHash})");
        }
        else
        {
            int idx = stableHash % nonKillerVariants.Length;
            chosen = nonKillerVariants[idx];
            Debug.Log($"Interact: selected non-killer radio variant {idx} for '{gameObject.name}' (hash={stableHash})");
            Debug.Log("HELLO");
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

        // Note: we intentionally do not assign a found scene radio to the instance `radioObject` here.
        // Cloning (spawnedRadio) is used above so the original scene template is not mutated or left active.

        // Start coroutine to hide/destroy the spawned radio when the dialogue ends
        if (spawnedRadio != null)
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
    }



}
