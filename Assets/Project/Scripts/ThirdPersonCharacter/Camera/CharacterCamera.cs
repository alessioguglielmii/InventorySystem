using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterCamera : MonoBehaviour
{

    [SerializeField] private Transform m_target;
    [SerializeField] private Transform m_elevation;
    [SerializeField] private Transform m_cameraPoint;
    [SerializeField] private Vector3 m_offset;
    [SerializeField] private float m_horizontalSpeed;
    [SerializeField] private float m_verticalSpeed;
    [SerializeField] private bool m_invertMouse;
    [SerializeField] private LayerMask m_collisionMask;
    [SerializeField] private float m_desiredArmLenght = 2.0f;
    [SerializeField] private float m_minVerticalAngle = -60.0f;
    [SerializeField] private float m_maxVerticalAngle = 45.0f;

    private float _horizontalRotation;
    private float _verticalRotation;

    private Vector2 _mouseLook;
    private bool _canMove = true;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Start()
    {
        _horizontalRotation = transform.eulerAngles.y;

        _verticalRotation = m_elevation.localEulerAngles.x;

        if (_verticalRotation > 180.0f)
        {
            _verticalRotation -= 360.0f;
        }

        PlayerInputSingleton.Instance.Actions["Look"].performed += OnLookPerformed;
        PlayerInputSingleton.Instance.Actions["Look"].canceled += OnLookCanceled;
    }

    private void Update()
    {
        HandleRotation();
        IsCameraOccluded();
    }

    private void LateUpdate()
    {
        transform.position = GetWantedPosition();
    }

    private void OnLookPerformed(InputAction.CallbackContext context)
    {
        if (_canMove)
        {
            _mouseLook = context.ReadValue<Vector2>();
        }
    }

    private void OnLookCanceled(InputAction.CallbackContext context)
    {
        _mouseLook = Vector2.zero;
    }

    private Vector3 GetWantedPosition()
    {
        return m_target.position + m_offset;
    }

    private void SetArmLength(float lenght)
    {
        m_cameraPoint.localPosition = new Vector3(0, 0, -lenght);
    }

    private void HandleRotation()
    {
        float horizontalInput = _mouseLook.x * m_horizontalSpeed * Time.deltaTime;

        _horizontalRotation += horizontalInput;

        transform.rotation = Quaternion.Euler(0.0f, _horizontalRotation, 0.0f);

        float verticalInput = _mouseLook.y * m_verticalSpeed * (m_invertMouse ? 1.0f : -1.0f) * Time.deltaTime;

        _verticalRotation += verticalInput;

        _verticalRotation = Mathf.Clamp(_verticalRotation, m_minVerticalAngle, m_maxVerticalAngle);

        m_elevation.localRotation = Quaternion.Euler(_verticalRotation, 0.0f, 0.0f);
    }

    private void IsCameraOccluded()
    {
        RaycastHit hit;
        Ray ray = new Ray(m_target.position + m_offset, m_cameraPoint.position - (m_target.position + m_offset));
        if (Physics.SphereCast(ray, 0.25f, out hit, m_desiredArmLenght, m_collisionMask))
        {
            SetArmLength(hit.distance);
        }
        else
        {
            SetArmLength(m_desiredArmLenght);
        }
    }

    public void OnMoveChanged()
    {
        _canMove = !_canMove;
    }
}
