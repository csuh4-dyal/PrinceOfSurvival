using UnityEngine;
using UnityEngine.UI;

public class AimProjectionUI : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public Transform throwPoint;
    public RectTransform cursorUI;

    [Header("Throw Settings (MATCH RACKET)")]
    public float throwForce = 20f;
    public float maxDistance = 100f;

    [Header("Prediction")]
    public float simulationTime = 3f;
    public float timeStep = 0.05f;

    void Update()
    {
        cursorUI.position = Input.mousePosition;
        // Use same ray logic as Racket
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit, maxDistance))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(maxDistance);

        Vector3 direction = (targetPoint - throwPoint.position).normalized;
        Vector3 startVelocity = direction * throwForce;

        // Predict landing
        Vector3 predictedPoint = PredictLanding(throwPoint.position, startVelocity);

        // Convert to screen position
        Vector3 screenPos = playerCamera.WorldToScreenPoint(predictedPoint);

        // If behind camera, clamp to edge
        if (screenPos.z < 0)
        {
            screenPos.x = Screen.width - screenPos.x;
            screenPos.y = Screen.height - screenPos.y;
        }

        // Clamp INSIDE screen (account for cursor size)
        float halfW = cursorUI.rect.width * 0.5f;
        float halfH = cursorUI.rect.height * 0.5f;

        float clampedX = Mathf.Clamp(screenPos.x, halfW, Screen.width - halfW);
        float clampedY = Mathf.Clamp(screenPos.y, halfH, Screen.height - halfH);

        cursorUI.position = new Vector3(clampedX, clampedY, 0f);
    }

    Vector3 PredictLanding(Vector3 startPos, Vector3 startVelocity)
    {
        Vector3 position = startPos;
        Vector3 velocity = startVelocity;

        for (float t = 0; t < simulationTime; t += timeStep)
        {
            velocity += Physics.gravity * timeStep;
            position += velocity * timeStep;

            if (Physics.Raycast(position, Vector3.down, 0.1f))
                return position;
        }

        return position;
    }
    
}