using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//attaches to that item or player with that inventory
public class InventoryController : MonoBehaviour
{
    public static InventoryController Instance { get; private set; }
    //[HideInInspector]
    public ItemGrid selectedItemGrid;
    public Transform itemsLayerTransform;

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

    [SerializeField]
    InventoryItem selectedItem;
    InventoryItem overlapItem;
    RectTransform rectTransform;

    [SerializeField] List<ItemData> items;
    [SerializeField] GameObject itemPrefab;
    [SerializeField] Transform canvasTransform;

    //for toggling the inventory off and on
    [SerializeField] GameObject inventoryObject;
    //used for shop purchases, since the mouse makes selectedItemGrid null when not hovering over, adding items would be impossible since the grid is null
    //this varaible would bypass that and allow item entry even when inventory is not selected.
    [SerializeField] ItemGrid selectedGrid;

    InventoryHighlight inventoryHighlight;

    //statement of whether the inventory is open or not
    private bool InventoryOpen = false;
    private bool rotated = false;

    private void Awake()
    {
        Instance = this;
        inventoryHighlight = GetComponent<InventoryHighlight>();
        PlayerInput.HandleI += OpenInventory;
        PlayerInput.HandleR += RotateItem;
        PlayerInput.HandleLeftClick += LeftClick;
        PlayerInput.HandleShiftClick += ShiftClick;
        PlayerInput.HandleJ += TestJ;
        PlayerInput.HandleK += TestK;
    }


    //checks for user input
    //updates item dragger and highlighter
    private void Update()
    {
        //object dragging highlighter
        HandleHighlight();

        //if there is no selected item, remove the item dragger
        //MAKES IT RUN OR NOT DEPENDING ON IF THE MOUSE IS OVER THE INVENTORY
        if (selectedItemGrid == null)
        {
            inventoryHighlight.Show(false);
            return;
        }
    }

    private void TestJ()
    {
        //creating a random item that is assigned to the mouse position, can be placed into an inventory after
        //USED FOR TESTING
        //only create if the item selected is null
        if (selectedItem == null)
        {
            CreateRandomItem();
        }
    }

    private void TestK()
    {
        //only insert if not holding another item
        if (selectedItem == null)
        {
            InsertRandomItem();
        }
    }

    private void LeftClick()
    {
        LeftMouseButtonPress();
    }

    //handles shift click function
    //if user is over an item and shift clicks with two inventories open it will send the item over to the other inventory in the next available spot
    //bugged - if going from shelf to player inventory, does not seem to account for too much space
    private void ShiftClick()
    {
        //if there is no selectedItemGrid get out
        if (selectedItemGrid == null) { return; }
        
        //if there is a selected item get out (must not be holding it to shift click)
        if(selectedItem != null) { return; }

        //checking to see if you're over the tileGrid
        Vector2Int? pos = GetTileGridPosition();
        if (pos == null) return;

        Vector2Int tileGridPosition = pos.Value;

        //finds all types of ItemGrids (hopefully two to transfer between, if there is not get out)
        ItemGrid[] grids = FindObjectsOfType<ItemGrid>();
        int savedNum = 0;

        //checking to see if there are two grids even open
        if(grids.Length < 2)
        {
            return;
        }

        InventoryItem itemToInsert = null;
        Vector2Int? posOnGrid = null;

        //Assigns the selected item and picks it up from the grid (removing it from the grid)
        selectedItem = selectedItemGrid.PickUpItem(tileGridPosition.x, tileGridPosition.y);

        //if the selectedItem is not null (function worked) continue
        if (selectedItem != null)
        {
            //for each item in grid, find the one where it's not the current one being transfered out, that Itemgrid is the one we want to transfer to
            rectTransform = selectedItem.GetComponent<RectTransform>();
            for (int i = 0; i < grids.Length; i++)
            {
                if (grids[i].gameObject != selectedItemGrid.gameObject)
                {
                    //Debug.Log(grids[i].gameObject);
                    //Debug.Log(selectedGrid.gameObject);
                    savedNum = i;
                }
            }
        }

        itemToInsert = selectedItem;
        posOnGrid = grids[savedNum].FindSpaceForObject(itemToInsert);

        if(itemToInsert == null)
        {
            return;
        }

        //making sure there's space on the grid, if not selected is null again and leave
        if (posOnGrid == null)
        {
            //ADD SOUNDS TO INDICATE YOU CAN'T PLACE ITEMS CAUSE OF FULL
            //had to make another method to return position on the grid.
            PlaceItem(grids[savedNum].returnPosOnGrid(itemToInsert));
            return;
        }
        rectTransform.SetParent(grids[savedNum].GetComponent<RectTransform>(), false);
        rectTransform.SetAsLastSibling();

        selectedItem = null;
        grids[savedNum].PlaceItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
    }

    private void OpenInventory()
    {
        //toggling player inventory
        if (!InventoryOpen)
        {
            inventoryObject.SetActive(true);
            InventoryOpen = true;
        }
        else if (InventoryOpen)
        {
            inventoryObject.SetActive(false);
            InventoryOpen = false;
        }
    }

    //adds a random Item to the inventory, placing it into the next available spot - USE FOR PICKING UP PACKAGES OF SUPPLIES IN GAME
    private void InsertRandomItem()
    {
        //this gets caught if mouse is not over the inventory, for inserting random item I want this to run either way...
        if (selectedItemGrid == null) { return; }

        CreateRandomItem();
        InventoryItem itemToInsert = selectedItem;
        selectedItem = null;
        InsertItem(itemToInsert);
    }

