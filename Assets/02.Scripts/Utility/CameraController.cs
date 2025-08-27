using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    [SerializeField] private Vector2 _xRange;
    [SerializeField] private Vector2 _yRange;

    private float _mouseX;
    private float _mouseY;

    private void Start()
    {
        Vector3 init = transform.localEulerAngles;
        _mouseX = init.y;
        _mouseY = init.x;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
    }

    void Update()
    {
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
}
