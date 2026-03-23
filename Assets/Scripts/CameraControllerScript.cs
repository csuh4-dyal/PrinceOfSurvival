using UnityEngine;

public class CameraControllerScript : MonoBehaviour
{
    [Header("Camera Settings")]
    public float mouseSensitivity = 200f;
    public Transform player; // Player object for movement
    public float fadeDistance = 1.5f; // distance to start fading
    public float minAlpha = 0.2f;     // minimum alpha when fully faded

    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Vertical Clamp")]
    public float minVerticalAngle = -90f;
    public float maxVerticalAngle = 90f;

    private float xRotation = 0f;
    private float yRotation = 0f;
    private Renderer[] playerRenderers;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Vector3 euler = transform.rotation.eulerAngles;
        xRotation = euler.x;
        yRotation = euler.y;

        if (player != null)
            playerRenderers = player.GetComponentsInChildren<Renderer>();
    }

    void Update()
    {
        HandleMouseRotation();
        HandlePlayerMovement();
        HandlePlayerFading();
    }

    void HandleMouseRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }

    void HandlePlayerMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 move = transform.forward * vertical + transform.right * horizontal;
        move.y = 0f; // lock player to ground
        player.position += move * moveSpeed * Time.deltaTime;
    }

    void HandlePlayerFading()
    {
        if (player == null || playerRenderers == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        float alpha = 1f;

        if (distance < fadeDistance)
        {
            alpha = Mathf.Lerp(minAlpha, 1f, distance / fadeDistance);
        }

        foreach (Renderer r in playerRenderers)
        {
            foreach (Material mat in r.materials)
            {
                Color c = mat.color;
                c.a = alpha;
                mat.color = c;

                // Enable transparency
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = 3000;
            }
        }
    }
}