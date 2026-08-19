using Unity.VisualScripting;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private float m_explosionRadius = 3.0f;
    [SerializeField] private GameObject m_explosionEffect;
    
    [HideInInspector] public bool isThrown = false;
    [HideInInspector] public Transform bombSocket;

    private void Update()
    {
        if (!isThrown)
        {
            transform.position = bombSocket.position;
            transform.rotation = bombSocket.rotation;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (m_explosionEffect != null)
        {
            Instantiate(m_explosionEffect, transform.position, transform.rotation);
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, m_explosionRadius);

        foreach (Collider collider in colliders)
        {
            Chest chest = collider.gameObject.GetComponent<Chest>();

            if (chest != null)
            {
                chest.DestroyChest();
            }
        }

        Destroy(gameObject);
    }
}
