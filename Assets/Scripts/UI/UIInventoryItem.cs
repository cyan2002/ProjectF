using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;

namespace Inventory.UI
{

    //THIS SCRIPT DETECTS INPUTS AND HOLDS THE EVENTS THEN TELLS THE SUBSCRIBERS OF THOSE EVENTS TO DO SOMETHING (IT DOESN'T KNOW WHAT)
    //detects events but doesn't know what to do with events -> tells other scripts that events happen
    //This script is LOCATED ON EACH ITEM includes behavior for each item
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
        //see below for further explainations of events and etc
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
        //turns image on to show item in the inventory
        //also sets the quantity
        public void SetData(Sprite sprite, int quantity)
        {
            itemImage.gameObject.SetActive(true);
            itemImage.sprite = sprite;
            quantityTxt.text = quantity + "";
            empty = false;
        }

        //on selection of our item, highlight it with the border
        //called from UIInventoryPage
        public void Select()
        {
            borderImage.enabled = true;
        }

        //THIS EVENT IS NOT A BUILT-IN METHOD BY UNITY. connected via EventTrigger
        //It would be if I used the IdragHandler on the class, but I did not (cause it did not work for me)
        //This is connected to a component in the inspector called "Event Trigger". What this event trigger can do is basically take those built in functions in unity and apply them to your own methods
        //This avoids the IDragHandler addition to the class (slower, but easier for me to understand)
        //This is connected to Begin drag event in Event Trigger. Therefore it is called when the item UI (UI element) is dragged. ONLY WHEN THIS SPECIFIC UI ELEMENT IS DRAGGED
        //If called and it's not on an empty object, the OnItemBegin Drag is invoked passing this item UI object
        public void OnBeginDrag()
        {
            if (empty)
                return;
            OnItemBeginDrag?.Invoke(this);
        }

        //THIS EVENT IS NOT A BUILT-IN METHOD BY UNITY. connected via EventTrigger
        //Drop is the event that OnDrop is connected to.
        //This event is called whenever you have another object being dragged and released over it (using unity's drag system)
        //when the item is dropped OVER ANOTHER ITEM it triggers the OnItemDroppedOn event.
        //Only triggers when dropped over another item - meaning it only drops when its over another object that can receive the drop - meaning only drops when its over another item with the TriggerEvent (or IdragHandler etc etc)
        //It then invokes the OnItemDropped sending the object (item) itself
        public void OnDrop()
        {
            OnItemDroppedOn?.Invoke(this);
        }

        //THIS EVENT IS NOT A BUILT-IN METHOD BY UNITY. connected via EventTrigger
        //Connected to the end drag event.
        //plays when the drag has ended regardless of another object is there or not.
        //It then invokes the OnItemEndDrag Action sending the object (item) itself
        public void OnEndDrag()
        {
            OnItemEndDrag?.Invoke(this);
        }
    }
}