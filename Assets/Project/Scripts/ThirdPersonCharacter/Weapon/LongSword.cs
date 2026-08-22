using UnityEngine;

public class LongSword : MonoBehaviour
{
    [Header("Playable Character")]
    [SerializeField] private CharacterMovement m_wielder;

    [Header("Effect")]
    [SerializeField] private int m_HitPower = 1;    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Chest"))
        {
            if (m_wielder != null && m_wielder.AttackOpen)
            {
                Chest chest = other.GetComponent<Chest>();

                chest.DamageChest(m_HitPower);
            }
        }    
    }
}
