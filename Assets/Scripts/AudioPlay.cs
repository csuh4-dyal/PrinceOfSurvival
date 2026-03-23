using UnityEngine;

public class AudioPlay : MonoBehaviour
{
    private AudioSource audioSource;
    // A boolean to check if the audio has played in the current run
    private bool hasPlayed = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get the AudioSource component attached to this GameObject
        audioSource = GetComponent<AudioSource>();

        // This is a simple way to make it play once at the start of the scene load
        // You can move this logic to a different trigger if needed (see section 3 below)
        if (!hasPlayed)
        {
            audioSource.Play();
            hasPlayed = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
