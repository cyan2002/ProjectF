using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventoryPage : MonoBehaviour
{
    [SerializeField]
    private UIInventoryItem itemPrefab;

    [SerializeField]
    private RectTransform contentPanel;

    //list of items in your inventory
    List<UIInventoryItem> listOfUIItems = new List<UIInventoryItem>();

    //creates a number of items in the inventory
    public void InitializeInventoryUI(int inventorysize)
    {
        for (int i = 0; i < inventorysize; i++)
        {
            UIInventoryItem uiItem = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
            uiItem.transform.SetParent(contentPanel);
            listOfUIItems.Add(uiItem);
            uiItem.transform.localScale = new Vector3(1, 1, 1);

            uiItem.OnItemClicked += HandleItemSelection;
            uiItem.OnItemBeginDrag += HandleBeginDrag;
            uiItem.OnItemDroppedOn += HandleSwap;
            uiItem.OnItemEndDrag += HandleEndDrag;
            uiItem.OnRightMouseBtnClick += HandleShowItemActions;
        }
    }

    private void HandleShowItemActions(UIInventoryItem obj)
    {
        
    }

    private void HandleEndDrag(UIInventoryItem obj)
    {
        
    }

    private void HandleSwap(UIInventoryItem obj)
    {
        
    }

    private void HandleBeginDrag(UIInventoryItem obj)
    {
        
    }

    private void HandleItemSelection(UIInventoryItem obj)
    {
        print(obj.name);
    }

    //opens the inventory
    public void Show()
    {
        gameObject.SetActive(true);
    }

    //closes the inventory
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
