using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management

public class SceneTeleporter : MonoBehaviour
{
    // Field to set the destination scene's name in the Inspector
    public string sceneToLoad;

    // This function is called when a collider enters the trigger
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the player
        if (other.CompareTag("Player"))
        {
            // Load the specified scene by name
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
