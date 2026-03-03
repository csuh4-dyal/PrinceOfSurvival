using UnityEngine;

public class CoconutDropScript : MonoBehaviour
{
    public Rigidbody coconut; // coconut fruit
    public int hitsNeeded = 3; // hits needed for coconut 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        coconut = GetComponent<Rigidbody>();

        if (coconut != null)
        {
            coconut.useGravity = false;
            coconut.isKinematic = true;
        }

    }

    // Update is called once per frame
    void Update()
    {
    }
    void OnCollisionEnter(Collision collision)
    {
        TakeDamage();
    }
    void TakeDamage() {
    hitsNeeded--;
    if (hitsNeeded <= 0) {
        DropItem();
    }
}
    void DropItem() {
        if (coconut != null)
        {
            coconut.useGravity = true;
            coconut.isKinematic = false;
        }
    }
    
}
