using System;
using UnityEngine;

[Serializable]
public class InventoryItem
{
    public ItemData Data { get; private set; }
    public int Quantity { get; private set; }

    public InventoryItem(ItemData dataItem, int quantity = 1)
    {
        Data = dataItem;
        Quantity = quantity;
    }

    public void AddQuantity(int amount)
    {
        Quantity += amount;
    }

    public void RemoveQuantity(int amount)
    {
        Quantity -= amount;
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
