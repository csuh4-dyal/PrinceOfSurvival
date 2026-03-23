using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance; // global access

    [Header("UI")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public CanvasGroup canvasGroup;
    [Header("Fade Settings")]
    public float fadeDuration = 0.5f;

    private string[] currentLines;
    private int currentIndex;
    private bool isActive = false;

    void Awake()
    {
        // Singleton setup
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        if (!isActive) return;

        if (Input.GetMouseButtonDown(0))
        {
            NextLine();
        }
    }
    public void StartDialogue(string[] lines)
    {
        if (lines == null || lines.Length == 0) return;

        currentLines = lines;
        currentIndex = 0;
        isActive = true;

        dialoguePanel.SetActive(true);
        ShowLine();

        StartCoroutine(FadeIn());

        Time.timeScale = 0f;
    }

    void ShowLine()
    {
        dialogueText.text = currentLines[currentIndex];
    }

    void NextLine()
    {
        currentIndex++;

        if (currentIndex >= currentLines.Length)
        {
            EndDialogue();
        }
        else
        {
            ShowLine();
        }
    }

    void EndDialogue()
    {
        StartCoroutine(FadeOut());
    }

    public bool IsDialogueActive()
    {
        return isActive;
    }

    IEnumerator FadeIn()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;

        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = t / fadeDuration;
            yield return null;
        }

        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    IEnumerator FadeOut()
    {
        canvasGroup.interactable = false;

        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = 1 - (t / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0;
        isActive = false;

        Time.timeScale = 1f;
    }
}