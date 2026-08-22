using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int m_capacity = 12;

    private readonly List<InventorySlot> m_listSlots = new();

    public IReadOnlyList<InventorySlot> Slots => m_listSlots;
    public int Capacity => m_capacity;

    public event Action OnInventoryChanged;
    public event Action<int, int> OnInventorySlotMoved;
    public event Action OnInventoryToggle;

    private void Awake()
    {
        Initialize();
    }

    private void Start()
    {
        PlayerInputSingleton.Instance.Actions["Inventory"].performed += OnInventoryShow;
    }

    private void OnDestroy()
    {
        PlayerInputSingleton.Instance.Actions["Inventory"].performed -= OnInventoryShow;
    }

    private void Initialize()
    {
        m_listSlots.Clear();

        for (int index = 0; index < m_capacity; index++)
        {
            m_listSlots.Add(new InventorySlot());
        }
    }

    public bool HasSpaceForItem(ItemData dataItem, int quantity = 1)
    {
        if (dataItem == null || quantity <= 0)
        {
            return false;
        }

        int remainingQuantity = quantity;

        for (int index = 0; index < m_listSlots.Count; index++)
        {
            InventorySlot inventorySlot = m_listSlots[index];

            if (inventorySlot.IsEmpty)
            {
                continue;
            }

            InventoryItem inventoryItem = inventorySlot.Item;

            if (inventoryItem.Data != dataItem)
            {
                continue;
            }

            int availableSpace = dataItem.MaxStackSize - inventoryItem.Quantity;

            if (availableSpace <= 0)
            {
                continue;
            }

            remainingQuantity -= availableSpace;

            if (remainingQuantity <= 0)
            {
                return true;
            }
        }

        for (int index = 0; index < m_listSlots.Count; index++)
        {
            InventorySlot inventorySlot = m_listSlots[index];

            if (!inventorySlot.IsEmpty)
            {
                continue;
            }

            remainingQuantity -= dataItem.MaxStackSize;

            if (remainingQuantity <= 0)
            {
                return true;
            }
        }

        return false;
    }

    public bool AddItem(ItemData dataItem, int quantity = 1)
    {
        if (dataItem == null || quantity <= 0)
        {
            return false;
        }

        int remainingQuantity = quantity;

        for (int index = 0; index < m_listSlots.Count; index++)
        {
            InventorySlot inventorySlot = m_listSlots[index];

            if (inventorySlot.IsEmpty)
            {
                continue;
            }

            InventoryItem inventoryItem = inventorySlot.Item;

            if (inventoryItem.Data != dataItem)
            {
                continue;
            }                

            int availableSpace = dataItem.MaxStackSize - inventoryItem.Quantity;

            if (availableSpace <= 0)
            {
                continue;
            }

            int amountToAdd = Mathf.Min(remainingQuantity, availableSpace);

            inventoryItem.AddQuantity(amountToAdd);

            remainingQuantity -= amountToAdd;

            if (remainingQuantity <= 0)
            {
                OnInventoryChanged?.Invoke();
                return true;
            }
        }

        for (int index = 0; index < m_listSlots.Count; index++)
        {
            InventorySlot inventorySlot = m_listSlots[index];

            if (!inventorySlot.IsEmpty)
            {
                continue;
            }

            int nAmountToAdd = Mathf.Min(remainingQuantity, dataItem.MaxStackSize);

            inventorySlot.SetItem(new InventoryItem(dataItem, nAmountToAdd));

            remainingQuantity -= nAmountToAdd;

            if (remainingQuantity <= 0)
            {
                OnInventoryChanged?.Invoke();
                return true;
            }
        }

        OnInventoryChanged?.Invoke();

        return remainingQuantity < quantity;
    }

    public bool RemoveItem(int slotIndex, int quantity = 1)
    {
        if (!IsValidSlot(slotIndex) || quantity <= 0)
        {
            return false;
        }

        InventorySlot inventorySlot = m_listSlots[slotIndex];

        if (inventorySlot.IsEmpty)
        {
            return false;
        }

        InventoryItem inventoryItem = inventorySlot.Item;

        if (quantity > inventoryItem.Quantity)
            return false;

        inventoryItem.RemoveQuantity(quantity);

        if (inventoryItem.Quantity <= 0)
        {
            inventorySlot.Clear();
        }

        OnInventoryChanged?.Invoke();

        return true;
    }

    public bool MoveItem(int sourceSlotIndex, int targetSlotIndex)
    {
        if (!IsValidSlot(sourceSlotIndex) || !IsValidSlot(targetSlotIndex))
        {
            return false;
        }

        if (sourceSlotIndex == targetSlotIndex)
        {
            return false;
        }

        InventorySlot inventorySourceSlot = m_listSlots[sourceSlotIndex];

        InventorySlot inventoryTargetSlot = m_listSlots[targetSlotIndex];

        if (inventorySourceSlot.IsEmpty)
        {
            return false;
        }

        if (inventoryTargetSlot.IsEmpty)
        {
            InventoryItem inventoryItem = inventorySourceSlot.Item;

            inventoryTargetSlot.SetItem(inventoryItem);
            inventorySourceSlot.Clear();

            OnInventoryChanged?.Invoke();
            OnInventorySlotMoved?.Invoke(sourceSlotIndex, targetSlotIndex);

            return true;
        }

        InventoryItem inventorySourceItem = inventorySourceSlot.Item;

        InventoryItem inventoryTargetItem = inventoryTargetSlot.Item;

        if (inventorySourceItem.Data == inventoryTargetItem.Data)
        {
            int availableSpace = inventoryTargetItem.GetAvailableSpace();

            if (availableSpace <= 0)
            {
                return false;
            }

            int amountToMove = Mathf.Min(inventorySourceItem.Quantity, availableSpace);

            inventoryTargetItem.AddQuantity(amountToMove);
            inventorySourceItem.RemoveQuantity(amountToMove);

            if (inventorySourceItem.Quantity <= 0)
            {
                inventorySourceSlot.Clear();
            }

            OnInventoryChanged?.Invoke();
            OnInventorySlotMoved?.Invoke(sourceSlotIndex, targetSlotIndex);

            return true;
        }

        InventoryItem tempItem = inventorySourceSlot.Item;

        inventorySourceSlot.SetItem(inventoryTargetSlot.Item);
        inventoryTargetSlot.SetItem(tempItem);

        OnInventoryChanged?.Invoke();
        OnInventorySlotMoved?.Invoke(sourceSlotIndex, targetSlotIndex);

        return true;
    }

    private bool IsValidSlot(int slotIndex)
    {
        return slotIndex >= 0 && slotIndex < m_listSlots.Count;
    }

    public bool UseItem(int slotIndex, GameObject target)
    {
        if (!IsValidSlot(slotIndex))
        {
            return false;
        }

        InventorySlot inventorySlot = m_listSlots[slotIndex];

        if (inventorySlot.IsEmpty)
        {
            return false;
        }
            
        InventoryItem inventoryItem = inventorySlot.Item;

        if (inventoryItem.Data.Effect == null)
        {
            return false;
        }

        bool effectApplied = inventoryItem.Data.Effect.Apply(target);

        if (!effectApplied)
        {
            return false;
        }

        inventoryItem.RemoveQuantity(1);

        if (inventoryItem.Quantity <= 0)
        {
            inventorySlot.Clear();
        }

        OnInventoryChanged?.Invoke();

        return true;
    }

    private void OnInventoryShow(InputAction.CallbackContext context)
    {
        OnInventoryToggle?.Invoke();
    }
}
