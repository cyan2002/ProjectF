using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemGrid : MonoBehaviour
{
    public const float tileSizeWidth = 32;
    public const float tileSizeHeight = 32;

    ShelfInventoryItem[,] inventoryItemSlot;

    RectTransform rectTransform;

    [SerializeField]
    private CanvasScaler canvasGroup;

    Vector2 positionOnTheGrid = new Vector2();
    Vector2Int tileGridPosition = new Vector2Int();
    public Camera uiCamera;

    [SerializeField] int gridSizeWidth = 10;
    [SerializeField] int gridSizeHeight = 9;

    //to removes item, it only gives u the location of the press
    //we need to find the origin and cycle from that origin to properly remove the item
    public ShelfInventoryItem PickUpItem(int x, int y)
    {
        ShelfInventoryItem toReturn = inventoryItemSlot[x, y];

        if (toReturn == null) { return null; }

        CleanGridReference(toReturn);

        return toReturn;
    }

    private void CleanGridReference(ShelfInventoryItem item)
    {
        for (int ix = 0; ix < item.itemData.width; ix++)
        {
            for (int iy = 0; iy < item.itemData.height; iy++)
            {
                inventoryItemSlot[item.onGridPositionX + ix, item.onGridPositionY + iy] = null;
            }
        }
    }

    internal ShelfInventoryItem GetItem(int x, int y)
    {
        return inventoryItemSlot[x, y];
    }

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        Init(gridSizeWidth, gridSizeHeight);
    }

    private void Init(int width, int height)
    {
        inventoryItemSlot = new ShelfInventoryItem[width, height];
        Vector2 size = new Vector2(width * tileSizeWidth, height * tileSizeHeight);
        rectTransform.sizeDelta = size;
    }

    //returns the Grid position of the tile where the mouse currently is
    public Vector2Int GetTileGridPosition(Vector2 mousePosition)
    {
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

        int x = Mathf.FloorToInt(TL.x / tileSizeWidth);
        int y = Mathf.FloorToInt(-TL.y / tileSizeHeight);  // NOTICE THE '-' HERE

        return new Vector2Int(x, y);
    }

    public bool PlaceItem(ShelfInventoryItem inventoryItem, int posX, int posY, ref ShelfInventoryItem overlapItem)
    {
        if (BoundryCheck(posX, posY, inventoryItem.itemData.width, inventoryItem.itemData.height) == false)
        {
            return false;
        }

        if (OverlapCheck(posX, posY, inventoryItem.itemData.width, inventoryItem.itemData.height, ref overlapItem) == false)
        {
            overlapItem = null;
            return false;
        }

        if (overlapItem != null)
        {
            CleanGridReference(overlapItem);
        }

        RectTransform itemRT = inventoryItem.GetComponent<RectTransform>();
        itemRT.SetParent(rectTransform, false);

        //adding it into the inventory array to keep track of it
        for (int xnum = 0; xnum < inventoryItem.itemData.width; xnum++)
        {
            for (int ynum = 0; ynum < inventoryItem.itemData.height; ynum++)
            {
                inventoryItemSlot[posX + xnum, posY + ynum] = inventoryItem;

            }
        }

        inventoryItem.onGridPositionX = posX;
        inventoryItem.onGridPositionY = posY;
        float x, y;
        Vector2 position = CalculatePositionOnGrid(inventoryItem, posX, posY);

        itemRT.localPosition = position;

        return true;
    }

    public Vector2 CalculatePositionOnGrid(ShelfInventoryItem inventoryItem, int posX, int posY)
    {
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
        position.x = topLeftOrigin.x + posX * tileSizeWidth + tileSizeWidth * 0.5f * inventoryItem.itemData.width;
        position.y = topLeftOrigin.y - posY * tileSizeHeight - tileSizeHeight * 0.5f * inventoryItem.itemData.height;
        return position;
    }

    private bool OverlapCheck(int posX, int posY, int width, int height, ref ShelfInventoryItem overlapItem)
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
