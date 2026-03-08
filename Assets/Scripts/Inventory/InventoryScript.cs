using UnityEngine;
using System.Collections.Generic;
using System;

public class InventoryScript : MonoBehaviour
{
    public ItemSO rockItem;
    public ItemSO seedItem;
    public ItemSO coconutItem;

  public GameObject hotbarObj;
  private List<Slot> hotbarSlots = new List<Slot>();
  private List<Slot> allSlots = new List<Slot>();

  private void Awake()
    {
        hotbarSlots.AddRange(hotbarObj.GetComponentsInChildren<Slot>());    

        allSlots.AddRange(hotbarSlots);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            //AddItem();
        }
    }
    public void AddItem(ItemSO itemToAdd, int amount)
    {
        int remaining = amount;
        foreach(Slot slot in allSlots)
        {
            if (slot.HasItem() && slot.GetItem() == itemToAdd)
            {
                int currentAmount = slot.GetAmt();
                int maxStack = itemToAdd.maxStackSize;
                    
                    if(currentAmount < maxStack)
                    {
                    int spaceLeft = maxStack - currentAmount;
                    int amountToAdd = Mathf.Min(spaceLeft, remaining);

                    slot.SetItem(itemToAdd, currentAmount + amountToAdd);
                    remaining -= amountToAdd;

                    if(remaining <= 0)
                        return;
                    }
            }
        }

        foreach(Slot slot in allSlots)
        {
            if (!slot.HasItem())
            {
                int amountToPlace = Mathf.Min(itemToAdd.maxStackSize, remaining);
                slot.SetItem(itemToAdd, amountToPlace);

                if (remaining <=0)
                    return;

            }        
        }

        if (remaining > 0)
        {
            Debug.Log("Inventory is full, could not add" + remaining + " of " + itemToAdd.ItemName); 
        }
    }
}
