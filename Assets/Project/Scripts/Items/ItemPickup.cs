using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private ItemData m_dataItem;

    public void CollectItem(Inventory inventory)
    {
        bool bAdded = inventory.AddItem(m_dataItem);

        if (!bAdded)
        {
            return;
        }

        Destroy(gameObject);
    }
}
