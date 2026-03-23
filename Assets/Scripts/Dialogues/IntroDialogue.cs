using UnityEngine;
using System.Collections;

public class IntroDialogue : MonoBehaviour
{
    public AudioSource soundToPlay;

    [TextArea(2, 5)]
    public string[] introLines;

    void Start()
    {
        soundToPlay = GetComponent<AudioSource>();
        StartCoroutine(PlayIntro());
    }

    IEnumerator PlayIntro()
    {
        yield return null;

        if (DialogueManager.Instance != null)
        {
            // Start dialogue
            DialogueManager.Instance.StartDialogue(introLines);

            // Wait until dialogue finishes
            yield return new WaitUntil(() => !DialogueManager.Instance.IsDialogueActive());

            // Now play BGM
            if (soundToPlay != null)
            {
                soundToPlay.Play();
            }
        }
    }
}