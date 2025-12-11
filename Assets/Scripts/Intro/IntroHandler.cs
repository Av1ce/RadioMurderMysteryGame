using UnityEngine;

public class IntroHandler : IIntro
{
    public void Skip()
    {
        Debug.Log("Intro skipped!");
        // Add any extra logic here when skipping the intro
    }
}