using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Slot : MonoBehaviour
{
    public bool selectedObject;
    public GameObject frameAlphaInc;

    private ItemSO heldItem;
    private int itemAmt;
    private Image iconImage;
    private TextMeshProUGUI amtText;

    private void Awake()
    {
        iconImage = transform.GetChild(0).GetComponent<Image>();
        amtText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    // ── Getters / Setters ─────────────────────────────────────

    public ItemSO GetItem() => heldItem;
    public int GetAmt() => itemAmt;
    public bool HasItem() => heldItem != null;

    public void SetItem(ItemSO item, int amt = 1)
    {
        heldItem = item;
        itemAmt = amt;
        UpdateSlot();
    }

    public void ClearSlot()
    {
        heldItem = null;
        itemAmt = 0;
        UpdateSlot();
    }

    public int AddAmt(int amtToAdd)
    {
        itemAmt += amtToAdd;
        UpdateSlot();
        return itemAmt;
    }

    public int RemoveAmount(int amtToRemove)
    {
        itemAmt -= amtToRemove;
        if (itemAmt <= 0)
            ClearSlot();
        else
            UpdateSlot();
        return itemAmt;
    }

    // ── Visual Update ─────────────────────────────────────────

    public void UpdateSlot()
    {
        // Safety: re-fetch refs if missing (e.g. called before Awake)
        if (iconImage == null)
        {
            iconImage = transform.GetChild(0).GetComponent<Image>();
            amtText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        }

        if (heldItem != null)
        {
            iconImage.enabled = true;
            iconImage.sprite = heldItem.icon;
            amtText.text = itemAmt.ToString();
        }
        else
        {
            iconImage.enabled = false;
            amtText.text = "";
        }

        if (frameAlphaInc != null)
        {
            Image frameImage = frameAlphaInc.GetComponent<Image>();
            if (frameImage != null)
            {
                Color c = frameImage.color;
                c.a = selectedObject ? 1f : 0.5f;
                frameImage.color = c;
            }
        }
    }
}