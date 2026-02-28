using UnityEngine;
using UnityEngine.UI;

public class AimProjectionUI : MonoBehaviour
{
    public Camera playerCamera;
    public Transform throwPoint;
    public RectTransform cursorUI;

    public float throwForce = 20f;
    public float maxDistance = 100f;
    public float simulationTime = 3f;
    public float timeStep = 0.05f;

    void Update()
    {
        // Ray from screen center (since cursor is locked)
        Vector2 center = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = playerCamera.ScreenPointToRay(center);

        RaycastHit hit;
        Vector3 targetPoint;

        // Always get a target point
        if (Physics.Raycast(ray, out hit, maxDistance))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(maxDistance);

        // Calculate throw direction
        Vector3 direction = (targetPoint - throwPoint.position).normalized;
        Vector3 velocity = direction * throwForce;

        // Simulate projectile landing
        Vector3 predictedPoint = SimulateProjectile(throwPoint.position, velocity);

        // Convert world position to screen position
        Vector3 screenPos = playerCamera.WorldToScreenPoint(predictedPoint);

        // If behind camera, flip
        if (screenPos.z < 0)
            screenPos *= -1f;

        // Clamp to screen
        float clampedX = Mathf.Clamp(screenPos.x, 0f, Screen.width);
        float clampedY = Mathf.Clamp(screenPos.y, 0f, Screen.height);

        cursorUI.position = new Vector3(clampedX, clampedY, 0f);
    }

    Vector3 SimulateProjectile(Vector3 startPos, Vector3 startVelocity)
    {
        Vector3 position = startPos;
        Vector3 velocity = startVelocity;

        for (float t = 0; t < simulationTime; t += timeStep)
        {
            velocity += Physics.gravity * timeStep;
            position += velocity * timeStep;

            // Stop if we hit something
            if (Physics.Raycast(position, Vector3.down, 0.1f))
                return position;
        }

        return position;
    }
}