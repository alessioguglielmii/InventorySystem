using System.Collections.Generic;
using TMPro;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Inventory m_inventory;
    [SerializeField] private CharacterMovement m_characterMovement;
    [SerializeField] private CharacterCamera m_characterCamera;
    [SerializeField] private InventorySlotUI m_prefabSlot;
    [SerializeField] private Transform m_trContainer;
    [SerializeField] private Animator m_animator;
    [SerializeField] private Button m_buttonUse;
    [SerializeField] private TMP_Text m_textUse;

    private readonly List<InventorySlotUI> _listSlots = new();

    private int _selectedSlotIndex = -1;

    private bool _isShown = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (m_inventory == null)
        {
           return;
        }

        if (m_prefabSlot == null)
        {
           return;
        }

        if (m_trContainer == null)
        {
           return;
        }

        CreateSlots();

        m_inventory.OnInventoryChanged += Refresh;
        m_inventory.OnInventoryToggle += Toggle;

        if (m_buttonUse != null && m_textUse != null)
        {
            m_buttonUse.onClick.AddListener(UseSelectedItem);
            m_buttonUse.gameObject.GetComponent<Image>().enabled = false;
            m_textUse.gameObject.GetComponent<TextMeshProUGUI>().enabled = false;
        }

        Refresh();
    }

    private void OnDestroy()
    {
        if (m_inventory != null)
        {
            m_inventory.OnInventoryChanged -= Refresh;
        }

        if (m_buttonUse != null)
        {
            m_buttonUse.onClick.RemoveListener(UseSelectedItem);
        }
    }

    private void CreateSlots()
    {
        for (int nIndex = 0; nIndex < m_inventory.Capacity; nIndex++)
        {
            InventorySlotUI slotUI = Instantiate(m_prefabSlot, m_trContainer);

            slotUI.Initialize(m_inventory, nIndex);

            _listSlots.Add(slotUI);
        }
    }

    private void Refresh()
    {
        IReadOnlyList<InventorySlot> listInventorySlots = m_inventory.Slots;

        for (int nIndex = 0; nIndex < _listSlots.Count; nIndex++)
        {
            InventorySlot inventorySlot = listInventorySlots[nIndex];

            _listSlots[nIndex].SetItem(inventorySlot.Item);
        }
    }

    public void SelectSlot(int nSlotIndex)
    {
        if (nSlotIndex < 0 || nSlotIndex >= m_inventory.Capacity)
        {
            return;
        }

        InventorySlot invSlot = m_inventory.Slots[nSlotIndex];

        if (invSlot.IsEmpty)
        {
            DeselectSlot();
            return;
        }

        _selectedSlotIndex = nSlotIndex;

        if (m_buttonUse != null)
        {
            bool bCanUse = invSlot.Item.Data.Effect != null;

            m_buttonUse.gameObject.GetComponent<Image>().enabled = bCanUse;
            m_textUse.gameObject.GetComponent<TextMeshProUGUI>().enabled = bCanUse;
        }
    }

    private void DeselectSlot()
    {
        _selectedSlotIndex = -1;

        if (m_buttonUse != null)
        {
            m_buttonUse.gameObject.GetComponent<Image>().enabled = false;
            m_textUse.gameObject.GetComponent<TextMeshProUGUI>().enabled = false;
        }
    }

    private void UseSelectedItem()
    {
        if (_selectedSlotIndex < 0)
        {
            return;
        }

        InventorySlot invSlot = m_inventory.Slots[_selectedSlotIndex];

        if (invSlot.IsEmpty)
        {
            DeselectSlot();
            return;
        }

        GameObject goTarget = GetItemTarget(invSlot.Item.Data);

        bool bUsed = m_inventory.UseItem(_selectedSlotIndex, goTarget);

        if (bUsed)
        {
            DeselectSlot();
        }
    }

    private GameObject GetItemTarget(ItemData itemData)
    {
        if (itemData == null)
        {
            return null;
        }

        if (itemData.Effect is ExplosionEffect)
        {
            return m_inventory.gameObject;
        }

        if (itemData.Effect is UnlockEffect)
        {
            return FindUnlockableTarget();
        }

        return null;
    }

    private GameObject FindUnlockableTarget()
    {
        if (m_characterMovement.Unlockable)
        {
            return m_characterMovement.Unlockable.gameObject;
        }

        return null;
    }

    private void Toggle()
    {
        m_animator.SetTrigger("Toggle");

        _isShown = !_isShown;

        if (_isShown)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        m_characterMovement.OnMoveChanged();
        m_characterCamera.OnMoveChanged();
    }
}
