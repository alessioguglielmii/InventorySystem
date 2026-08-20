using System.Collections.Generic;
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
    [SerializeField] private GameObject m_weaponMesh;
    [SerializeField] private float m_invisibilityTime;

    [SerializeField] private Transform m_bombSocket;
    [SerializeField] private GameObject m_bombObject;
    [SerializeField] private float m_bombForce = 30.0f;
    [SerializeField] private float m_bombUpwardForce = 0.01f;

    [SerializeField] private AudioSource m_audioSource;
    [SerializeField] private AudioClip[] m_footspepsClips;
    [SerializeField] private AudioClip[] m_swordClips;
    [SerializeField] private AudioClip m_itemGrabClip;
    [SerializeField] private AudioClip m_bombThrowClip;

    private Vector3 _currentSpeed;
    private Vector3 _wantedSpeed;

    private float _speedMagnitude;

    private Vector2 _moveInput;

    private bool _attackOpen;
    private bool _canPickUp;
    private ItemPickup _itemPickUp;

    private Unlockable _unlockable;

    private bool _canMove = true;
    private bool _isAttacking = false;
    private bool _isGrabbing = false;
    private bool _isThrowing = false;

    private Dictionary<string, Material> _characterMaterials = new();
    private Material _weaponMaterial;

    private float _invisibilityCurrentTime;

    private bool _throwingBomb = false;
    private GameObject _goBomb;

    public bool AttackOpen => _attackOpen;
    public Unlockable Unlockable => _unlockable;

    private void Start()
    {
        _speedMagnitude = m_walkSpeed;
        PlayerInputSingleton.Instance.Actions["Move"].performed += OnMoveStart;
        PlayerInputSingleton.Instance.Actions["Move"].canceled += OnMoveEnd;
        PlayerInputSingleton.Instance.Actions["Sprint"].started += OnSprintStart;
        PlayerInputSingleton.Instance.Actions["Sprint"].canceled += OnSprintEnd;
        PlayerInputSingleton.Instance.Actions["Attack"].performed += OnAttackInput;
        PlayerInputSingleton.Instance.Actions["PickUp"].performed += OnPickUpInput;

        foreach (Transform child in gameObject.transform)
        {
            SkinnedMeshRenderer meshRenderer = child.GetComponent<SkinnedMeshRenderer>();

            if (meshRenderer != null)
            {
                _characterMaterials.Add(child.name, meshRenderer.materials[0]);


            }
        }

        MeshRenderer weaponMeshRenderer = m_weaponMesh.GetComponent<MeshRenderer>();

        if (weaponMeshRenderer != null)
        {
            _weaponMaterial = weaponMeshRenderer.materials[0];
        }

        _invisibilityCurrentTime = 0;
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

        if (_invisibilityCurrentTime > 0)
        {
            _invisibilityCurrentTime -= Time.deltaTime;
        }
        else
        {
            _invisibilityCurrentTime = 0;
            EndInvisibility();
        }
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
        if (_canMove)
        {
            _moveInput = context.ReadValue<Vector2>();
        }
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
        if (_canMove && !_isAttacking && !_isGrabbing && !_isThrowing)
        {
            _isAttacking = true;
            m_animator.SetTrigger("Attack");
        }
    }

    private void OnPickUpInput(InputAction.CallbackContext context)
    {
        if (!_canMove || _isGrabbing || _isAttacking || _isThrowing)
        {
            return;
        }

        if (!_canPickUp || _itemPickUp == null)
        {
            return;
        }

        Inventory inventory = GetComponent<Inventory>();

        if (inventory == null)
        {
            return;
        }

        if (!_itemPickUp.CheckItem(inventory))
        {
            return;
        }

        _isGrabbing = true;
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

        if (other.CompareTag("Unlockable"))
        {
            Unlockable compUnlockable = other.GetComponent<Unlockable>();

            if (compUnlockable == null)
            {
                return;
            }

            _unlockable = compUnlockable;
            _unlockable.CanBeUnlocked = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Inventory Item"))
        {
            _canPickUp = false;
            _itemPickUp = null;
        }

        if (other.CompareTag("Unlockable"))
        {
            Unlockable compUnlockable = other.GetComponent<Unlockable>();

            if (compUnlockable == null)
            {
                return;
            }

            compUnlockable.CanBeUnlocked = false;

            if (_unlockable == compUnlockable)
            {
                _unlockable = null;
            }
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

    public void OnAttackEnded()
    {
        _isAttacking = false;
    }

    public void OnGrabItem()
    {
       Inventory inventory = GetComponent<Inventory>();

        if (inventory == null)
        {
            return;
        }

        _itemPickUp.CollectItem(inventory);

        m_audioSource.PlayOneShot(m_itemGrabClip);
    }

    public void OnGrabEnded()
    {
        _isGrabbing = false;
    }

    public void OnMoveChanged()
    {
        _canMove = !_canMove;

        if (!_canMove)
        {
            _moveInput = Vector3.zero;
        }
    }

    public void ThrowBomb()
    {
        if (_throwingBomb)
        {
            return;
        }

        if (m_bombObject == null)
        {
            _throwingBomb = false;

            return;
        }

        if (m_bombSocket == null)
        {
           _throwingBomb = false;

           return;
        }

        _goBomb = Instantiate(m_bombObject, m_bombSocket.position, m_bombSocket.rotation);

        Bomb bomb = _goBomb.GetComponent<Bomb>();
        bomb.bombSocket = m_bombSocket;

        _isThrowing = true;

        m_animator.SetTrigger("Throw");

        _throwingBomb = true;
    }

    public void OnThrowBomb()
    {
        Bomb bomb = _goBomb.GetComponent<Bomb>();
        bomb.isThrown = true;

        if(m_audioSource != null && m_bombThrowClip != null)
        {
            m_audioSource.PlayOneShot(m_bombThrowClip);
        }        

        Rigidbody rigidbody = _goBomb.GetComponent<Rigidbody>();

        if (rigidbody != null)
        {
            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;

            Vector3 throwDirection = transform.forward;
            throwDirection.y = m_bombUpwardForce;
            throwDirection.Normalize();

            rigidbody.AddForce(throwDirection * m_bombForce, ForceMode.Impulse);
        }

        _throwingBomb = false;
    }

    public void OnThrowBombEnded()
    {
        _isThrowing = false;
    }

    public void StartInvisibility(Material invisibilityMaterial)
    {
        foreach (Transform child in gameObject.transform)
        {
            SkinnedMeshRenderer meshRenderer = child.GetComponent<SkinnedMeshRenderer>();

            if (meshRenderer != null)
            {
                Material[] materials = meshRenderer.materials;
                materials[0] = invisibilityMaterial;
                meshRenderer.materials = materials;
            }
        }

        MeshRenderer weaponMeshRenderer = m_weaponMesh.GetComponent<MeshRenderer>();

        if (weaponMeshRenderer != null)
        {
            Material[] materials = weaponMeshRenderer.materials;
            materials[0] = invisibilityMaterial;
            weaponMeshRenderer.materials = materials;
        }

        _invisibilityCurrentTime += m_invisibilityTime;
    }

    public void EndInvisibility()
    {
        foreach (Transform child in gameObject.transform)
        {
            SkinnedMeshRenderer meshRenderer = child.GetComponent<SkinnedMeshRenderer>();

            if (meshRenderer != null)
            {
                Material[] materials = meshRenderer.materials;
                Material characterMaterial;
                _characterMaterials.TryGetValue(child.name, out characterMaterial);
                materials[0] = characterMaterial;
                meshRenderer.materials = materials;
            }
        }

        MeshRenderer weaponMeshRenderer = m_weaponMesh.GetComponent<MeshRenderer>();

        if (weaponMeshRenderer != null)
        {
            Material[] materials = weaponMeshRenderer.materials;
            materials[0] = _weaponMaterial;
            weaponMeshRenderer.materials = materials;
        }
    }

    public void OnFootSound()
    {
        int audioIndex = Random.Range(0, m_footspepsClips.Length);
        float lowerVolume = 0.8f;
        float upperVolume = 1.2f;
        float lowerPitch = 0.8f;
        float upperPitch = 1.2f;

        m_audioSource.volume = Random.Range(lowerVolume, upperVolume);
        m_audioSource.pitch = Random.Range(lowerPitch, upperPitch);
        m_audioSource.PlayOneShot(m_footspepsClips[audioIndex]);
    }

    public void OnSwordSound()
    {
        int audioIndex = Random.Range(0, m_swordClips.Length);

        m_audioSource.PlayOneShot(m_swordClips[audioIndex]);
    }

}
