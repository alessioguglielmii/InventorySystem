using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("General")]
    [SerializeField] private string m_stringItemName;
    [SerializeField] private Sprite m_spriteIcon;

    [Header("Stack")]
    [SerializeField] private int m_maxStackSize = 1;

    [Header("Effect")]
    [SerializeField] private ItemEffect m_itemEffect;

    public string ItemName => m_stringItemName;
    public Sprite Icon => m_spriteIcon;
    public int MaxStackSize => m_maxStackSize;
    public ItemEffect Effect => m_itemEffect;
}
