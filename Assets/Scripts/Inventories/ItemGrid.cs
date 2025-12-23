using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemGrid : MonoBehaviour
{
    public const float tileSizeWidth = 80;
    public const float tileSizeHeight = 80;

    //list of items in the inventory
    InventoryItem[,] inventoryItemSlot;

    RectTransform rectTransform;

    [SerializeField]
    private CanvasScaler canvasGroup;

    public Camera uiCamera;

    [SerializeField] int gridSizeWidth = 10;
    [SerializeField] int gridSizeHeight = 9;

    //to removes item, it only gives u the location of the press
    //we need to find the origin and cycle from that origin to properly remove the item
    public InventoryItem PickUpItem(int x, int y)
    {
        InventoryItem toReturn = inventoryItemSlot[x, y];

        if (toReturn == null) { return null; }

        CleanGridReference(toReturn);

        return toReturn;
    }

    private void CleanGridReference(InventoryItem item)
    {
        for (int ix = 0; ix < item.WIDTH; ix++)
        {
            for (int iy = 0; iy < item.HEIGHT; iy++)
            {
                inventoryItemSlot[item.onGridPositionX + ix, item.onGridPositionY + iy] = null;
            }
        }
    }

    internal InventoryItem GetItem(int x, int y)
    {
        return inventoryItemSlot[x, y];
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        Init(gridSizeWidth, gridSizeHeight);
        transform.parent.gameObject.SetActive(false);
    }

    private void Init(int width, int height)
    {
        float scale = canvasGroup.scaleFactor;  // CanvasScaler scale

        float tw = tileSizeWidth * scale;
        float th = tileSizeHeight * scale;

        inventoryItemSlot = new InventoryItem[width, height];

        Vector2 size = new Vector2(width * tw, height * th);
        rectTransform.sizeDelta = size;
    }

    //finds space for the object to be placed into inventory
    //searches through the whole inventory, slot by slot to see if the item to insert can fit
    //returns the vector2 of the first spot where the item can fit
    public Vector2Int? FindSpaceForObject(InventoryItem itemToInsert)
    {
        int height = gridSizeHeight - itemToInsert.HEIGHT + 1;
        int width = gridSizeWidth - itemToInsert.WIDTH + 1;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if(CheckAvailableSpace(x, y, itemToInsert.WIDTH, itemToInsert.HEIGHT))
                {
                    return new Vector2Int(x, y);
                }
            }
        }

        return null;
    }

    //this passes the item data itself, not the object.
    //we are checking to see if this object can fit in the player, if so continue on
    public bool CheckSpaceInInventory(ItemData itemToInsert)
    {
        int height = gridSizeHeight - itemToInsert.height - 1;
        int width = gridSizeWidth - itemToInsert.width - 1;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (CheckAvailableSpace(x, y, itemToInsert.width, itemToInsert.height))
                {
                    return true;
                }
            }
        }

        return false;
    }

    //returns the Grid position of the tile where the mouse currently is
    public Vector2Int GetTileGridPosition(Vector2 mousePosition)
    {
        float s = canvasGroup.scaleFactor; // CanvasScaler scale

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            mousePosition,
            null, // for Screen Space - Overlay
            out localPoint
        );

        // Convert pivot-based coords → top-left coords
        Vector2 rectSize = rectTransform.rect.size;
        Vector2 topLeftOrigin = new Vector2(
            -rectSize.x * rectTransform.pivot.x,
             rectSize.y * (1 - rectTransform.pivot.y)
        );

        // Top-left–relative coordinate
        Vector2 TL = localPoint - topLeftOrigin;

        int x = Mathf.FloorToInt(TL.x / (tileSizeWidth * s));
        int y = Mathf.FloorToInt(-TL.y / (tileSizeHeight * s));  // NOTICE THE '-' HERE

        return new Vector2Int(x, y);
    }

    //places item, but checks if the item will go over borders/out of bounds
    public bool PlaceItem(InventoryItem inventoryItem, int posX, int posY, ref InventoryItem overlapItem)
    {
        if (BoundryCheck(posX, posY, inventoryItem.WIDTH, inventoryItem.HEIGHT) == false)
        {
            return false;
        }

        if (OverlapCheck(posX, posY, inventoryItem.WIDTH, inventoryItem.HEIGHT, ref overlapItem) == false)
        {
            overlapItem = null;
            return false;
        }

        if (overlapItem != null)
        {
            CleanGridReference(overlapItem);
        }

        PlaceItem(inventoryItem, posX, posY);

        return true;
    }

    //can be called to place item IF you already know that the item will fit
    public void PlaceItem(InventoryItem inventoryItem, int posX, int posY)
    {
        RectTransform itemRT = inventoryItem.GetComponent<RectTransform>();
        itemRT.SetParent(rectTransform, false);

        //adding it into the inventory array to keep track of it
        for (int xnum = 0; xnum < inventoryItem.WIDTH; xnum++)
        {
            for (int ynum = 0; ynum < inventoryItem.HEIGHT; ynum++)
            {
                inventoryItemSlot[posX + xnum, posY + ynum] = inventoryItem;

            }
        }

        inventoryItem.onGridPositionX = posX;
        inventoryItem.onGridPositionY = posY;
        Vector2 position = CalculatePositionOnGrid(inventoryItem, posX, posY);

        itemRT.localPosition = position;
    }

    public Vector2 CalculatePositionOnGrid(InventoryItem inventoryItem, int posX, int posY)
    {
        float scale = canvasGroup.scaleFactor;  // CanvasScaler scale
        float tw = tileSizeWidth * scale;
        float th = tileSizeHeight * scale;

        // Position item inside the grid
        Vector2 rectSize = rectTransform.rect.size;
        Vector2 pivot = rectTransform.pivot;

        // Convert pivot origin → top-left origin
        Vector2 topLeftOrigin = new Vector2(
            -rectSize.x * pivot.x,
             rectSize.y * (1 - pivot.y)
        );

        Vector2 position = new Vector2();
        // Item position in top-left coordinate space
        position.x = topLeftOrigin.x + posX * tileSizeWidth + tw * 0.5f * inventoryItem.WIDTH;
        position.y = topLeftOrigin.y - posY * tileSizeHeight - th * 0.5f * inventoryItem.HEIGHT;
        return position;
    }

    private bool OverlapCheck(int posX, int posY, int width, int height, ref InventoryItem overlapItem)
    {
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                if(inventoryItemSlot[posX + x, posY + y] != null)
                {
                    if(overlapItem == null)
                    {
                        overlapItem = inventoryItemSlot[posX + x, posY + y];
                    }
                    else
                    {
                        //overlapped with two items - player needs to find another place to put item
                        if(overlapItem != inventoryItemSlot[posX + x, posY + y])
                        {
                            return false;
                        }
                    }
                    
                }
            }
        }

        return true;
    }

    //given a position, checks to see if it's empty or not
    private bool CheckAvailableSpace(int posX, int posY, int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (inventoryItemSlot[posX + x, posY + y] != null)
                {
                    return false;
                }
            }
        }
        return true;
    }

    //checks if the position is within the boundaires of the inventory grid or not
    bool PositionCheck(int posX, int posY)
    {
        if(posX < 0 || posY < 0)
        {
            return false;
        }

        if(posX >= gridSizeWidth || posY >= gridSizeHeight)
        {
            return false;
        }

        return true;
    }

    public bool BoundryCheck(int posX, int posY, int width, int height)
    {
        if(PositionCheck(posX, posY) == false)
        {
            return false;
        }

        posX += width-1;
        posY += height-1;

        if(PositionCheck(posX, posY) == false)
        {
            return false;
        }

        return true;
    }
}
