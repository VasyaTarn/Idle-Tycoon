using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraMovementController : MonoBehaviour
{
    [SerializeField] private SettingsManager _settingsManager;

    private float _horizontalInput;
    private float _verticalInput;

    private CinemachineVirtualCamera _camera;

    private void Start()
    {
        _camera = GetComponent<CinemachineVirtualCamera>();
    }

    private void Update()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");

        Move();

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        HandleZoom();
    }

    private void Move()
    {
        Vector3 direction = new Vector3(_horizontalInput, 0f, _verticalInput);
        Vector3 newPosition = transform.position + direction * _settingsManager.CameraSettings.MoveSpeed * Time.deltaTime;

        newPosition.x = Mathf.Clamp(newPosition.x, -30f, 40f);
        newPosition.z = Mathf.Clamp(newPosition.z, -60f, 30f);

        transform.position = newPosition;
    }

    private void HandleZoom()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput != 0)
        {
            float newFieldOfView = _camera.m_Lens.FieldOfView - scrollInput * _settingsManager.CameraSettings.ZoomSpeed;
            _camera.m_Lens.FieldOfView = Mathf.Clamp(newFieldOfView, _settingsManager.CameraSettings.MinZoom, _settingsManager.CameraSettings.MaxZoom);
        }
    }
}
