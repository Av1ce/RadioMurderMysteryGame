using UnityEngine;

[System.Serializable]
public class Dialogue
{
    public string name;

    [TextArea(4, 8)]
    public string[] sentences;

}
