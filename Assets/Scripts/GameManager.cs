using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Use TextMeshPro for better UI text

public class GameManager : MonoBehaviour
{
    [Header("Lose Scene")]
    [SerializeField] private string loseSceneName = "Lose";

    [Header("Duck Taming")]
    public int ducksRequiredToUnlock = 10;
    public GameObject cavePath; // Path or door to unlock
    private int ducksTamed = 0;

    [Header("UI")]
    public TMP_Text duckCounterText; // Assign in inspector

    void Start()
    {
        UpdateDuckUI();

        if (cavePath != null)
            cavePath.SetActive(false); // initially locked
    }

    void OnEnable()
    {
        HungerBar.OnPlayerStarved += HandleGameOver;
    }

    void OnDisable()
    {
        HungerBar.OnPlayerStarved -= HandleGameOver;
    }

    void HandleGameOver()
    {
        SceneManager.LoadScene(loseSceneName);
    }

    // Called by DuckAI when a duck is tamed
    public void OnDuckTamed()
    {
        ducksTamed++;
        UpdateDuckUI();

        Debug.Log("Ducks tamed: " + ducksTamed + "/" + ducksRequiredToUnlock);

        if (ducksTamed >= ducksRequiredToUnlock)
            UnlockCavePath();
    }

    void UpdateDuckUI()
    {
        if (duckCounterText != null)
        {
            duckCounterText.text = ducksTamed + " / " + ducksRequiredToUnlock;
        }
    }

    void UnlockCavePath()
    {
        if (cavePath != null && !cavePath.activeSelf)
        {
            cavePath.SetActive(true);
            Debug.Log("Cave path unlocked! The ultimate duck awaits!");
        }
    }
}