using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class InventorySlotUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image m_imageIcon;
    [SerializeField] private TMP_Text m_textQuantity;

    private Inventory m_inventory;
    private int m_nSlotIndex;
    private bool m_bWasDragged;

    public void Initialize(Inventory inventory, int nSlotIndex)
    {
        m_inventory = inventory;
        m_nSlotIndex = nSlotIndex;

        Clear();
    }

    public void SetItem(InventoryItem inventoryItem)
    {
        if (inventoryItem == null)
        {
            Clear();
            return;
        }

        m_imageIcon.sprite = inventoryItem.Data.Icon;
        m_imageIcon.enabled = true;

        m_textQuantity.text = inventoryItem.Quantity.ToString();
        m_textQuantity.enabled = true;
    }

    public void Clear()
    {
        m_imageIcon.sprite = null;
        m_imageIcon.enabled = false;

        m_textQuantity.text = string.Empty;
        m_textQuantity.enabled = false;
    }

    public int GetSlotIndex()
    {
        return m_nSlotIndex;
    }

    public Inventory GetInventory()
    {
        return m_inventory;
    }

    public Image GetIcon()
    {
        return m_imageIcon;
    }

    public bool HasItem()
    {
        return m_inventory != null && !m_inventory.Slots[m_nSlotIndex].IsEmpty;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        if (!HasItem())
        {
            return;
        }

        InventoryUI inventoryUI = GetComponentInParent<InventoryUI>();

        if (inventoryUI == null)
        {
            return;
        }

        inventoryUI.SelectSlot(m_nSlotIndex);
    }
}
