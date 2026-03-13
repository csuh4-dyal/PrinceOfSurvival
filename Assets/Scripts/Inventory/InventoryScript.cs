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

    private int equipptedHotBarIndex = 0; //0-3
    public float equippedOpacity = 0.9f;
    public float normalOpacity = 0.58f;

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
            AddItem(seedItem,1);
        }
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            container.SetActive(container.activeInHierarchy);
            UnityEngine.Cursor.lockState = UnityEngine.Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
            UnityEngine.Cursor.visible = !UnityEngine.Cursor.visible;
            
        }
        DetectLookedAtItem();
        Pickup();

        HandleHotBarSelection();
        HandleDropEquippedItem();
        UpdateHotbarOpacity();

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

    private void UpdateHotbarOpacity()
    {
        for(int i = 0; i < hotbarSlots.Count; i++)
        {
            Image icon = hotbarSlots[i].GetComponent<Image>();
            if(icon != null)
            {
                icon.tintColor = (i == equipptedHotBarIndex) ? new Color(1, 1, 1, equipptedHotBarIndex) : new Color(1, 1, 1, normalOpacity);

            }
        }
    }
    private void HandleHotBarSelection()
    {
        for (int i = 0; i<6; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                equipptedHotBarIndex = i;
                UpdateHotbarOpacity();

            }
        }
    }
    private void HandleDropEquippedItem()
    {
        if (!Input.GetKeyDown(KeyCode.Q)) return;

        Slot equippedSlot = hotbarSlots[equipptedHotBarIndex];

        if (!equippedSlot.HasItem()) return;

        ItemSO itemSO = equippedSlot.GetItem();
        GameObject prefab = itemSO.itemPrefab;

        if (prefab == null) return;

        GameObject dropped = Instantiate(prefab, Camera.main.transform.position + Camera.main.transform.forward, Quaternion.identity);

        Item item = dropped.GetComponent<Item>();
        item.item = itemSO;
        item.amount =equippedSlot.GetAmt();

        equippedSlot.ClearSlot();


    }
}
