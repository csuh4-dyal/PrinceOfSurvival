using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform playerBody; // Reference to the Player's Transform
    public Vector3 offset;      // Optional offset for camera position
    public float mouseSensitivity = 100f;
    float xRotation = 0f;
    public float verticalMultiplier = 1.5f;

    void Start()
    {
        // Lock cursor to the center of the screen and hide it
        Cursor.lockState = CursorLockMode.Locked;
    }


    // Use LateUpdate to ensure the player has moved before the camera updates position
    void LateUpdate()
    {
        // Set the camera's position to the player's position plus the offset
        transform.position = playerBody.position + offset;
    }
    void Update()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * verticalMultiplier * Time.deltaTime;

        // Calculate vertical rotation and clamp it so you can't flip upside down
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Apply rotations
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); // Camera tilts up/down
        playerBody.Rotate(Vector3.up * mouseX); // Player body turns left/right
    }
}


