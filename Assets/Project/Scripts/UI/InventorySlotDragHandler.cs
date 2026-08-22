using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    private Canvas m_canvas;
    private InventorySlotUI m_slotUI;
    private GameObject m_dragObject;
    private RectTransform m_transformDragObject;

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

        if (m_dragObject != null)
        {
            m_transformDragObject.position = eventData.position;
        }


    }

    public void OnDrag(PointerEventData eventData)
    {
        if (m_transformDragObject == null)
        {
            return;
        }

        m_transformDragObject.position = eventData.position;
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

        int sourceSlotIndex = sourceSlotUI.GetSlotIndex();

        int targetSlotIndex = targetSlotUI.GetSlotIndex();

        if (sourceSlotIndex == targetSlotIndex)
        {
            return;
        }

        Inventory inventory = m_slotUI.GetInventory();

        if (inventory == null)
        {
            return;
        }

        inventory.MoveItem(sourceSlotIndex, targetSlotIndex);
    }

    private void CreateDragObject()
    {
        Image imageSourceIcon = m_slotUI.GetIcon();

        if (imageSourceIcon == null || imageSourceIcon.sprite == null)
        {
            return;
        }

        m_dragObject = new GameObject("DragIcon");

        m_dragObject.transform.SetParent(m_canvas.transform, false);

        m_transformDragObject = m_dragObject.AddComponent<RectTransform>();

        Image imgageDragIcon = m_dragObject.AddComponent<Image>();

        imgageDragIcon.sprite = imageSourceIcon.sprite;
        imgageDragIcon.raycastTarget = false;

        m_transformDragObject.sizeDelta = m_slotUI.GetIcon().rectTransform.rect.size;
    }

    private void DestroyDragObject()
    {
        if (m_dragObject != null)
        {
            Destroy(m_dragObject);

            m_dragObject = null;
            m_transformDragObject = null;
        }
    }

    private InventorySlotUI GetSlotUI()
    {
        return m_slotUI;
    }

}
