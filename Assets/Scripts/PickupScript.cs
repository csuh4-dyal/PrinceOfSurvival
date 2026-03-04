using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PickupScript : MonoBehaviour
{
    public Camera playerCam; // main player camera
    public GameObject myHands; //refer to the hands, where the rock is going
    public float pickupDistance = 3f; // distance to reach
    private bool canPickupObject; //true = can pick up, false = can't pick up
    private GameObject ObjectIwantToPickUp; // the game object you want to pick up
    private bool hasItem; // checks if you have item
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canPickupObject = false;
        hasItem = false;
    }

    // Update is called once per frame
    void Update()
    {
        CheckforItem();
        if(canPickupObject && !hasItem)
        {
            if (Input.GetKeyDown(KeyCode.E))  
            {
                GrabObject();
                Debug.Log ("E Pressed");
            }
        }
        if (hasItem == true) // if you have an item and get the key to remove the object, again can be any key
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                DropObject();
                Debug.Log ("Q Pressed");
            }
        } 
      
    }
    void CheckforItem()
    {
        Ray rayCast = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        
        if (Physics.Raycast(rayCast, out hit, pickupDistance))
        {
            if (hit.collider.CompareTag("Item"))
            {
                canPickupObject = true;
                ObjectIwantToPickUp = hit.collider.gameObject;
                Destroy(ObjectIwantToPickUp);
            }
            else
            {
                canPickupObject = false;
            }
        }
        else
        {
            canPickupObject = false;
        }
    }
    void GrabObject()
    {
        hasItem = true;
        Rigidbody rb = ObjectIwantToPickUp.GetComponent<Rigidbody>();
        
        rb.isKinematic = true; 
        // Snap to hands perfectly
        ObjectIwantToPickUp.transform.SetParent(myHands.transform); 
        ObjectIwantToPickUp.transform.localPosition = Vector3.zero;
        ObjectIwantToPickUp.transform.localRotation = Quaternion.identity;
    }
    void DropObject()
    {
        hasItem = false;
        Rigidbody rb = ObjectIwantToPickUp.GetComponent<Rigidbody>();
        
        rb.isKinematic = false;
        ObjectIwantToPickUp.transform.SetParent(null);
    }
        private void OnTriggerEnter(Collider other) // to see when the player enters the collider
    {
        if(other.gameObject.tag == "Item") //on the object you want to pick up set the tag to be anything, in this case "object"
        {
            canPickupObject = true;  //set the pick up bool to true
            ObjectIwantToPickUp = other.gameObject; //set the gameobject you collided with to one you can reference
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            canPickupObject = false; //when you leave the collider set the canpickup bool to false
        }
       
    }

}

