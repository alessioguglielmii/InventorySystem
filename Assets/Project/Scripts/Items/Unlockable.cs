using UnityEngine;

public class Unlockable : MonoBehaviour
{
    [SerializeField] private Animator m_animator;
    [SerializeField] private bool m_isUnlocked = false;

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
        m_animator.SetTrigger("Open");
    }
}
