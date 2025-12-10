using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    public InventoryController playerInventory;
    public InventoryItem testItem;
    [SerializeField] List<ItemData> items;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            addItemsToInventory();
        }
    }

    private void addItemsToInventory()
    {
        playerInventory.InsertItem(testItem);
    }
}
