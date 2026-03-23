using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI; // For fade panel

public class UltimateDuck : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource bgmSource;
    public AudioClip initialBGM;
    public AudioClip triggeredBGM;

    [Header("Duck Spawn")]
    public GameObject duckPrefab;
    public int duckCount = 5;
    public float spawnRadius = 5f;

    [Header("Scene")]
    public string winSceneName = "WinScene";

    [Header("UI")]
    public Image fadePanel; // Full-screen white Image for fade
    public float fadeDuration = 2f;

    [Header("Story Dialogue")]
    [TextArea(2, 5)]
    public string[] finalStoryLines;

    private bool triggered = false;

    void Start()
    {
        // Play initial BGM
        if (bgmSource != null && initialBGM != null)
        {
            bgmSource.clip = initialBGM;
            bgmSource.loop = true;
            bgmSource.Play();
        }

        // Make sure fade panel is transparent at start
        if (fadePanel != null)
        {
            fadePanel.color = new Color(1, 1, 1, 0);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;
            StartCoroutine(TriggerSequence(other.transform));
        }
    }

    IEnumerator TriggerSequence(Transform player)
    {
        // Change BGM
        if (bgmSource != null && triggeredBGM != null)
        {
            bgmSource.Stop();
            bgmSource.clip = triggeredBGM;
            bgmSource.loop = true;
            bgmSource.Play();
        }

        // Spawn ducks around player
        for (int i = 0; i < duckCount; i++)
        {
            Vector3 randomPos = player.position + Random.insideUnitSphere * spawnRadius;
            randomPos.y = player.position.y;
            Instantiate(duckPrefab, randomPos, Quaternion.identity);
        }

        // Wait a short moment before starting dialogue
        yield return new WaitForSeconds(5f);

        // Fade to white
        if (fadePanel != null)
        {
            yield return StartCoroutine(FadeToWhite());
        }

        // Load Win Scene
        SceneManager.LoadScene(winSceneName);
    }

    IEnumerator FadeToWhite()
    {
        float elapsed = 0f;
        Color startColor = fadePanel.color;
        Color targetColor = new Color(1, 1, 1, 1);

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadePanel.color = Color.Lerp(startColor, targetColor, elapsed / fadeDuration);
            yield return null;
        }

        fadePanel.color = targetColor;
    }
}