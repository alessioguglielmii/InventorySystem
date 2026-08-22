using UnityEngine;
using UnityEngine.InputSystem;

public class Chest : MonoBehaviour
{
    [Header("Break")]
    [SerializeField] private int m_endurance;
    [SerializeField] private GameObject m_breakEffect;

    [Header("Item")]
    [SerializeField] private ItemPickup m_inventoryItem;

    [Header("Animation")]
    [SerializeField] private Animator m_animator;

    [Header("Audio")]
    [SerializeField] private AudioSource m_audioSource;
    [SerializeField] private AudioClip m_chestHit;
    [SerializeField] private AudioClip m_chestBreak;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
    }

    public void DamageChest(int hitPower)
    {
        m_endurance -= hitPower;

        if (m_endurance <= 0)
        {
            DestroyChest();
        }
        else
        {
            if (m_animator != null)
            {
                m_animator.SetTrigger("Hit");

                if (m_audioSource != null && m_chestHit != null)
                {
                    m_audioSource.PlayOneShot(m_chestHit);
                }
            }
        }
    }
    
    public void DestroyChest()
    {
        if (m_breakEffect != null)
        {
            Instantiate(m_breakEffect, transform.position, transform.rotation);
        }

        if(m_audioSource != null && m_chestBreak != null)
        {
            GameObject audioObject = new GameObject("ChestBreakAudio");

            audioObject.transform.position = transform.position;

            AudioSource audioSource = audioObject.AddComponent<AudioSource>();

            audioSource.clip = m_chestBreak;
            audioSource.outputAudioMixerGroup = m_audioSource.outputAudioMixerGroup;
            audioSource.spatialBlend = m_audioSource.spatialBlend;
            audioSource.minDistance = m_audioSource.minDistance;
            audioSource.maxDistance = m_audioSource.maxDistance;
            audioSource.volume = m_audioSource.volume;
            audioSource.reverbZoneMix = m_audioSource.reverbZoneMix;
            audioSource.rolloffMode = m_audioSource.rolloffMode;

            audioSource.Play();

            Destroy(audioObject, m_chestBreak.length);
        }

        if (m_inventoryItem != null)
        {
            Vector3 itemPosition = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 1.5f, gameObject.transform.position.z);

            GameObject inventoryItem = Instantiate(m_inventoryItem.gameObject);
            inventoryItem.transform.position = itemPosition;

        }

        Destroy(gameObject);
    }
}
