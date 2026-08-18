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
    [SerializeField] private float m_desiredArmLenght = 2;


    private float _veritcalInput;
    private float _horizontalInput;

    private float _currentArmLenght;

    private Vector2 _mouseLook;
    private bool _canMove = true;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Start()
    {
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
        _currentArmLenght = lenght;
        m_cameraPoint.localPosition = new Vector3(0, 0, -lenght);
    }

    private void HandleRotation()
    {
        transform.Rotate(Vector3.up, _mouseLook.x * m_horizontalSpeed * Time.deltaTime);

        m_elevation.Rotate(Vector3.right, _mouseLook.y * m_verticalSpeed * (m_invertMouse ? 1f : -1f) * Time.deltaTime);
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
