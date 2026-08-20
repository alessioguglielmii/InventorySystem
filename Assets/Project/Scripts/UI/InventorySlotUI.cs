using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class InventorySlotUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image m_imageIcon;
    [SerializeField] private TMP_Text m_textQuantity;
    [SerializeField] private GameObject m_goSelection;
    [SerializeField] private ColorPalette m_colorPalette;

    private Inventory _inventory;
    private int _slotIndex;
    private bool _isPointerEnter = false;
    private bool _isSelected = false;

    public void Initialize(Inventory inventory, int slotIndex)
    {
        _inventory = inventory;
        _slotIndex = slotIndex;

        SetSelected(false, false);

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

        SetSelected(false, false);
    }

    public int GetSlotIndex()
    {
        return _slotIndex;
    }

    public Inventory GetInventory()
    {
        return _inventory;
    }

    public Image GetIcon()
    {
        return m_imageIcon;
    }

    public bool HasItem()
    {
        return _inventory != null && !_inventory.Slots[_slotIndex].IsEmpty;
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

        inventoryUI.SelectSlot(_slotIndex, false);
    }

    public void SetSelected(bool selected, bool moved)
    {
        if (m_goSelection != null)
        {
            m_goSelection.SetActive(selected);
        }

        _isSelected = selected;

        if (selected)
        {
            m_textQuantity.color = m_colorPalette.Text.Hovered;
        }
        else
        {
            if (_isPointerEnter)
            {
                m_textQuantity.color = m_colorPalette.Text.Hovered;
            }
            else
            {
                m_textQuantity.color = m_colorPalette.Text.Normal;
            }
        }

        if (selected && !moved)
        {
            InventoryUI inventoryUI = GetComponentInParent<InventoryUI>();

            if (inventoryUI != null)
            {
                inventoryUI.PlaySlotSelectedClip();
            }
        }     
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!HasItem())
        {
            return;
        }

        InventoryUI inventoryUI = GetComponentInParent<InventoryUI>();

        if (inventoryUI == null)
        {
            return;
        }

        RectTransform rectTransform = GetComponent<RectTransform>();

        Vector3[] arrCorners = new Vector3[4];

        rectTransform.GetWorldCorners(arrCorners);

        Vector3 position = arrCorners[2];
        bool invertPivot = false;

        if (_slotIndex >= 10)
        {
            position = arrCorners[1];
            invertPivot = true;
        }

        _isPointerEnter = true;

        m_textQuantity.color = m_colorPalette.Text.Hovered;

        inventoryUI.ShowTooltip(_slotIndex, position, invertPivot);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryUI inventoryUI = GetComponentInParent<InventoryUI>();

        if (inventoryUI == null)
        {
            return;
        }

        _isPointerEnter = false;

        if (!_isSelected)
        {
            m_textQuantity.color = m_colorPalette.Text.Normal;
        }

        inventoryUI.HideTooltip();
    }
}
