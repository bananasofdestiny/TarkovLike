using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGrid : MonoBehaviour
{
    public const float tileSizeWidth = 32;
    public const float tileSizeHeight = 32;

    public RectTransform rectTransform;
    Vector2 positionOnTheGrid = new Vector2();
    Vector2Int tileGridPosition = new Vector2Int();
    InventoryItem[,] inventoryItemsSlot; 

    [SerializeField] int gridSizeWidth = 9;
    [SerializeField] int gridSizeeHight = 12;
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        Init(gridSizeWidth, gridSizeeHight);


    }

    private void Init(int width, int height)
    {
        inventoryItemsSlot = new InventoryItem[width, height];
        Vector2 size = new Vector2(width * tileSizeWidth,height * tileSizeHeight);  
        rectTransform.sizeDelta = size;
    }

    public Vector2Int GetTileGridPosition(Vector2 mousePosition)
    {
        positionOnTheGrid.x = mousePosition.x - rectTransform.position.x;
        positionOnTheGrid.y = rectTransform.position.y - mousePosition.y;

        tileGridPosition.x = (int)(positionOnTheGrid.x / tileSizeWidth);
        tileGridPosition.y = (int)(positionOnTheGrid.y / tileSizeHeight);

        return tileGridPosition;
    }
    public bool PlaceItem(InventoryItem inventoryItem,int posX,int posY, ref InventoryItem overlapItem)
    {

        if (BoundryCheck(posX, posY, inventoryItem.WIDTH, inventoryItem.HEIGHT) == false) { return false; }

        if (OverlapCheck(posX, posY, inventoryItem.WIDTH, inventoryItem.HEIGHT, ref overlapItem) == false)
        {
            overlapItem = null;
            return false;
        }

        if (overlapItem != null)
        {
            CleanGridReference(overlapItem);
        }

        return PlaceItem(inventoryItem, posX, posY);
    }

    public bool PlaceItem(InventoryItem inventoryItem, int posX, int posY)
    {
        RectTransform rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(this.rectTransform);


        for (int x = 0; x < inventoryItem.WIDTH; x++)
        {
            for (int y = 0; y < inventoryItem.HEIGHT; y++)
            {
                inventoryItemsSlot[posX + x, posY + y] = inventoryItem;
            }
        }
        inventoryItem.onGridPositionX = posX;
        inventoryItem.onGridPositionY = posY;

        Vector2 position = CalculatePositionOnGrid(inventoryItem, posX, posY);

        rectTransform.localPosition = position;
        return true;
    }

    public Vector2 CalculatePositionOnGrid(InventoryItem inventoryItem, int posX, int posY)
    {
        Vector2 position = new Vector2();
        position.x = posX * tileSizeWidth + tileSizeWidth * inventoryItem.WIDTH / 2;
        position.y = -(posY * tileSizeHeight + tileSizeHeight * inventoryItem.HEIGHT / 2);
        return position;
    }

    private bool OverlapCheck(int posX, int posY, int width, int height, ref InventoryItem overlapItem)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0;y<height; y++)
            {
                if (inventoryItemsSlot[posX + x, posY + y] != null)
                {
                    if (overlapItem == null)
                    {
                        overlapItem = inventoryItemsSlot[posX + x, posY + y];

                    }
                    else
                    {
                        if (overlapItem != inventoryItemsSlot[posX + x, posY + y])
                        {
                            return false;
                        }
                    }
                }
            }
        }



        return true;
    }

    private bool CheckAvialableSpace(int posX, int posY, int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (inventoryItemsSlot[posX + x, posY + y] != null)
                {
                    return false;
                }
            }
        }



        return true;
    }



    public  InventoryItem PickUpItem(int x, int y)
    {
        InventoryItem toReturn = inventoryItemsSlot[x, y];

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
                inventoryItemsSlot[item.onGridPositionX + ix, item.onGridPositionY + iy] = null;
            }
        }
    }

    bool PositionCheck(int posX, int posY) 
    {
        if (posX <0 || posY < 0)
        {
            return false;
        }
        if (posX >= gridSizeWidth || posY >= gridSizeeHight)
        {
            return false;
        }
        return true;
    }

     public bool BoundryCheck(int posX, int posY, int width, int height)
    {
        if(PositionCheck(posX, posY) == false) { return false; }
        posX += width - 1;
        posY += height - 1;
        if (PositionCheck(posX, posY) == false) { return false; }



        return true;
    }

    internal InventoryItem GetItem(int x, int y)
    {
        return inventoryItemsSlot[x, y];
    }

    public Vector2Int? FindSpaceForObject(InventoryItem itemToInsert)
    {
        int height = gridSizeeHight - itemToInsert.HEIGHT + 1;
        int width = gridSizeWidth - itemToInsert.WIDTH + 1;
        for (int y = 0;y< height; y++)
        {
            for (int x = 0; x < width; x++)
            {
               if (CheckAvialableSpace(x, y, itemToInsert.WIDTH, itemToInsert.HEIGHT))
               {
                    return new Vector2Int(x, y);
               }
            }
        }

        return null;

    }
}