    //inserts an item into the inventory
    //USE THIS FUNCTION TO ADD ITEMS AFTER PURCHASE
    public void InsertItem(InventoryItem itemToInsert)
    {
        //will print out null
        //Debug.Log(selectedItemGrid);
        //no selectedItemGrid (null), can fix with other controller?
        Vector2Int? posOnGrid = selectedItemGrid.FindSpaceForObject(itemToInsert);

        if (posOnGrid == null)
        {
            return;
        }

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

    //same as CreateRandomItem; however, it's not random, so it's just create the item given the item
    public bool purchaseItem(ItemData item)
    {
        InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
        selectedItem = inventoryItem;

        rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(canvasTransform, false);
        rectTransform.SetAsLastSibling();

        inventoryItem.Set(item);

        //this part comes from InsertRandomItem
        InventoryItem itemToInsert = selectedItem;
        selectedItem = null;

        //comes from InsertItem
        //no selectedItemGrid (null), can fix with other controller?
        //selectedGrid used here instead of selectedItemGrid because selectedItemGrid is turned on and off from mouse placement, but selectedGrid is always there.
        //this means when testing I won't be able to place items in the inventory via mouse off grid
        Vector2Int? posOnGrid = inventoryObject.GetComponentInChildren<ItemGrid>().FindSpaceForObject(itemToInsert);

        //if posOnGrid is null, there is no space for the object to be placed and it will be destroyed and returned to the box
        if (posOnGrid == null)
        {
            Destroy(inventoryItem.gameObject);
            return false;
        }

        inventoryObject.GetComponentInChildren<ItemGrid>().PlaceItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
        return true;
    }



    //this returns the tile grid position that mouse is currently over
    //this affects where the item is placed and the starting block it is.
    private Vector2Int? GetTileGridPosition()
    {
        if (selectedItemGrid == null)
            return null;

        Vector2 position = Input.mousePosition;

        //for some reason this code was added and was creating an offset for the grid. 
        //if (selectedItem != null)
        //{
        //    position.x -= (selectedItem.WIDTH - 1) * ItemGrid.tileSizeWidth / 2;
        //    position.y += (selectedItem.HEIGHT - 1) * ItemGrid.tileSizeHeight / 2;
        //}

        return selectedItemGrid.GetTileGridPosition(position);
    }

    Vector2Int oldPosition;
    InventoryItem itemToHighlight;

    private void HandleHighlight()
    {
        //need to check when mouse is out of bounds... to not run code.
        if (selectedItem != null)
        {
            //this is where the item follows the mouse input
            //CHANGE HERE TO ADJUST MOUSE BEING IN THE MIDDLE OF THE DRAGGED ITEM VS ON THE LEFT (WHERE IT'S PLACED)
            int width = selectedItem.WIDTH;
            int height = selectedItem.HEIGHT;

            //offset formula to make mouse appear in the top left corner
            float x = Mathf.Max(0f, 20f * width - 30f);
            float y = Mathf.Max(0f, 20f * height - 30f);

            rectTransform.position = Input.mousePosition + new Vector3(x, -y,0);
            //rectTransform.position = new Vector2(0, 0);
        }
        //if there is no item turn the highlighter off.
        if (selectedItem == null)
        {
            inventoryHighlight.Show(false);
            return;
        }

        Vector2Int? pos = GetTileGridPosition();

        //if the mouse isn't on the grid remove the highlighter.
        if (pos == null)
        {
            inventoryHighlight.Show(false);
            return;
        }

        Vector2Int positionOnGrid = pos.Value;
        //the below part made it so that when picking up the item, the highlighter was not showing...
        //I think this code just lets the code below not run constantly... If preformance becomes an issue need to fix.
        //checks if it's been rotatated, if so pass on to allow highlighter to be on.
        //also checks if the old position is the same as the new one (to not allow constant run of this code.)
        //finally checks if the highligher is on - this is because when you pick up and item the position stayed the same, but still needed the highlighter to be turned on again
        if (oldPosition == positionOnGrid && !rotated)
        {
            if (inventoryHighlight.checkOn())
            {
                return;
            }
        }
        oldPosition = positionOnGrid;
        if (selectedItem == null)
        {
            itemToHighlight = selectedItemGrid.GetItem(positionOnGrid.x, positionOnGrid.y);

            if (itemToHighlight != null)
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
            //code for actually turning on the highlighter.
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

        //Debug.Log(pos);

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
            if (overlapItem != null)
            {
                selectedItem = overlapItem;
                overlapItem = null;
                rectTransform = selectedItem.GetComponent<RectTransform>();
                rectTransform.SetParent(canvasTransform, false);
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
            //to avoid item from being hidden
            rectTransform.SetParent(itemsLayerTransform, false);
            rectTransform.SetAsLastSibling();
        }
        
    }

    //rotates the item is R is pressed, only rotates if the selected item is not null (there is an item to rotate)
    private void RotateItem()
    {
        if (selectedItem == null) { return; }

        selectedItem.Rotate();

        //simple boolean to allow the highlighter to be refreshed when the item is rotated.
        rotated = true;
    }
}
