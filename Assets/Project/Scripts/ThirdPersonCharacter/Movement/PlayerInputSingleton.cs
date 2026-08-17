using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputSingleton : MonoBehaviour
{
    public static PlayerInputSingleton Instance { get; private set; }

    [SerializeField] private PlayerInput m_input;

    public InputActionAsset Actions => m_input.actions;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
