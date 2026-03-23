using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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
    public string sceneToLoad = "MainScene";
    public float delayBeforeSceneLoad = 5f;

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

        // Wait before loading next scene
        yield return new WaitForSeconds(delayBeforeSceneLoad);

        // Load scene
        SceneManager.LoadScene(sceneToLoad);
    }
}
