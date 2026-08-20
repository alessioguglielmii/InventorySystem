using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    private InventorySlotUI m_slotUI;
    private Canvas m_canvas;

    private GameObject m_goDragObject;
    private RectTransform m_trDragObject;

    private void Awake()
    {
        m_slotUI = GetComponent<InventorySlotUI>();
        m_canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        InventoryUI inventoryUI = GetComponentInParent<InventoryUI>();

        if (inventoryUI != null)
        {
            inventoryUI.HideTooltip();

            inventoryUI.isDragging = true;
        }

        if (m_slotUI == null)
        {
            return;
        }

        if (!m_slotUI.HasItem())
        {
            return;
        }

        CreateDragObject();

        if (m_goDragObject != null)
        {
            m_trDragObject.position = eventData.position;
        }


    }

    public void OnDrag(PointerEventData eventData)
    {
        if (m_trDragObject == null)
        {
            return;
        }

        m_trDragObject.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DestroyDragObject();

        InventoryUI inventoryUI = GetComponentInParent<InventoryUI>();

        if (inventoryUI != null)
        {
            inventoryUI.isDragging = false;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        InventorySlotDragHandler dragHandler = eventData.pointerDrag?.GetComponent<InventorySlotDragHandler>();

        if (dragHandler == null)
        {
            return;
        }
        
        InventorySlotUI sourceSlotUI = dragHandler.GetSlotUI();

        if (sourceSlotUI == null)
        {
            return;
        }

        InventorySlotUI targetSlotUI = m_slotUI;

        int nSourceSlotIndex = sourceSlotUI.GetSlotIndex();

        int nTargetSlotIndex = targetSlotUI.GetSlotIndex();

        if (nSourceSlotIndex == nTargetSlotIndex)
        {
            return;
        }

        Inventory inventory = m_slotUI.GetInventory();

        if (inventory == null)
        {
            return;
        }

        inventory.MoveItem(nSourceSlotIndex, nTargetSlotIndex);
    }

    private void CreateDragObject()
    {
        Image imgSourceIcon = m_slotUI.GetIcon();

        if (imgSourceIcon == null || imgSourceIcon.sprite == null)
        {
            return;
        }

        m_goDragObject = new GameObject("DragIcon");

        m_goDragObject.transform.SetParent(m_canvas.transform, false);

        m_trDragObject = m_goDragObject.AddComponent<RectTransform>();

        Image imgDragIcon = m_goDragObject.AddComponent<Image>();

        imgDragIcon.sprite = imgSourceIcon.sprite;
        imgDragIcon.raycastTarget = false;

        m_trDragObject.sizeDelta = m_slotUI.GetIcon().rectTransform.rect.size;
    }

    private void DestroyDragObject()
    {
        if (m_goDragObject != null)
        {
            Destroy(m_goDragObject);

            m_goDragObject = null;
            m_trDragObject = null;
        }
    }

    private InventorySlotUI GetSlotUI()
    {
        return m_slotUI;
    }

}
