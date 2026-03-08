using System.Linq;
using UnityEngine;

public class Racket : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public Transform holdPoint;
    public string[] targetTags = { "Food", "Rock", "Seed" };

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
            {
                TryPickup();
            }
            else
            {
                Debug.Log("Already holding an object: " + heldObject.name);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (heldObject != null)
            {
                Throw();
            }
            else
            {
                Debug.Log("No object held to throw.");
            }
        }
    }

    void TryPickup()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        float sphereRadius = 0.5f; // adjust for small seeds

        if (Physics.SphereCast(ray, sphereRadius, out hit, pickupDistance))
        {
            GameObject target = hit.collider.gameObject;
            if (targetTags.Contains(target.tag))
            {
                Debug.Log("Picked up: " + target.name);
                Pickup(target);
            }
            else
            {
                Debug.Log("Hit object, but tag not in targetTags: " + target.tag);
            }
        }
        else
        {
            Debug.Log("No object in range to pick up.");
        }
    }

    void Pickup(GameObject obj)
    {
        heldObject = obj;
        heldRb = obj.GetComponent<Rigidbody>();

        if (heldRb == null)
        {
            Debug.LogWarning("Picked object has no Rigidbody: " + obj.name);
            return;
        }

        heldRb.isKinematic = true;
        obj.transform.SetParent(holdPoint);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;

        Debug.Log("Object attached to holdPoint: " + obj.name);
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

        Debug.Log("Threw object: " + heldObject.name + " towards " + targetPoint);

        heldObject = null;
        heldRb = null;
    }
}