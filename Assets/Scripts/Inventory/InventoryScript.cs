using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UIElements;

public class InventoryScript : MonoBehaviour
{
    public ItemSO rockItem;
    public ItemSO seedItem;
    public ItemSO coconutItem;

    public GameObject hotbarObj;
    public GameObject container;

    public float pickupRange = 3f;
    private Item lookedAtItem = null;
    public Material highlightMaterial;
    private Material originalMaterial;
    private Renderer lookedAtRenderer = null;

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
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            container.SetActive(container.activeInHierarchy);
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = !Cursor.visible;

        }

    }
    public void AddItem(ItemSO itemToAdd, int amount)
    {
        int remaining = amount;
        foreach (Slot slot in allSlots)
        {
            if (slot.HasItem() && slot.GetItem() == itemToAdd)
            {
                int currentAmount = slot.GetAmt();
                int maxStack = itemToAdd.maxStackSize;

                if (currentAmount < maxStack)
                {
                    int spaceLeft = maxStack - currentAmount;
                    int amountToAdd = Mathf.Min(spaceLeft, remaining);

                    slot.SetItem(itemToAdd, currentAmount + amountToAdd);
                    remaining -= amountToAdd;

                    if (remaining <= 0)
                        return;
                }
            }
        }

        foreach (Slot slot in allSlots)
        {
            if (!slot.HasItem())
            {
                int amountToPlace = Mathf.Min(itemToAdd.maxStackSize, remaining);
                slot.SetItem(itemToAdd, amountToPlace);

                if (remaining <= 0)
                    return;

            }
        }

        if (remaining > 0)
        {
            Debug.Log("Inventory is full, could not add" + remaining + " of " + itemToAdd.ItemName);
        }
    }
        private void Pickup()
    {
        if (lookedAtRenderer != null && Input.GetKeyDown(KeyCode.E))
        {
            Item item = lookedAtRenderer.GetComponent<Item>();
            if (item != null)
            {
                AddItem(item.item, item.amount);
                Destroy(item.gameObject);
            }
        }
    }
        private void DetectLookedAtItem()
    {
        if(lookedAtRenderer != null)
        {
            lookedAtRenderer.material = originalMaterial;
            lookedAtRenderer = null;   
            originalMaterial = null;    
        }
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange))
        {
            Item item = hit.collider.GetComponent<Item>();  
            if (item != null)
            {
                Renderer rend = item.GetComponent<Renderer>();
                if (rend != null)
                {
                    originalMaterial = rend.material;
                    rend.material = highlightMaterial;
                    lookedAtRenderer = rend;
                }
            }
        }
    }
}
