using UnityEngine;

public class Unlockable : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private bool m_isUnlocked = false;
    
    [Header("Animation")]
    [SerializeField] private Animator m_animator;

    [Header("Audio")]
    [SerializeField] private AudioSource m_audioSource;
    [SerializeField] private AudioClip m_openGateClip;

    public bool IsUnlocked => m_isUnlocked;
    [HideInInspector] public bool CanBeUnlocked = false;

    public bool Unlock()
    {
        if (m_isUnlocked || !CanBeUnlocked)
        {
            return false;
        }

        m_isUnlocked = true;

        OperDoor();

        return true;
    }

    private void OperDoor()
    {
        if (m_audioSource != null && m_openGateClip != null)
        {
            m_audioSource.PlayOneShot(m_openGateClip);
        }

        m_animator.SetTrigger("Open");
    }
}
