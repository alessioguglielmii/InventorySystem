using System;

[Serializable]
public class InventorySlot
{
    public InventoryItem Item { get; private set; }

    public bool IsEmpty => Item == null;

    public void SetItem(InventoryItem inventoryItem)
    {
        Item = inventoryItem;
    }

    public void Clear()
    {
        Item = null;
    }
}
