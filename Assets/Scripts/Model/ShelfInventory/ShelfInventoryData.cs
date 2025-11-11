using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory.Model;
using System.Linq;

public class ShelfInventoryData : MonoBehaviour
{
    private List<InventoryItemShelf> slots;

    //responsible for inventory size, can change in the Data folder (inspector)
    [field: SerializeField]
    public int Size { get; private set; } = 90;

    //this is an event that passes a dictionary as a parameter
    //passes when inventory is updated
    public event Action<Dictionary<int, InventoryItemShelf>> OnInventoryUpdatedShelf;

    //called in Inventory controller, creates the list of items stored here with the size of the inventory
    //adds empty items initially to fill in inventory
    public void Initialize()
    {
        slots = new List<InventoryItemShelf>();
        for (int i = 0; i < Size; i++)
        {
            slots.Add(InventoryItemShelf.GetEmptyItem());
        }
    }

    //This method adds an item to the inventory given the item data and amount of the item
    //no need to check for stackability
    public int AddItem(ItemSO item, int quantity)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            //while the item quantity is above 0 and if there are empty slots.
            while (quantity > 0 && IsInventoryFull() == false)
            {
                //The quantity should be one in this case (not stackable). 
                //When AddItemToFirstFreeSlot is ran in this case it returns 1.
                //Thus subtracting one from quantity and making it 0.
                quantity -= AddItemToFirstFreeSlot(item, 1);
            }

            //invokes the action OnInvetoryUpdated which alerts the inventory Controller
            InformAboutChange();
            return quantity;
        }
        print("error!");
        return -1;
    }

    //this is a loaded method
    //.Where branches off of the list of items (inventory) and searches through only the empty slots of the inventory (item.IsEmpty)
    //".Where(condition) -> returns an IEnumerable (filtered list)
    //.Any returns true if there's at least one element in it, so == false negates it
    //Return true if there are no empty slots — in other words, if the inventory is full.
    private bool IsInventoryFull()
        => slots.Where(item => item.IsEmpty).Any() == false;

    //UNSURE IF WE NEED THIS ONE HERE
    //Adds the given item to the first free slot in the inventory
    private int AddItemToFirstFreeSlot(ItemSO item, int quantity)
    {
        //create a new item (struct) to be added to the inventory (of structs)
        InventoryItemShelf newItem = new InventoryItemShelf
        {
            item = item,
            quantity = quantity
        };

        //go through the whole inventory
        //check in each interation if it's empty or not. If it is, place the new item in the empty slot
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].IsEmpty)
            {
                slots[i] = newItem;
                return quantity;
            }
        }
        return 0;
    }

    //referenced by the inventory controller to add items to the inventory
    public void AddItem(InventoryItemShelf item)
    {
        AddItem(item.item, item.quantity);
    }

    //return a list/dictionary of all inventory items that exist (not empty) in the inventory 
    public Dictionary<int, InventoryItemShelf> GetCurrentInventoryStateShelf()
    {
        Dictionary<int, InventoryItemShelf> returnValue =
            new Dictionary<int, InventoryItemShelf>();

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].IsEmpty)
                continue;
            returnValue[i] = slots[i];
        }
        return returnValue;
    }

    public InventoryItemShelf GetItemAt(int itemIndex)
    {
        return slots[itemIndex];
    }

    public void SwapItems(int itemIndex_1, int itemIndex_2)
    {
        Debug.Log("!");
        InventoryItemShelf item1 = slots[itemIndex_1];
        slots[itemIndex_1] = slots[itemIndex_2];
        slots[itemIndex_2] = item1;
        InformAboutChange();
    }

    private void InformAboutChange()
    {
        OnInventoryUpdatedShelf?.Invoke(GetCurrentInventoryStateShelf());
    }
}

[Serializable]
public struct InventoryItemShelf
{
    public int quantity;
    public ItemSO item;
    public bool IsEmpty => item == null;

    public InventoryItemShelf ChangeQuantity(int newQuantity)
    {
        return new InventoryItemShelf
        {
            item = this.item,
            quantity = newQuantity,
        };
    }

    public static InventoryItemShelf GetEmptyItem()
        => new InventoryItemShelf
        {
            item = null,
            quantity = 0,
        };
}

