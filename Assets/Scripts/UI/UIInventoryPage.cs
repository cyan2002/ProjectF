using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventoryPage : MonoBehaviour
{
    [SerializeField]
    private UIInventoryItem itemPrefab;

    [SerializeField]
    private RectTransform contentPanel;

    //used making a description for the item
    //[SerializeField]
    //private UIInventoryDescription itemDescription;

    [SerializeField]
    private MouseFollower mouseFollower;

    //list of items in your inventory
    List<UIInventoryItem> listOfUIItems = new List<UIInventoryItem>();

    public Sprite image, image2;
    public int quantity;
    public string title;

    private int currentlyDraggedItemIndex = -1;

    private void Awake()
    {
        Hide();
        mouseFollower.Toggle(false);
    }

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

    private void HandleShowItemActions(UIInventoryItem inventoryItemUI)
    {
        
    }

    private void HandleEndDrag(UIInventoryItem inventoryItemUI)
    {
        mouseFollower.Toggle(false);
    }

    private void HandleSwap(UIInventoryItem inventoryItemUI)
    {
        int index = listOfUIItems.IndexOf(inventoryItemUI);
        if (index == -1)
        {
            mouseFollower.Toggle(false);
            currentlyDraggedItemIndex = -1;
            return;
        }
        listOfUIItems[currentlyDraggedItemIndex]
            .SetData(index == 0 ? image : image2, quantity);
        listOfUIItems[index]
            .SetData(currentlyDraggedItemIndex == 0 ? image : image2, quantity);

        mouseFollower.Toggle(false);
        currentlyDraggedItemIndex = -1;

    }

    private void HandleBeginDrag(UIInventoryItem inventoryItemUI)
    {
        int index = listOfUIItems.IndexOf(inventoryItemUI);
        if (index == -1)
            return;
        currentlyDraggedItemIndex = index;

        mouseFollower.Toggle(true);
        mouseFollower.SetData(index == 0 ? image : image2, quantity);
    }

    private void HandleItemSelection(UIInventoryItem inventoryItemUI)
    {
        listOfUIItems[0].Select();
    }

    //opens the inventory
    public void Show()
    {
        gameObject.SetActive(true);

        listOfUIItems[0].SetData(image, quantity);
        listOfUIItems[1].SetData(image2, quantity);
    }

    //closes the inventory
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
