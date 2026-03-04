using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.ComponentModel;
using JetBrains.Annotations;
public class Slot : MonoBehaviour
{
    public bool selectedObject;
    private ItemSO heldItem;
    private int itemAmt;
    private Image iconImage;
    private TextMeshProUGUI amtText;
    public GameObject frameAlphaInc;

    private void Awake()
    {
        iconImage = transform.GetChild(0).GetComponent<Image>();
        amtText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    public ItemSO GetItem()
    {
        return heldItem;
        // the purpose of this script : 
        // to act as a window for other scripts to access
        // since held item is a private component, we don't want to make things messy by making it public
        // so can still be accessed, but with a method
        // This is called a Getter Function
    }
    public int GetAmt()
    {
        return itemAmt;
        //show item amt
        // og itemAmt funct. is private int, this method is also a window
        // Getter Function
    }
    public void SetItem(ItemSO item, int amt = 1) //Setter Function
    {
        heldItem = item;
        itemAmt = amt;

        UpdateSlot(); // update slot function
    }
    public void UpdateSlot()
    {
        if(heldItem != null)
        {
            iconImage.enabled = true;
            iconImage.sprite = heldItem.icon;
            amtText.text = itemAmt.ToString();
        }
        else
        {
            iconImage.enabled = false; // can't see icon
            amtText.text = ""; //empty string
        }
        if (frameAlphaInc != null)
        {
            Image frameImage = frameAlphaInc.GetComponent<Image>();
            if (frameImage != null)
            {
                Color tempColor = frameImage.color;
                tempColor.a = selectedObject ? 1.0f : 0.5f;

                frameImage.color = tempColor;

            }
        }
    }
    public int AddAmt(int amtToAdd)
    {
        itemAmt += amtToAdd;
        UpdateSlot();
        return itemAmt; // to return
    }
    public int RemoveAmount (int amtToRemove)
    {
        itemAmt -= amtToRemove;
        if(itemAmt <=0)
        {
            ClearSlot();
        }
        else
        {
            UpdateSlot();
        }
        return itemAmt; // to return value
    }
    public void ClearSlot()
    {
        heldItem = null;
        itemAmt = 0;
        UpdateSlot();
    }
    public bool HasItem()
    {
        return heldItem != null; // check if we have item and retuns if we have item
    }
}
