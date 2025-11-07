using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Inventory.Model
{
    [CreateAssetMenu]
    public class InventorySO : ScriptableObject
    {
        //list of the struct data structure that carries information about each item
        //this is the inventory data
        [SerializeField]
        private List<InventoryItem> inventoryItems;

        //responsible for inventory size, can change in the Data folder (inspector)
        [field: SerializeField]
        public int Size { get; private set; } = 20;

        //this is an event that passes a dictionary as a parameter
        public event Action<Dictionary<int, InventoryItem>> OnInventoryUpdated;

        //called in Inventory controller, creates the list of items stored here with the size of the inventory
        //adds empty items initially to fill in inventory
        public void Initialize()
        {
            inventoryItems = new List<InventoryItem>();
            for (int i = 0; i < Size; i++)
            {
                inventoryItems.Add(InventoryItem.GetEmptyItem());
            }
        }

        //This method adds an item to the inventory given the item data and amount of the item
        public int AddItem(ItemSO item, int quantity)
        {
            //if the item is not stackable
            if (item.IsStackable == false)
            {
                //run through the whole inventory list (not just items, but empty space)
                for (int i = 0; i < inventoryItems.Count; i++)
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
            }
            quantity = AddStackableItem(item, quantity);
            InformAboutChange();
            return quantity;
        }

        //Adds the given item to the first free slot in the inventory
        private int AddItemToFirstFreeSlot(ItemSO item, int quantity)
        {
            //create a new item (struct) to be added to the inventory (of structs)
            InventoryItem newItem = new InventoryItem
            {
                item = item,
                quantity = quantity
            };

            //go through the whole inventory
            //check in each interation if it's empty or not. If it is, place the new item in the empty slot
            for (int i = 0; i < inventoryItems.Count; i++)
            {
                if (inventoryItems[i].IsEmpty)
                {
                    inventoryItems[i] = newItem;
                    return quantity;
                }
            }
            return 0;
        }

        //this is a loaded method
        //.Where branches off of the list of items (inventory) and searches through only the empty slots of the inventory (item.IsEmpty)
        //".Where(condition) -> returns an IEnumerable (filtered list)
        //.Any returns true if there's at least one element in it, so == false negates it
        //Return true if there are no empty slots — in other words, if the inventory is full.
        private bool IsInventoryFull()
            => inventoryItems.Where(item => item.IsEmpty).Any() == false;

        //adds a stackable item to the inventory
        private int AddStackableItem(ItemSO item, int quantity)
        {
            //loop through the item list (including the empty one)
            for (int i = 0; i < inventoryItems.Count; i++)
            {
                //if this is empty, it doesn't have the item we are looking for
                if (inventoryItems[i].IsEmpty)
                    continue;
                //checks if the item matches the ID of the item
                if (inventoryItems[i].item.ID == item.ID)
                {
                    //this is logic for placing an item in your inventory, but it goes over the amount you can stack
                    int amountPossibleToTake =
                        inventoryItems[i].item.MaxStackSize - inventoryItems[i].quantity;

                    //you have more collected than you can store in one space
                    if (quantity > amountPossibleToTake)
                    {
                        //.Change Quantity returns an item with the new quantity
                        inventoryItems[i] = inventoryItems[i]
                            .ChangeQuantity(inventoryItems[i].item.MaxStackSize);
                        quantity -= amountPossibleToTake;
                    }
                    else
                    {
                        inventoryItems[i] = inventoryItems[i]
                            .ChangeQuantity(inventoryItems[i].quantity + quantity);
                        InformAboutChange();
                        return 0;
                    }
                }
            }
            while (quantity > 0 && IsInventoryFull() == false)
            {
                int newQuantity = Mathf.Clamp(quantity, 0, item.MaxStackSize);
                quantity -= newQuantity;
                AddItemToFirstFreeSlot(item, newQuantity);
            }
            return quantity;
        }

        public void AddItem(InventoryItem item)
        {
            AddItem(item.item, item.quantity);
        }

        public Dictionary<int, InventoryItem> GetCurrentInventoryState()
        {
            Dictionary<int, InventoryItem> returnValue =
                new Dictionary<int, InventoryItem>();

            for (int i = 0; i < inventoryItems.Count; i++)
            {
                if (inventoryItems[i].IsEmpty)
                    continue;
                returnValue[i] = inventoryItems[i];
            }
            return returnValue;
        }

        public InventoryItem GetItemAt(int itemIndex)
        {
            return inventoryItems[itemIndex];
        }

        public void SwapItems(int itemIndex_1, int itemIndex_2)
        {
            InventoryItem item1 = inventoryItems[itemIndex_1];
            inventoryItems[itemIndex_1] = inventoryItems[itemIndex_2];
            inventoryItems[itemIndex_2] = item1;
            InformAboutChange();
        }

        private void InformAboutChange()
        {
            OnInventoryUpdated?.Invoke(GetCurrentInventoryState());
        }
    }

    [Serializable]
    public struct InventoryItem
    {
        public int quantity;
        public ItemSO item;
        public bool IsEmpty => item == null;

        public InventoryItem ChangeQuantity(int newQuantity)
        {
            return new InventoryItem
            {
                item = this.item,
                quantity = newQuantity,
            };
        }

        public static InventoryItem GetEmptyItem()
            => new InventoryItem
            {
                item = null,
                quantity = 0,
            };
    }
}