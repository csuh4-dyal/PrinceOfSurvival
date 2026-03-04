using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private string loseSceneName = "LoseScene";

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
}