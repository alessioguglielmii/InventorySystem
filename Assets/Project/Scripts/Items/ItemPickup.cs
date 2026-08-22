using TMPro;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("Item")]
    [SerializeField] private ItemData m_dataItem;

    [Header("Effect")]
    [SerializeField] private float m_floatingHeight = 0.25f;
    [SerializeField] private float m_floatingSpeed = 1.5f;
    [SerializeField] private float m_rotationSpeed = 60.0f;

    private float _initialPositionY;

    public void Start()
    {
        _initialPositionY = transform.position.y;
    }

    public void Update()
    {
        FloatItem();
    }

    public bool CheckItem(Inventory inventory)
    {
        return inventory.HasSpaceForItem(m_dataItem);
    }

    public void CollectItem(Inventory inventory)
    {
        bool bAdded = inventory.AddItem(m_dataItem);

        if (!bAdded)
        {
            return;
        }

        Destroy(gameObject);
    }

    private void FloatItem()
    {
        float offset = Mathf.Sin(Time.time * m_floatingSpeed) * m_floatingHeight;

        transform.position = new Vector3(transform.position.x, _initialPositionY + offset, transform.position.z);

        transform.Rotate(0.0f, m_rotationSpeed * Time.deltaTime, 0.0f);
    }
}
