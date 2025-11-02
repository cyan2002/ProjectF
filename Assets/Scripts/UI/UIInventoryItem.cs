using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;

//THIS SCRIPT DETECTS INPUTS AND HOLDS THE EVENTS THEN TELLS THE SUBSCRIBERS OF THOSE EVENTS TO DO SOMETHING (IT DOESN'T KNOW WHAT)
//detects events but doesn't know what to do with events -> tells other scripts that events happen
public class UIInventoryItem : MonoBehaviour
{
    //image we are going to use to display our item
    [SerializeField]
    private Image itemImage;
    //the amount of item we have in our inventory
    [SerializeField]
    private TMP_Text quantityTxt;

    //used to enable and disable to show that we have selected our item or not
    [SerializeField]
    private Image borderImage;

    //used to drag arond items in inventory
    //action is a method used in unity where if an action happens it sends information somewhere
    //with the item that the action happened to
    public event Action<UIInventoryItem> OnItemClicked,
            OnItemDroppedOn, OnItemBeginDrag, OnItemEndDrag,
            OnRightMouseBtnClick;
    //OnItemClicked - left mouse button clicked (item select)
    //Right mouse button click - menu options

    //some of those events should not be called when item is empty (cannot drag an empty item)
    private bool empty = true;

    public void Awake()
    {
        ResetData();
        Deselect();
    }

    //detects when there is a right click and left click 
    //passes data of object but also which button was used to click on this object
    public void OnPointerClick(BaseEventData data)
    {
        PointerEventData pointerData = (PointerEventData)data;
        if (pointerData.button == PointerEventData.InputButton.Left)
        {
            OnItemClicked?.Invoke(this);
        }
        else if (pointerData.button == PointerEventData.InputButton.Right)
        {
            OnRightMouseBtnClick?.Invoke(this);
        }
    }

    //disables image - removes item from slot
    public void ResetData()
    {
        itemImage.gameObject.SetActive(false);
        empty = true;
    }

    //disables border
    public void Deselect()
    {
        borderImage.enabled = false;
    }

    //sets data about our item and how much we have
    public void SetData(Sprite sprite, int quantity)
    {
        itemImage.gameObject.SetActive(true);
        itemImage.sprite = sprite;
        quantityTxt.text = quantity + "";
        empty = false;
    }

    //on selection of our item, highlight it with the border
    public void Select()
    {
        borderImage.enabled = true;
    }

    //clicked and starting to drag event
    //if item is empty then return (nothing happens)
    //if there is an item tell another function about "this"
    public void OnBeginDrag()
    {
        if (empty)
            return;
        OnItemBeginDrag?.Invoke(this);
    }

    //when you drop an item on another item (inform UI)
    public void OnDrop()
    {
        OnItemDroppedOn?.Invoke(this);
    }

    public void OnEndDrag()
    {
        OnItemEndDrag?.Invoke(this);
    }
}
