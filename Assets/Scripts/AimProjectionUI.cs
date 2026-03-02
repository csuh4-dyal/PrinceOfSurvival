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

    void Start()
    {
    }
    void Update()
{
    // Always lock UI to screen center
    cursorUI.position = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

    // Ray from CENTER of screen
    Ray ray = playerCamera.ScreenPointToRay(
        new Vector3(Screen.width / 2f, Screen.height / 2f)
    );

    RaycastHit hit;
    Vector3 targetPoint;

    if (Physics.Raycast(ray, out hit, maxDistance))
        targetPoint = hit.point;
    else
        targetPoint = ray.GetPoint(maxDistance);

    Vector3 direction = (targetPoint - throwPoint.position).normalized;
    Vector3 startVelocity = direction * throwForce;

    Vector3 predictedPoint = PredictLanding(throwPoint.position, startVelocity);

    // Convert landing point to screen
    Vector3 screenPos = playerCamera.WorldToScreenPoint(predictedPoint);

    // Optional: if you want landing indicator separate,
    // use another UI element instead of cursorUI here.
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