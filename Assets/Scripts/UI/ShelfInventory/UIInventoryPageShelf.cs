using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventoryPageShelf : MonoBehaviour
{
    [SerializeField]
    private UIInventoryItemShelf itemPrefab;

    [SerializeField]
    private RectTransform contentPanel;

    //[SerializeField]
    //private UIInventoryDescription itemDescription;

    [SerializeField]
    private MouseFollowerShelf mouseFollower;

    //list of items that the script keeps track of so that it can map out the inventory UI appropriately
    List<UIInventoryItemShelf> listOfUIItems = new List<UIInventoryItemShelf>();

    private int currentlyDraggedItemIndex = -1;

    public event Action<int> ShelfOnDescriptionRequested,
            ShelfOnItemActionRequested,
            ShelfOnStartDragging;

    public event Action<int, int> ShelfOnSwapItems;

    //hides the page and toggles the mouse follower (dragging) to false
    private void Awake()
    {
        Hide();
        //mouseFollower.Toggle(false);
        //itemDescription.ResetDescription();
    }

    //Called by the inventory controller, given a size creates the item prefab slots for where items could go. 
    //Basically creates all the item slots or item prefabs in the inventory for the UI.
    //Then subscribes to each of those items events
    //It also sets the parent of the object so that they remain in that grid formation and uniform.
    //Also adds to the list of UI inventory items that it keeps track of for the UI
    public void InitializeInventoryUI(int inventorysize)
    {
        for (int i = 0; i < inventorysize; i++)
        {
            UIInventoryItemShelf uiItem =
    Instantiate(itemPrefab, contentPanel, false); // <-- important!
            listOfUIItems.Add(uiItem);
            uiItem.ShelfOnItemBeginDrag += HandleBeginDrag;
            uiItem.ShelfOnItemDroppedOn += HandleSwap;
            uiItem.ShelfOnItemEndDrag += HandleEndDrag;
        }
    }

    //resets all items in the inventory so that the image and border of the item is disabled.
    internal void ResetAllItems()
    {
        foreach (var item in listOfUIItems)
        {
            item.ResetData();
        }
    }

    //updates the description of the item - NOT IN USE
    internal void UpdateDescription(int itemIndex, Sprite itemImage, string name, string description)
    {
        //itemDescription.SetDescription(itemImage, name, description);
        DeselectAllItems();
    }

    //Updates the UI for a specific item given the index, sprite and quantity
    public void UpdateData(int itemIndex,
        Sprite itemImage, int itemQuantity)
    {
        if (listOfUIItems.Count > itemIndex)
        {
            listOfUIItems[itemIndex].SetData(itemImage, itemQuantity);
        }
    }


    //subscriber to the end drag event. Just resets the dragged item (toggles mouse follower off and makes the index -1)
    private void HandleEndDrag(UIInventoryItemShelf inventoryItemUI)
    {
        ResetDraggedItem();
    }

    //subscriber to the OnItemDroppedOn event - when an object UI is dragged and released over another object (using unity's drag system)
    //When this happens, if the index is valid (not -1), the OnSwapItems action is evoked and the currently dragged index and the index to swap is passed
    //the handleItemselection method is then called with the item 
    private void HandleSwap(UIInventoryItemShelf inventoryItemUI)
    {
        int index = listOfUIItems.IndexOf(inventoryItemUI);
        if (index == -1)
        {
            return;
        }
        ShelfOnSwapItems?.Invoke(currentlyDraggedItemIndex, index);
        HandleItemSelection(inventoryItemUI);
    }

    //This just resets the dragged item. Disables mouseFollower and makes the index -1 indicating there is no dragged item currently
    private void ResetDraggedItem()
    {
        mouseFollower.Toggle(false);
        currentlyDraggedItemIndex = -1;
    }

    //Subscriber to OnItemBeginDrag event (from each item inventory slot)
    //is passed the item prefab
    //if it's a valid index, set the currently dragged index to that index of the item and evoke handleitemselection method
    //Also evoke the OnStartDragging Action while passing the index of the item being dragged
    private void HandleBeginDrag(UIInventoryItemShelf inventoryItemUI)
    {
        int index = listOfUIItems.IndexOf(inventoryItemUI);
        if (index == -1)
            return;
        currentlyDraggedItemIndex = index;
        HandleItemSelection(inventoryItemUI);
        ShelfOnStartDragging?.Invoke(index);
    }

    //This just makes the dragged item appearance by toggling the mouseFollower and setting the sprite and quantity of it
    public void CreateDraggedItem(Sprite sprite, int quantity)
    {
        mouseFollower.Toggle(true);
        mouseFollower.SetData(sprite, quantity);
    }

    //subscriber to the OnItemClicked event (from each item inventory slot)
    //is passed the item prefab
    //If it's a valid index, invoke the description requested passing the current index of the item
    //NOT IN USE CURRENTLY (description stuff)
    private void HandleItemSelection(UIInventoryItemShelf inventoryItemUI)
    {
        int index = listOfUIItems.IndexOf(inventoryItemUI);
        if (index == -1)
            return;
        ShelfOnDescriptionRequested?.Invoke(index);
    }

    //Sets the UI for the inventory on and deselects all items (removes borders)
    public void Show()
    {
        gameObject.SetActive(true);
        ResetSelection();
    }

    //disables borders and resets item descriptions (which is not in use)
    public void ResetSelection()
    {
        //itemDescription.ResetDescription();
        DeselectAllItems();
    }

    //for each item prefab, deselect the border to reset the sleection.
    private void DeselectAllItems()
    {
        foreach (UIInventoryItemShelf item in listOfUIItems)
        {
            //item.Deselect();
        }
    }

    //hide this object and reset all the item selections.
    public void Hide()
    {
        gameObject.SetActive(false);
        ResetDraggedItem();
    }
}