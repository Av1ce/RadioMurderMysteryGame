using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public Queue<string> sentences;
    public TMP_Text dialogText;
    public TMP_Text nameText;
    public GameObject pressE;

    public bool isActive = false;

    void Start()
    {
        sentences = new Queue<string> ();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        Debug.Log("Starting conversation with " + dialogue.name);
        dialogText.gameObject.SetActive(true);
        if (nameText != null)
        {
            nameText.gameObject.SetActive(true);
            nameText.text = dialogue.name;
        }
        pressE.gameObject.SetActive(false);
        sentences.Clear();

        foreach(string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        isActive = true;
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if(sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        Debug.Log("NUMBER OF SENTENCES:" + sentences.Count);
        string sentence = sentences.Dequeue();
        
        dialogText.text = sentence;

        Debug.Log(sentence);
    }

    void EndDialogue()
    {
        Debug.Log("End of conversation");
        dialogText.gameObject.SetActive(false);
        if (nameText != null)
        {
            nameText.gameObject.SetActive(false);
            nameText.text = string.Empty;
        }
        pressE.gameObject.SetActive(true);
        isActive = false;
    }

}
