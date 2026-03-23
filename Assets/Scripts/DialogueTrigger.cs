using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [TextArea(2, 5)]
    public string[] dialogueLines;

    public bool playOnce = true;
    private bool hasPlayed = false;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (playOnce && hasPlayed) return;

        if (DialogueManager.Instance != null)
        {
            // Prevent overlapping dialogue
            if (!DialogueManager.Instance.IsDialogueActive())
            {
                DialogueManager.Instance.StartDialogue(dialogueLines);
                hasPlayed = true;
            }
        }
    }
}