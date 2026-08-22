using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemTooltipUI : MonoBehaviour
{
    private const string QUANTITYLABEL = "QUANTITY: ";
    private const string MAXSTACKSIZELABEL = "MAX STACK SIZE: ";

    [Header("UI")]
    [SerializeField] private TMP_Text m_textItemName;
    [SerializeField] private TMP_Text m_textItemQuantity;
    [SerializeField] private TMP_Text m_textItemMaxStackSize;

    [Header("Pivot")]
    [SerializeField] private Vector2 m_offset = new Vector2(-2.0f, -2.0f);

    private Vector2 _pivot;
    private Vector2 _invertedPivot;

    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();

        _pivot = _rectTransform.pivot;
        _invertedPivot = new Vector2(1 - _pivot.x, _pivot.y);

        Hide();
    }

    public void Show(string itemName, string itemQuantity, string itemMaxStackSize, Vector2 screenPosition, bool invertPivot)
    {
        if (m_textItemName != null)
        {
            m_textItemName.text = itemName;
        }

        if (m_textItemQuantity != null)
        {
            m_textItemQuantity.text = QUANTITYLABEL + itemQuantity;
        }

        if (m_textItemMaxStackSize != null)
        {
            m_textItemMaxStackSize.text = MAXSTACKSIZELABEL + itemMaxStackSize;
        }

        gameObject.SetActive(true);

        Canvas.ForceUpdateCanvases();

        Vector2 position = screenPosition + m_offset;

        if (invertPivot)
        {
            _rectTransform.pivot = _invertedPivot;
        }
        else
        {
            _rectTransform.pivot = _pivot;
        }

        _rectTransform.position = position;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
