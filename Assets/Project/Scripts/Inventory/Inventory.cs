using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int m_nCapacity = 12;

    private readonly List<InventorySlot> m_listSlots = new();

    public IReadOnlyList<InventorySlot> Slots => m_listSlots;
    public int Capacity => m_nCapacity;

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

        InventorySlot invSourceSlot = m_listSlots[sourceSlotIndex];

        InventorySlot invTargetSlot = m_listSlots[targetSlotIndex];

        if (invSourceSlot.IsEmpty)
        {
            return false;
        }

        if (invTargetSlot.IsEmpty)
        {
            InventoryItem inventoryItem = invSourceSlot.Item;

            invTargetSlot.SetItem(inventoryItem);
            invSourceSlot.Clear();

            OnInventoryChanged?.Invoke();
            OnInventorySlotMoved?.Invoke(sourceSlotIndex, targetSlotIndex);

            return true;
        }

        InventoryItem invSourceItem = invSourceSlot.Item;

        InventoryItem invTargetItem = invTargetSlot.Item;

        if (invSourceItem.Data == invTargetItem.Data)
        {
            int nAvailableSpace = invTargetItem.GetAvailableSpace();

            if (nAvailableSpace <= 0)
            {
                return false;
            }

            int amountToMove = Mathf.Min(invSourceItem.Quantity, nAvailableSpace);

            invTargetItem.AddQuantity(amountToMove);
            invSourceItem.RemoveQuantity(amountToMove);

            if (invSourceItem.Quantity <= 0)
            {
                invSourceSlot.Clear();
            }

            OnInventoryChanged?.Invoke();
            OnInventorySlotMoved?.Invoke(sourceSlotIndex, targetSlotIndex);

            return true;
        }

        InventoryItem tempItem = invSourceSlot.Item;

        invSourceSlot.SetItem(invTargetSlot.Item);
        invTargetSlot.SetItem(tempItem);

        OnInventoryChanged?.Invoke();
        OnInventorySlotMoved?.Invoke(sourceSlotIndex, targetSlotIndex);

        return true;
    }

    private bool IsValidSlot(int nSlotIndex)
    {
        return nSlotIndex >= 0 && nSlotIndex < m_listSlots.Count;
    }

    public bool UseItem(int nSlotIndex, GameObject goTarget)
    {
        if (!IsValidSlot(nSlotIndex))
        {
            return false;
        }

        InventorySlot invSlot = m_listSlots[nSlotIndex];

        if (invSlot.IsEmpty)
        {
            return false;
        }
            
        InventoryItem invItem = invSlot.Item;

        if (invItem.Data.Effect == null)
        {
            return false;
        }

        bool bEffectApplied = invItem.Data.Effect.Apply(goTarget);

        if (!bEffectApplied)
        {
            return false;
        }

        invItem.RemoveQuantity(1);

        if (invItem.Quantity <= 0)
        {
            invSlot.Clear();
        }

        OnInventoryChanged?.Invoke();

        return true;
    }

    private void OnInventoryShow(InputAction.CallbackContext context)
    {
        OnInventoryToggle?.Invoke();
    }
}
