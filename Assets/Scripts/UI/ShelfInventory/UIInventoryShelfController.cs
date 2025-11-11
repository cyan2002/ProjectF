using Inventory.Model;
using Inventory.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventoryShelfController : MonoBehaviour
{
    [SerializeField]
    private UIInventoryPageShelf inventoryUI;

    [SerializeField]
    private ShelfInventoryData inventoryData;

    public List<InventoryItemShelf> initialItems = new List<InventoryItemShelf>();

    private void Start()
    {
        PrepareUI();
        PrepareInventoryData();
    }

    //prepares the UI by initializing the inventory size in the UI page script.
    //the UI page script then creates items that are added to the grid canvas (the item prefabs, to represent items they are disabled when empty and image is placed in holder when they have something in it)
    //The script then subscribes the inventory page a subscriber to the events. 
    private void PrepareUI()
    {
        inventoryUI.InitializeInventoryUI(inventoryData.Size);
        inventoryUI.ShelfOnSwapItems += HandleSwapItems;
        inventoryUI.ShelfOnStartDragging += HandleDragging;
        inventoryUI.ShelfOnItemActionRequested += HandleItemActionRequest;
    }

    //This updates the inventory SO, so where the data is kept for the objects in the inventory (not UI)
    //It them makes the scriptable object inventory a subscriber to the inventory update event
    //It then runs through each item in the initial items list (given by here) and adds it to the inventory data scriptable object
    private void PrepareInventoryData()
    {
        inventoryData.Initialize();
        inventoryData.OnInventoryUpdatedShelf += UpdateInventoryUI;
        foreach (InventoryItemShelf item in initialItems)
        {
            if (item.IsEmpty)
                continue;
            inventoryData.AddItem(item);
            Debug.Log(item);
        }
    }

    //This method plays when the items are swapped. - comes from UI input
    //This just tells the scriptable object inventory to swap the two items that need to be swapped.
    private void HandleSwapItems(int itemIndex_1, int itemIndex_2)
    {
        inventoryData.SwapItems(itemIndex_1, itemIndex_2);
    }

    //This method is played when the event OnStartDragging occurs
    //Given the item index of the item that's being dragged, it sets the MouseFollower to be on and appear as you are dragging the item
    private void HandleDragging(int itemIndex)
    {
        InventoryItemShelf inventoryItem = inventoryData.GetItemAt(itemIndex);
        if (inventoryItem.IsEmpty)
            return;
        inventoryUI.CreateDraggedItem(inventoryItem.item.ItemImage, inventoryItem.quantity);
    }

    private void HandleItemActionRequest(int itemIndex)
    {

    }

    //This method is triggered when the OnInventoryUpdated event occurs
    //the inventory UI is reset and for each item in the inventory state dictionary, it updates the UI for it
    private void UpdateInventoryUI(Dictionary<int, InventoryItemShelf> inventoryState)
    {
        inventoryUI.ResetAllItems();
        foreach (var item in inventoryState)
        {
            inventoryUI.UpdateData(item.Key, item.Value.item.ItemImage,
                item.Value.quantity);
        }
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (inventoryUI.isActiveAndEnabled == false)
            {
                inventoryUI.Show();
                //for each item in the inventoryData scriptable object, update each item in the UI
                foreach (var item in inventoryData.GetCurrentInventoryStateShelf())
                {
                    inventoryUI.UpdateData(item.Key,
                        item.Value.item.ItemImage,
                        item.Value.quantity);
                }
            }
            else
            {
                inventoryUI.Hide();
            }

        }
    }
}