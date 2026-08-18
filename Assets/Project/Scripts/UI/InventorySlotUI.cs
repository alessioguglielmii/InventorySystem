using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image m_imageIcon;
    [SerializeField] private TMP_Text m_textQuantity;

    private int m_nSlotIndex;
    public void Initialize(int nSlotIndex)
    {
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
        m_textQuantity.enabled = inventoryItem.Quantity > 1;
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
}
