using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryScript : MonoBehaviour
{
    [Header("Item Definitions")]
    public ItemSO rockItem;
    public ItemSO seedItem;
    public ItemSO coconutItem;

    [Header("UI")]
    public GameObject hotbarObj;
    public GameObject container;

    [Header("Pickup")]
    public float pickupRange = 3f;
    public Material highlightMaterial;

    [Header("Hotbar Opacity")]
    public float equippedOpacity = 0.9f;
    public float normalOpacity = 0.58f;

    private List<Slot> hotbarSlots = new List<Slot>();
    private List<Slot> allSlots = new List<Slot>();

    private int equippedHotbarIndex = 0;

    private Renderer lookedAtRenderer;
    private Material originalMaterial;

    private void Awake()
    {
        hotbarSlots.AddRange(hotbarObj.GetComponentsInChildren<Slot>());
        allSlots.AddRange(hotbarSlots);
    }

    void Update()
    {
        HandleHotbarSelection();
        DetectLookedAtItem();
        Pickup();
        HandleDropEquippedItem();
        UpdateHotbarOpacity();
        HandleInventoryToggle();
    }

    // ── Hotbar ────────────────────────────────────────────────

    private void HandleHotbarSelection()
    {
        for (int i = 0; i < hotbarSlots.Count; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
                SelectSlot(i);
        }
    }

    private void SelectSlot(int index)
    {
        if (index < 0 || index >= hotbarSlots.Count) return;

        equippedHotbarIndex = index;
        UpdateHotbarOpacity();
        Debug.Log("Selected Slot: " + index);
    }

    private void UpdateHotbarOpacity()
    {
        for (int i = 0; i < hotbarSlots.Count; i++)
        {
            Image icon = hotbarSlots[i].GetComponent<Image>();
            if (icon != null)
                icon.color = new Color(1, 1, 1, i == equippedHotbarIndex ? equippedOpacity : normalOpacity);
        }
    }

    // ── Inventory Toggle ──────────────────────────────────────

    private void HandleInventoryToggle()
    {
        if (!Input.GetKeyDown(KeyCode.Tab)) return;

        bool isOpen = !container.activeInHierarchy;
        container.SetActive(isOpen);
        Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isOpen;
    }

    // ── Pickup ────────────────────────────────────────────────

    private void DetectLookedAtItem()
    {
        // Clear previous highlight
        if (lookedAtRenderer != null)
        {
            lookedAtRenderer.material = originalMaterial;
            lookedAtRenderer = null;
            originalMaterial = null;
        }

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (!Physics.Raycast(ray, out RaycastHit hit, pickupRange)) return;

        Item item = hit.collider.GetComponent<Item>();
        if (item == null) return;

        Renderer rend = item.GetComponent<Renderer>();
        if (rend == null) return;

        originalMaterial = rend.material;
        rend.material = highlightMaterial;
        lookedAtRenderer = rend;
    }

    private void Pickup()
    {
        if (lookedAtRenderer == null || !Input.GetKeyDown(KeyCode.E)) return;

        Item item = lookedAtRenderer.GetComponent<Item>();
        if (item == null) return;

        AddItem(item.item, item.amount);
        Destroy(item.gameObject);
    }

    // ── Drop ──────────────────────────────────────────────────

    private void HandleDropEquippedItem()
    {
        if (!Input.GetKeyDown(KeyCode.Q)) return;

        Slot slot = hotbarSlots[equippedHotbarIndex];
        if (!slot.HasItem()) return;

        ItemSO itemSO = slot.GetItem();
        if (itemSO.itemPrefab == null) return;

        Vector3 dropPos = Camera.main.transform.position + Camera.main.transform.forward;
        GameObject dropped = Instantiate(itemSO.itemPrefab, dropPos, Quaternion.identity);

        Item droppedItem = dropped.GetComponent<Item>();
        if (droppedItem != null)
        {
            droppedItem.item = itemSO;
            droppedItem.amount = slot.GetAmt();
        }

        slot.ClearSlot();
    }

    // ── Add Item ──────────────────────────────────────────────

    public void AddItem(ItemSO itemToAdd, int amount)
    {
        int remaining = amount;

        // Pass 1: fill existing stacks
        foreach (Slot slot in allSlots)
        {
            if (!slot.HasItem() || slot.GetItem() != itemToAdd) continue;

            int space = itemToAdd.maxStackSize - slot.GetAmt();
            if (space <= 0) continue;

            int toAdd = Mathf.Min(space, remaining);
            slot.SetItem(itemToAdd, slot.GetAmt() + toAdd);
            remaining -= toAdd;

            if (remaining <= 0) return;
        }

        // Pass 2: fill empty slots
        foreach (Slot slot in allSlots)
        {
            if (slot.HasItem()) continue;

            int toPlace = Mathf.Min(itemToAdd.maxStackSize, remaining);
            slot.SetItem(itemToAdd, toPlace);
            remaining -= toPlace;

            if (remaining <= 0) return;
        }

        if (remaining > 0)
            Debug.Log($"Inventory full — could not add {remaining} of {itemToAdd.ItemName}");
    }
}