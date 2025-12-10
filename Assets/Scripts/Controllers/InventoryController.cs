using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//attaches to that item or player with that inventory
public class InventoryController : MonoBehaviour
{
    [HideInInspector]
    public ItemGrid selectedItemGrid;

    //whenever you make a call to selectedItemGrid it comes with functions attached
    //calling SelectedItemGrid returns selceted Item Grid
    //with this, you can also set selectedItemGrid to value and set the parent of that grid to value.
    public ItemGrid SelectedItemGrid
    {
        get => selectedItemGrid;
        set
        {
            selectedItemGrid = value;
            inventoryHighlight.SetParent(value);
        }
    }

    InventoryItem selectedItem;
    InventoryItem overlapItem;
    RectTransform rectTransform;

    [SerializeField] List<ItemData> items;
    [SerializeField] GameObject itemPrefab;
    [SerializeField] Transform canvasTransform;

    //for toggling the inventory off and on
    [SerializeField] GameObject inventoryObject;

    InventoryHighlight inventoryHighlight;

    //statement of whether the inventory is open or not
    private bool InventoryOpen = false;

    private void Awake()
    {
        inventoryHighlight = GetComponent<InventoryHighlight>();
    }

    //checks for user input
    //updates item dragger and highlighter
    private void Update()
    {
        //toggling player inventory
        if (Input.GetKeyDown(KeyCode.I) && !InventoryOpen)
        {
            inventoryObject.SetActive(true);
            InventoryOpen = true;
        }
        else if(Input.GetKeyDown(KeyCode.I) && InventoryOpen)
        {
            inventoryObject.SetActive(false);
            InventoryOpen = false;
        }

        //object dragging highlighter
        HandleHighlight();

        //rotate the selected item being held
        if(Input.GetKeyDown(KeyCode.R))
        {
            RotateItem();
        }

        //place or pick up item on/in inventory
        if (Input.GetMouseButtonDown(0))
        {
            LeftMouseButtonPress();
        }

        //if there is no selected item, remove the item dragger
        if (selectedItemGrid == null) 
        {
            inventoryHighlight.Show(false);
            return; 
        }

        //creating a random item that is assigned to the mouse position, can be placed into an inventory after
        //USED FOR TESTING
        if (Input.GetKeyDown(KeyCode.J))
        {
            //only create if the item selected is null
            if (selectedItem == null)
            {
                CreateRandomItem();
            }
        }

        //inserts a random item if mouse is over the inventory inside the inventory
        //USED FOR TESTING
        if (Input.GetKeyDown(KeyCode.K))
        {
            //only insert if not holding another item
            if (selectedItem == null)
            {
                InsertRandomItem();
            }
        }
    }

    //adds a random Item to the inventory, placing it into the next available spot - USE FOR PICKING UP PACKAGES OF SUPPLIES IN GAME
    private void InsertRandomItem()
    {
        if (selectedItemGrid == null) { return;}

        CreateRandomItem();
        InventoryItem itemToInsert = selectedItem;
        selectedItem = null;
        InsertItem(itemToInsert);
    }

    //inserts an item into the inventory
    //USE THIS FUNCTION TO ADD ITEMS AFTER PURCHASE
    public void InsertItem(InventoryItem itemToInsert)
    {
        Vector2Int? posOnGrid = selectedItemGrid.FindSpaceForObject(itemToInsert);

        if (posOnGrid == null) { return; }

        selectedItemGrid.PlaceItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
    }

    //creates a random item from the list of items given
    //for testing purposes
    private void CreateRandomItem()
    {
        InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
        selectedItem = inventoryItem;

        rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(canvasTransform);
        rectTransform.SetAsLastSibling();

        int selectedItemID = UnityEngine.Random.Range(0, items.Count);
        inventoryItem.Set(items[selectedItemID]);
    }

    //this returns the tile grid position that mouse is currently over
    private Vector2Int? GetTileGridPosition()
    {
        if (selectedItemGrid == null)
            return null;

        Vector2 position = Input.mousePosition;

        if (selectedItem != null)
        {
            position.x -= (selectedItem.WIDTH - 1) * ItemGrid.tileSizeWidth / 2;
            position.y += (selectedItem.HEIGHT - 1) * ItemGrid.tileSizeHeight / 2;
        }

        return selectedItemGrid.GetTileGridPosition(position);
    }

    Vector2Int oldPosition;
    InventoryItem itemToHighlight;

    private void HandleHighlight()
    {
        //need to check when mouse is out of bounds... to not run code.
        if (selectedItem != null)
        {
            rectTransform.position = Input.mousePosition;
        }
        if(selectedItem == null)
        {
            inventoryHighlight.Show(false);
            return;
        }

        Vector2Int? pos = GetTileGridPosition();
        if (pos == null)
        {
            inventoryHighlight.Show(false);
            return;
        }

        Vector2Int positionOnGrid = pos.Value;
        if (oldPosition == positionOnGrid) { return; }
        oldPosition = positionOnGrid;
        if (selectedItem == null)
        {
            itemToHighlight = selectedItemGrid.GetItem(positionOnGrid.x, positionOnGrid.y);

            if(itemToHighlight != null)
            {
                inventoryHighlight.Show(true);
                inventoryHighlight.SetSize(itemToHighlight);
                inventoryHighlight.SetPosition(selectedItemGrid, itemToHighlight);
            }
            else
            {
                inventoryHighlight.Show(false);
            }
        }
        else
        {
            inventoryHighlight.Show(selectedItemGrid.BoundryCheck(positionOnGrid.x, 
                positionOnGrid.y, 
                selectedItem.WIDTH,
                selectedItem.HEIGHT));
            inventoryHighlight.SetSize(selectedItem);
            inventoryHighlight.SetPosition(selectedItemGrid, selectedItem, positionOnGrid.x, positionOnGrid.y);
        }
    }

    //when mouse button is clicked, get the tilegrid position of where the mouse is and pass that to the pick up or place item function
    private void LeftMouseButtonPress()
    {
        Vector2Int? pos = GetTileGridPosition();
        if (pos == null) return;

        Vector2Int tileGridPosition = pos.Value;


        if (selectedItem == null)
        {
            PickUpItem(tileGridPosition);
        }
        else
        {
            PlaceItem(tileGridPosition);
        }
    }

    //places the item using the ItemGrid Function and if completed sucessfully, make the selecteditem nbull and reset variables.
    private void PlaceItem(Vector2Int tileGridPosition)
    {
        bool complete = selectedItemGrid.PlaceItem(selectedItem, tileGridPosition.x, tileGridPosition.y, ref overlapItem);

        if (complete)
        {
            selectedItem = null;
            if(overlapItem != null)
            {
                selectedItem = overlapItem;
                overlapItem = null;
                rectTransform = selectedItem.GetComponent<RectTransform>();
                rectTransform.SetAsLastSibling();
            }
        }
    }

    //picks up items using the ItemGrid Function and if completed sucessfully, make the rectransform the current transform that we are working with
    private void PickUpItem(Vector2Int tileGridPosition)
    {
        selectedItem = selectedItemGrid.PickUpItem(tileGridPosition.x, tileGridPosition.y);
        if (selectedItem != null)
        {
            rectTransform = selectedItem.GetComponent<RectTransform>();
        }
    }

    //rotates the item is R is pressed, only rotates if the selected item is not null (there is an item to rotate)
    private void RotateItem()
    {
        if (selectedItem == null) { return; }

        selectedItem.Rotate();
    }
}
