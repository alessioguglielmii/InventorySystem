using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] private int m_endurance;
    [SerializeField] private ItemPickup m_inventoryItem;

    public void DamageChest(int _hitPower)
    {
        m_endurance -= _hitPower;

        if (m_endurance <= 0)
        {
            if (m_inventoryItem != null)
            {
                Vector3 _itemPosition = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 1.0f, gameObject.transform.position.z);

                Instantiate(m_inventoryItem.gameObject, _itemPosition, Quaternion.identity);
            }

            gameObject.SetActive(false);
        }
    }    
}
