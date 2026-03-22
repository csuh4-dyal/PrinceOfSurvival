using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
    [Header("Scene Settings")]
    public string sceneToLoad;

    [Header("Image Toggle Settings")]
    public GameObject imageObject;

    // Function 1: Toggle Image
    public void ToggleImage()
    {
        if (imageObject != null)
        {
            imageObject.SetActive(!imageObject.activeSelf);
        }
    }

    // Function 2: Load Scene
    public void LoadScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
