using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public List<Slot> allSlots = new List<Slot>(); // slot list, calls Slot script
    private Slot currentlySelectedSlot; //currently selected slot
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
   
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))SelectSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2))SelectSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3))SelectSlot(2);
    }
    void SelectSlot(int index) // select slot function
    {
        if (index < 0 || index >= allSlots.Count) return; // checks if slot exist

        if (currentlySelectedSlot != null)
        {
            currentlySelectedSlot.selectedObject = false;
        }
        // 2. Select the new slot
        currentlySelectedSlot = allSlots[index];
        currentlySelectedSlot.selectedObject = true;

        Debug.Log("Selected Slot: " + index);
        
        // 3. Tell the slot to update its visuals (like a highlight border)
        currentlySelectedSlot.UpdateSlot();
    }
}
