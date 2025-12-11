using UnityEngine;
using TMPro;

public class SubtitleSync : MonoBehaviour
{
    public AudioSource audioSource;
    public TMP_Text subtitleText;
    public Caption[] captions;
    public IntroFadeScene introFadeScene;
    public IIntro introInteractable;

    private int index = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            SkipIntro();
            return; 
        }

        if (index < captions.Length && audioSource.time >= captions[index].time)
        {
            subtitleText.text = captions[index].text;
            index++;
        }
    }
    void SkipIntro()
    {
        introInteractable = introFadeScene as IIntro;
        // Stop the audio
        audioSource.Stop();

        // Optionally hide the subtitle text
        subtitleText.text = "";

        // Set index to end so subtitles stop updating
        index = captions.Length;
        introInteractable.Skip();
    }


}