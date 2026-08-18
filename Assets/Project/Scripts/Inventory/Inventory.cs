using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int m_nCapacity = 12;

    private readonly List<InventorySlot> m_listSlots = new();

    public IReadOnlyList<InventorySlot> Slots => m_listSlots;
    public int Capacity => m_nCapacity;

    public event Action OnInventoryChanged;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        m_listSlots.Clear();

        for (int nIndex = 0; nIndex < m_nCapacity; nIndex++)
        {
            m_listSlots.Add(new InventorySlot());
        }
    }


    public bool AddItem(ItemData dataItem, int nQuantity = 1)
    {
        if (dataItem == null || nQuantity <= 0)
        {
            return false;
        }

        int nRemainingQuantity = nQuantity;

        // First try to add the item to existing stacks.
        for (int nIndex = 0; nIndex < m_listSlots.Count; nIndex++)
        {
            InventorySlot inventorySlot = m_listSlots[nIndex];

            if (inventorySlot.IsEmpty)
            {
                continue;
            }

            InventoryItem inventoryItem = inventorySlot.Item;

            if (inventoryItem.Data != dataItem)
            {
                continue;
            }                

            int nAvailableSpace = dataItem.MaxStackSize - inventoryItem.Quantity;

            if (nAvailableSpace <= 0)
            {
                continue;
            }

            int nAmountToAdd = Mathf.Min(nRemainingQuantity, nAvailableSpace);

            inventoryItem.AddQuantity(nAmountToAdd);

            nRemainingQuantity -= nAmountToAdd;

            if (nRemainingQuantity <= 0)
            {
                OnInventoryChanged?.Invoke();
                return true;
            }
        }

        for (int nIndex = 0; nIndex < m_listSlots.Count; nIndex++)
        {
            InventorySlot inventorySlot = m_listSlots[nIndex];

            if (!inventorySlot.IsEmpty)
            {
                continue;
            }

            int nAmountToAdd = Mathf.Min(nRemainingQuantity, dataItem.MaxStackSize);

            inventorySlot.SetItem(new InventoryItem(dataItem, nAmountToAdd));

            nRemainingQuantity -= nAmountToAdd;

            if (nRemainingQuantity <= 0)
            {
                OnInventoryChanged?.Invoke();
                return true;
            }
        }

        OnInventoryChanged?.Invoke();

        return nRemainingQuantity < nQuantity;
    }

    public bool RemoveItem(int nSlotIndex, int nQuantity = 1)
    {
        if (!IsValidSlot(nSlotIndex) || nQuantity <= 0)
        {
            return false;
        }

        InventorySlot inventorySlot = m_listSlots[nSlotIndex];

        if (inventorySlot.IsEmpty)
        {
            return false;
        }

        InventoryItem inventoryItem = inventorySlot.Item;

        if (nQuantity > inventoryItem.Quantity)
            return false;

        inventoryItem.RemoveQuantity(nQuantity);

        if (inventoryItem.Quantity <= 0)
        {
            inventorySlot.Clear();
        }

        OnInventoryChanged?.Invoke();

        return true;
    }

    public void SwapSlots(int nFirstSlotIndex, int nSecondSlotIndex)
    {
        if (!IsValidSlot(nFirstSlotIndex) || !IsValidSlot(nSecondSlotIndex))
        {
            return;
        }

        InventoryItem firstItem = m_listSlots[nFirstSlotIndex].Item;
        InventoryItem secondItem = m_listSlots[nSecondSlotIndex].Item;

        m_listSlots[nFirstSlotIndex].SetItem(secondItem);
        m_listSlots[nSecondSlotIndex].SetItem(firstItem);

        OnInventoryChanged?.Invoke();
    }

    private bool IsValidSlot(int nSlotIndex)
    {
        return nSlotIndex >= 0 && nSlotIndex < m_listSlots.Count;
    }
}
