using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    private const float MIN_MOVE_SPEED = 0.1f;

    [SerializeField] private float m_walkSpeed;
    [SerializeField] private float m_runSpeed;
    [SerializeField] private float m_acceleration;
    [SerializeField] private CharacterController m_characterController;
    [SerializeField] private Animator m_animator;
    [SerializeField] private Transform m_cameraPivot;
    [SerializeField] private float m_rotationSpeed;

    private Vector3 _currentSpeed;
    private Vector3 _wantedSpeed;

    private float _speedMagnitude;

    private Vector2 _moveInput;

    private bool _attackOpen;
    private bool _canPickUp;
    private ItemPickup _itemPickUp;

    public bool AttackOpen => _attackOpen;

    private void Start()
    {
        _speedMagnitude = m_walkSpeed;
        PlayerInputSingleton.Instance.Actions["Move"].performed += OnMoveStart;
        PlayerInputSingleton.Instance.Actions["Move"].canceled += OnMoveEnd;
        PlayerInputSingleton.Instance.Actions["Sprint"].started += OnSprintStart;
        PlayerInputSingleton.Instance.Actions["Sprint"].canceled += OnSprintEnd;
        PlayerInputSingleton.Instance.Actions["Attack"].performed += OnAttackInput;
        PlayerInputSingleton.Instance.Actions["PickUp"].performed += OnPickUpInput;
    }

    private void OnDestroy()
    {
        PlayerInputSingleton.Instance.Actions["Move"].performed -= OnMoveStart;
        PlayerInputSingleton.Instance.Actions["Move"].canceled -= OnMoveEnd;
        PlayerInputSingleton.Instance.Actions["Sprint"].started -= OnSprintStart;
        PlayerInputSingleton.Instance.Actions["Sprint"].canceled -= OnSprintEnd;
        PlayerInputSingleton.Instance.Actions["Attack"].performed -= OnAttackInput;
        PlayerInputSingleton.Instance.Actions["PickUp"].performed -= OnPickUpInput;
    }

    private void Update()
    {
        UpdateMovementInput();
        OrientCharacterToCamera();
        UpdateAnimator();
    }

    private void OnAnimatorMove()
    {
        Vector3 _motion = m_animator.deltaPosition;

        _motion.y = 0f;

        m_characterController.Move(_motion);

        transform.rotation = m_animator.rootRotation;
    }

    private void OnMoveStart(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    private void OnMoveEnd(InputAction.CallbackContext context)
    {
        _moveInput = Vector2.zero;
    }

    private void OnSprintStart(InputAction.CallbackContext context)
    {
        _speedMagnitude = m_runSpeed;
    }

    private void OnSprintEnd(InputAction.CallbackContext context)
    {
        _speedMagnitude = m_walkSpeed;
    }

    private void OnAttackInput(InputAction.CallbackContext context)
    {
        m_animator.SetTrigger("Attack");
    }

    private void OnPickUpInput(InputAction.CallbackContext context)
    {
        m_animator.SetTrigger("PickUp");
    }

    private void UpdateMovementInput()
    {
        _wantedSpeed.x = _moveInput.x * _speedMagnitude;

        _wantedSpeed.z = _moveInput.y * _speedMagnitude;

        _currentSpeed = Vector3.MoveTowards(_currentSpeed, _wantedSpeed, m_acceleration * Time.deltaTime);
    }

    private void OrientCharacterToCamera()
    {

        if (_moveInput.sqrMagnitude < MIN_MOVE_SPEED * MIN_MOVE_SPEED)
        {
            return;
        }

        Vector3 _cameraForward = m_cameraPivot.forward;

        _cameraForward.y = 0f;

        if (_cameraForward.sqrMagnitude < 0.001f)
        {
            return;
        }

        _cameraForward.Normalize();

        Quaternion _targetRotation = Quaternion.LookRotation(_cameraForward);

        transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, 1f - Mathf.Exp(-m_rotationSpeed * Time.deltaTime));
    }

    private void UpdateAnimator()
    {
        m_animator.SetFloat("SpeedZ", _currentSpeed.z);
        m_animator.SetFloat("SpeedX", _currentSpeed.x);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Inventory Item"))
        {
            _canPickUp = true;
            _itemPickUp = other.GetComponent<ItemPickup>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Inventory Item"))
        {
            _canPickUp = false;
            _itemPickUp = null;
        }
    }

    public void OnAttackOpen()
    {
        _attackOpen = true;
    }

    public void OnAttackClosed()
    {
        _attackOpen = false;
    }

    public void OnGrabItem()
    {
        if (_canPickUp && _itemPickUp != null)
        {
            Inventory _inventory = GetComponent<Inventory>();

            if (_inventory == null)
            {
                return;
            }

            _itemPickUp.CollectItem(_inventory);
        }
    }
}
