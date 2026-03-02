using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PickupScript : MonoBehaviour
{
    public GameObject myHands; //refer to the hands, where the rock is going
    bool canPickupObject; //true = can pick up, false = can't pick up
    GameObject ObjectIwantToPickUp; // the game object you want to pick up
    bool hasItem; // checks if you have item
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canPickupObject = false;
        hasItem = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(canPickupObject == true)
        {
            if (Input.GetKeyDown("e"))  
            {
                ObjectIwantToPickUp.GetComponent<Rigidbody>().isKinematic = true;   //makes the rigidbody not be acted upon by forces
                ObjectIwantToPickUp.transform.position = myHands.transform.position; // sets the position of the object to your hand position
                ObjectIwantToPickUp.transform.parent = myHands.transform; //makes the object become a child of the parent so that it moves with the hands
            }
        }
        if (Input.GetButtonDown("q") && hasItem == true) // if you have an item and get the key to remove the object, again can be any key
        {
            ObjectIwantToPickUp.GetComponent<Rigidbody>().isKinematic = false; // make the rigidbody work again
            ObjectIwantToPickUp.transform.parent = null; // make the object no be a child of the hands
        }   
      
    }
        private void OnTriggerEnter(Collider other) // to see when the player enters the collider
    {
        if(other.gameObject.tag == "object") //on the object you want to pick up set the tag to be anything, in this case "object"
        {
            canPickupObject = true;  //set the pick up bool to true
            ObjectIwantToPickUp = other.gameObject; //set the gameobject you collided with to one you can reference
        }
    }
    private void OnTriggerExit(Collider other)
    {
        canPickupObject = false; //when you leave the collider set the canpickup bool to false
     
    }

}

