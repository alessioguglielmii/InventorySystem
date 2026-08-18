using UnityEngine;

public class LongSword : MonoBehaviour
{
    [SerializeField] private int m_HitPower = 1;
    [SerializeField] private CharacterMovement m_wielder;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Chest"))
        {
            if (m_wielder != null && m_wielder.AttackOpen)
            {
                Chest _chest = other.GetComponent<Chest>();

                _chest.DamageChest(m_HitPower);
            }
        }    
    }
}
