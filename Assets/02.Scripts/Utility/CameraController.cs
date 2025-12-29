using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }
    public float mouseSensitivity = 100f;
    [SerializeField] private Vector2 _xRange;
    [SerializeField] private Vector2 _yRange;

    private float _mouseX;
    private float _mouseY;
    private Vector3 _defaultAngles;
    private bool _isLock = false;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _defaultAngles = transform.localEulerAngles;
        Vector3 init = _defaultAngles;
        _mouseX = init.y;
        _mouseY = init.x;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (_isLock) return;
        RotateCamera();
    }

    void RotateCamera()
    {
        _mouseX += Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
        _mouseY -= Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;

        _mouseX = Mathf.Clamp(_mouseX, _xRange.x, _xRange.y);
        _mouseY = Mathf.Clamp(_mouseY, _yRange.x, _yRange.y);

        transform.localRotation = Quaternion.Euler(_mouseY, _mouseX, 0f);
    }

    public void LockCamera()
    {
        _isLock = true;
        _mouseX = _defaultAngles.y;
        _mouseY = _defaultAngles.x;
        transform.localRotation = Quaternion.Euler(_mouseY, _mouseX, 0f);
    }

    public void UnLockCamera()
    {
        _isLock = false;
        _mouseX = _defaultAngles.y;
        _mouseY = _defaultAngles.x;
        transform.localRotation = Quaternion.Euler(_mouseY, _mouseX, 0f);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    public void SetCursorFree()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
