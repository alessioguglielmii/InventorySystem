using System;
using UnityEngine;

[Serializable]
public class InventoryItem
{
    public ItemData Data { get; private set; }
    public int Quantity { get; private set; }

    public InventoryItem(ItemData dataItem, int nQuantity = 1)
    {
        Data = dataItem;
        Quantity = nQuantity;
    }

    public void AddQuantity(int nAmount)
    {
        Quantity += nAmount;
    }

    public void RemoveQuantity(int nAmount)
    {
        Quantity -= nAmount;
    }

    public int GetAvailableSpace()
    {
        if (Data == null)
        {
            return 0;
        }

        return Data.MaxStackSize - Quantity;
    }
}
