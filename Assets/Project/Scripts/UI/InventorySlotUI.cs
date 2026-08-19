using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class InventorySlotUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image m_imageIcon;
    [SerializeField] private TMP_Text m_textQuantity;
    [SerializeField] private GameObject m_goSelection;

    private Inventory _inventory;
    private int _nSlotIndex;

    public void Initialize(Inventory inventory, int nSlotIndex)
    {
        _inventory = inventory;
        _nSlotIndex = nSlotIndex;

        SetSelected(false);

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

        SetSelected(false);
    }

    public int GetSlotIndex()
    {
        return _nSlotIndex;
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
        return _inventory != null && !_inventory.Slots[_nSlotIndex].IsEmpty;
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

        inventoryUI.SelectSlot(_nSlotIndex);
    }

    public void SetSelected(bool bSelected)
    {
        if (m_goSelection != null)
        {
            m_goSelection.SetActive(bSelected);
        }

        if (bSelected)
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

        if (_nSlotIndex >= 10)
        {
            position = arrCorners[1];
            invertPivot = true;
        }

        inventoryUI.ShowTooltip(_nSlotIndex, position, invertPivot);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryUI inventoryUI = GetComponentInParent<InventoryUI>();

        if (inventoryUI == null)
        {
            return;
        }

        inventoryUI.HideTooltip();
    }
}
