using UnityEngine;

public class IntroFadeScene : MonoBehaviour, IIntro
{
    public CanvasGroup introCanvas;
    public float fadeDuration = 2f;
    public float holdTime = 2f;

    private bool skipRequested = false;

    void Start()
    {
        StartCoroutine(FadeOutIntro());
    }

    private System.Collections.IEnumerator FadeOutIntro()
    {
        // Stay on screen for a moment, but allow skipping
        float elapsedHold = 0f;
        while (elapsedHold < holdTime && !skipRequested)
        {
            elapsedHold += Time.deltaTime;
            yield return null;
        }

        // Fade out
        float t = 0f;
        float startAlpha = introCanvas.alpha;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            introCanvas.alpha = Mathf.Lerp(startAlpha, 0f, t / fadeDuration);
            yield return null;
        }

        // Disable intro UI after fade
        introCanvas.gameObject.SetActive(false);
    }

    public void Skip()
    {
        Debug.Log("Intro skipped!");
        skipRequested = true; // immediately break the hold loop
    }
}
