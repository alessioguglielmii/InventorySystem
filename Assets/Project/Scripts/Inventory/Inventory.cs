using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int m_nCapacity = 12;

    private readonly List<InventoryItem> m_lstItems = new();

    public IReadOnlyList<InventoryItem> Items => m_lstItems;
    public int Capacity => m_nCapacity;

    public event Action OnInventoryChanged;

    public bool AddItem(ItemData dataItem, int nQuantity = 1)
    {
        if (dataItem == null || nQuantity <= 0)
            return false;

        InventoryItem inventoryItem = FindItem(dataItem);

        if (inventoryItem != null)
        {
            inventoryItem.AddQuantity(nQuantity);

            OnInventoryChanged?.Invoke();
            return true;
        }

        if (m_lstItems.Count >= m_nCapacity)
            return false;

        m_lstItems.Add(new InventoryItem(dataItem, nQuantity));

        OnInventoryChanged?.Invoke();
        return true;
    }

    public bool RemoveItem(ItemData dataItem, int nQuantity = 1)
    {
        InventoryItem inventoryItem = FindItem(dataItem);

        if (inventoryItem == null || nQuantity <= 0)
            return false;

        inventoryItem.RemoveQuantity(nQuantity);

        if (inventoryItem.Quantity <= 0)
        {
            m_lstItems.Remove(inventoryItem);
        }

        OnInventoryChanged?.Invoke();
        return true;
    }

    private InventoryItem FindItem(ItemData dataItem)
    {
        return m_lstItems.Find(item => item.Data == dataItem);
    }
}
