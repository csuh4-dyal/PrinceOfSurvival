using UnityEngine;

public class Racket : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public Transform holdPoint;

    [Header("Settings")]
    public float pickupDistance = 3f;
    public float throwForce = 20f;
    public float maxDistance = 100f;

    private GameObject heldObject;
    private Rigidbody heldRb;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldObject == null)
                TryPickup();
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (heldObject != null)
                Throw();
        }
    }

    void TryPickup()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupDistance))
        {
            if (hit.collider.CompareTag("Throwable"))
            {
                Pickup(hit.collider.gameObject);
            }
        }
    }

    void Pickup(GameObject obj)
    {
        heldObject = obj;
        heldRb = obj.GetComponent<Rigidbody>();

        heldRb.isKinematic = true;
        obj.transform.SetParent(holdPoint);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
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

        Vector3 direction = (targetPoint - holdPoint.position).normalized;

        heldObject.transform.SetParent(null);

        heldRb.isKinematic = false;
        heldRb.linearVelocity = direction * throwForce;

        heldObject = null;
        heldRb = null;
    }
}