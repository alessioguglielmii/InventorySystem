using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] private int m_endurance;
    [SerializeField] private GameObject m_breakEffect;
    [SerializeField] private ItemPickup m_inventoryItem;

    public void DamageChest(int _hitPower)
    {
        m_endurance -= _hitPower;

        if (m_endurance <= 0)
        {
            DestroyChest();
        }
        else
        {
            Animator animator = GetComponent<Animator>();

            if (animator != null)
            {
                animator.SetTrigger("Hit");
            }
        }
    }
    
    public void DestroyChest()
    {
        if (m_breakEffect != null)
        {
            Instantiate(m_breakEffect, transform.position, transform.rotation);
        }

        if (m_inventoryItem != null)
        {
            Vector3 _itemPosition = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 1.5f, gameObject.transform.position.z);

            GameObject inventoryItem = Instantiate(m_inventoryItem.gameObject);
            inventoryItem.transform.position = _itemPosition;

        }

        gameObject.SetActive(false);
    }
}
