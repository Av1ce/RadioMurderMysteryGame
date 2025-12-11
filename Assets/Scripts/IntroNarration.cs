using TMPro;
using UnityEngine;

public class IntroNarration : MonoBehaviour
{
    public AudioSource narrationSource;
    public TMP_Text introText;
    public float fadeDuration = 2f; // how long the text takes to brighten

    void Start()
    {
        // Start narration
        if (narrationSource != null)
            narrationSource.Play();

        // Start fading text
        StartCoroutine(FadeTextToWhite());
    }

    private System.Collections.IEnumerator FadeTextToWhite()
    {
        Color startColor = new Color(1, 1, 1, 0);   // transparent white
        Color endColor = new Color(1, 1, 1, 1);     // full white

        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float lerp = t / fadeDuration;

            introText.color = Color.Lerp(startColor, endColor, lerp);
            yield return null;
        }

        introText.color = endColor;
    }
}
