using UnityEngine;

public class Racket : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public Transform throwPoint;
    public GameObject throwablePrefab;

    [Header("Throw Settings")]
    public float throwForce = 20f;
    public float maxDistance = 100f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Throw();
        }
    }

    void Throw()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(maxDistance);
        }

        Vector3 direction = (targetPoint - throwPoint.position).normalized;

        GameObject obj = Instantiate(
            throwablePrefab,
            throwPoint.position,
            Quaternion.identity
        );

        Rigidbody rb = obj.GetComponent<Rigidbody>();

        rb.linearVelocity = direction * throwForce;
    }
}