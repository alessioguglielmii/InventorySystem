using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("Playable Character")]
    [SerializeField] private Inventory m_inventory;
    [SerializeField] private CharacterMovement m_characterMovement;
    [SerializeField] private CharacterCamera m_characterCamera;

    [Header("Slots")]
    [SerializeField] private Transform m_container;
    [SerializeField] private InventorySlotUI m_slot;

    [Header("UI")]
    [SerializeField] private ItemTooltipUI m_itemTooltip;
    [SerializeField] private Button m_buttonUse;
    [SerializeField] private TMP_Text m_textUse;

    [Header("Animation")]
    [SerializeField] private Animator m_animator;

    [Header("Audio")]
    [SerializeField] private AudioSource m_audioSource;
    [SerializeField] private AudioClip m_toggleClip;
    [SerializeField] private AudioClip m_slotSelectedClip;
    [SerializeField] private AudioClip m_buttonUsePressedSuccessClip;
    [SerializeField] private AudioClip m_buttonUsePressedFailClip;

    [Header("Library")]
    [SerializeField] private ColorPalette m_colorPalette;

    [HideInInspector] public bool isDragging = false;

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

        if (m_slot == null)
        {
           return;
        }

        if (m_container == null)
        {
           return;
        }

        if (m_itemTooltip != null)
        {
            m_itemTooltip.Hide();
        }

        CreateSlots();

        m_inventory.OnInventoryChanged += Refresh;
        m_inventory.OnInventorySlotMoved += UpdateSelectedSlot;
        m_inventory.OnInventoryToggle += Toggle;

        if (m_buttonUse != null && m_textUse != null)
        {
            m_buttonUse.onClick.AddListener(UseSelectedItem);
            m_buttonUse.gameObject.GetComponent<Image>().enabled = false;
            m_textUse.gameObject.GetComponent<TextMeshProUGUI>().enabled = false;

            m_textUse.color = m_colorPalette.Text.Normal;

            EventTrigger trigger = m_buttonUse.gameObject.GetComponent<EventTrigger>();

            if (trigger == null)
            {
                trigger = m_buttonUse.gameObject.AddComponent<EventTrigger>();
            }

            EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
            pointerEnter.eventID = EventTriggerType.PointerEnter;
            pointerEnter.callback.AddListener((data) => UseButtonPonterEnter());

            trigger.triggers.Add(pointerEnter);

            EventTrigger.Entry pointerExit = new EventTrigger.Entry();
            pointerExit.eventID = EventTriggerType.PointerExit;
            pointerExit.callback.AddListener((data) => UseButtonPonterExit());

            trigger.triggers.Add(pointerExit);
        }

        Refresh();
    }

    private void OnDestroy()
    {
        if (m_inventory != null)
        {
            m_inventory.OnInventoryChanged -= Refresh;
            m_inventory.OnInventorySlotMoved -= UpdateSelectedSlot;
            m_inventory.OnInventoryToggle -= Toggle;
        }

        if (m_buttonUse != null)
        {
            m_buttonUse.onClick.RemoveListener(UseSelectedItem);
        }
    }

    private void CreateSlots()
    {
        for (int index = 0; index < m_inventory.Capacity; index++)
        {
            InventorySlotUI slotUI = Instantiate(m_slot, m_container);

            slotUI.Initialize(m_inventory, index);

            _listSlots.Add(slotUI);
        }
    }

    private void Refresh()
    {
        IReadOnlyList<InventorySlot> listInventorySlots = m_inventory.Slots;

        for (int index = 0; index < _listSlots.Count; index++)
        {
            InventorySlot inventorySlot = listInventorySlots[index];

            _listSlots[index].SetItem(inventorySlot.Item);
        }
    }

    private void UpdateSelectedSlot(int sourceSlotIndex, int targetSlotIndex)
    {
        if (_selectedSlotIndex >= 0)
        {
            if (_selectedSlotIndex == sourceSlotIndex)
            {
                DeselectSlot();
                SelectSlot(targetSlotIndex, true);
            }
            else if(_selectedSlotIndex == targetSlotIndex)
            {
                DeselectSlot();
                SelectSlot(sourceSlotIndex, true);
            }
        }
    }

    public void SelectSlot(int slotIndex, bool moved)
    {
        if (slotIndex < 0 || slotIndex >= m_inventory.Capacity)
        {
            return;
        }

        InventorySlot inventorySlot = m_inventory.Slots[slotIndex];

        if (inventorySlot.IsEmpty)
        {
            DeselectSlot();
            return;
        }

        if (_selectedSlotIndex >= 0 && _selectedSlotIndex < _listSlots.Count)
        {
            _listSlots[_selectedSlotIndex].SetSelected(false, moved);
        }

        if (_selectedSlotIndex >= 0 && _selectedSlotIndex == slotIndex)
        {
            DeselectSlot();

            PlaySlotSelectedClip();

            return;
        }

        _selectedSlotIndex = slotIndex;

        _listSlots[_selectedSlotIndex].SetSelected(true, moved);

        if (m_buttonUse != null && m_textUse != null)
        {
            bool bCanUse = inventorySlot.Item.Data.Effect != null;

            m_buttonUse.gameObject.GetComponent<Image>().enabled = bCanUse;
            m_textUse.gameObject.GetComponent<TextMeshProUGUI>().enabled = bCanUse;
        }
    }

    private void DeselectSlot()
    {
        if (_selectedSlotIndex >= 0 && _selectedSlotIndex < _listSlots.Count)
        {
            _listSlots[_selectedSlotIndex].SetSelected(false, false);
        }

        _selectedSlotIndex = -1;

        if (m_buttonUse != null && m_textUse != null)
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

        InventorySlot inventorySlot = m_inventory.Slots[_selectedSlotIndex];

        if (inventorySlot.IsEmpty)
        {
            DeselectSlot();
            return;
        }

        GameObject target = GetItemTarget(inventorySlot.Item.Data);

        bool bUsed = m_inventory.UseItem(_selectedSlotIndex, target);

        if (bUsed)
        {
            if (inventorySlot.Item == null)
            {
                DeselectSlot();
            }

            if (m_audioSource != null && m_buttonUsePressedSuccessClip != null)
            {
                m_audioSource.PlayOneShot(m_buttonUsePressedSuccessClip);
            }
        }
        else
        {
            if (m_audioSource != null && m_buttonUsePressedFailClip != null)
            {
                m_audioSource.PlayOneShot(m_buttonUsePressedFailClip);
            }
        }
    }

    private void UseButtonPonterEnter()
    {
        m_textUse.color = m_colorPalette.Text.Hovered;
    }

    private void UseButtonPonterExit()
    {
        m_textUse.color = m_colorPalette.Text.Normal;
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

        if (itemData.Effect is InvisibilityEffect)
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
        if (m_audioSource != null && m_toggleClip != null)
        {
            m_audioSource.PlayOneShot(m_toggleClip);
        }

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

    public void ShowTooltip(int nSlotIndex, Vector3 position, bool invertPivot)
    {
        if (m_itemTooltip == null)
        {
            return;
        }

        if (nSlotIndex < 0 || nSlotIndex >= m_inventory.Capacity)
        {
            return;
        }

        if (isDragging)
        {
            return;
        }

        InventorySlot invSlot = m_inventory.Slots[nSlotIndex];

        if (invSlot.IsEmpty)
        {
            m_itemTooltip.Hide();
            return;
        }

        string itemName = invSlot.Item.Data.ItemName;
        string itemQuantity = invSlot.Item.Quantity.ToString();
        string itemMaxStackSize = invSlot.Item.Data.MaxStackSize.ToString();

        m_itemTooltip.Show(itemName, itemQuantity, itemMaxStackSize, new Vector2(position.x, position.y), invertPivot);
    }

    public void HideTooltip()
    {
        if (m_itemTooltip != null)
        {
            m_itemTooltip.Hide();
        }
    }

    public void PlaySlotSelectedClip()
    {
        if (m_audioSource != null && m_slotSelectedClip != null)
        {
            m_audioSource.PlayOneShot(m_slotSelectedClip);
        }
    }
}
