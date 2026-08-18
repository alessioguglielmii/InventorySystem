using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Inventory m_inventory;
    [SerializeField] private InventorySlotUI m_prefabSlot;
    [SerializeField] private Transform m_trContainer;

    private readonly List<InventorySlotUI> m_listSlots = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (m_inventory == null)
        {
            return;
        }

        CreateSlots();

        m_inventory.OnInventoryChanged += Refresh;

        Refresh();
    }

    private void OnDestroy()
    {
        if (m_inventory != null)
        {
            m_inventory.OnInventoryChanged -= Refresh;
        }
    }

    private void CreateSlots()
    {
        for (int nIndex = 0; nIndex < m_inventory.Capacity; nIndex++)
        {
            InventorySlotUI slotUI = Instantiate(m_prefabSlot, m_trContainer);

            slotUI.Initialize(nIndex);

            m_listSlots.Add(slotUI);
        }
    }

    private void Refresh()
    {
        IReadOnlyList<InventorySlot> listInventorySlots = m_inventory.Slots;

        for (int nIndex = 0; nIndex < m_listSlots.Count; nIndex++)
        {
            InventorySlot inventorySlot = listInventorySlots[nIndex];

            m_listSlots[nIndex].SetItem(inventorySlot.Item);
        }
    }
}
