using UnityEngine;

public class IntroDialogue : MonoBehaviour
{
    [TextArea(2, 5)]
    public string[] introLines;

    void Start()
    {
        // Delay one frame to ensure everything is initialized
        StartCoroutine(PlayIntro());
    }

    System.Collections.IEnumerator PlayIntro()
    {
        yield return null;

        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(introLines);
        }
    }
}