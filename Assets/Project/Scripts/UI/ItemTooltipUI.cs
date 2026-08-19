using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemTooltipUI : MonoBehaviour
{
    [SerializeField] private TMP_Text m_textItemName;
    [SerializeField] private Vector2 m_offset = new Vector2(10f, 10f);

    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();

        Hide();
    }

    public void Show(string itemName, Vector2 screenPosition)
    {
        if (m_textItemName != null)
        {
            m_textItemName.text = itemName;
        }

        gameObject.SetActive(true);

        Canvas.ForceUpdateCanvases();

        Vector2 position = screenPosition + m_offset;

        Vector2 size = _rectTransform.rect.size;

        float halfWidth = size.x * 0.5f;

        float halfHeight = size.y * 0.5f;

        position.x = Mathf.Clamp(position.x, halfWidth, Screen.width -halfWidth);

        position.y = Mathf.Clamp(position.y, halfHeight, Screen.height - halfHeight);

        _rectTransform.position = position;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
