using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Lose Scene")]
    [SerializeField] private string loseSceneName = "LoseScene";

    [Header("Duck Taming")]
    public int ducksRequiredToUnlock = 10;
    public GameObject cavePath; // Path or door to unlock
    private int ducksTamed = 0;

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

    // Called by ducks automatically
    public void OnDuckTamed()
    {
        ducksTamed++;
        Debug.Log("Ducks tamed: " + ducksTamed + "/" + ducksRequiredToUnlock);

        if (ducksTamed >= ducksRequiredToUnlock)
        {
            UnlockCavePath();
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