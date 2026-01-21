using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public float MouseSensitivity = 1f;
    [SerializeField] InputAction mouseinput;
    float pitch = 0f;
    float yaw = 0f;
    Camera cam;

    void OnEnable()
    {
        mouseinput.Enable();
    }

    void Start()
    {
        cam = GetComponent<Camera>();
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
        if (cam != null)
        {
            pitch = cam.transform.localEulerAngles.x;
            yaw = cam.transform.localEulerAngles.y;
        }
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed)
        {
            CameraLook();
        }
    }

    private void CameraLook()
    {
        if (cam == null) return;
        Vector2 mouseinputvector = mouseinput.ReadValue<Vector2>();
        yaw += mouseinputvector.x * MouseSensitivity * Time.deltaTime;
        pitch -= mouseinputvector.y * MouseSensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, -90f, 90f);
        cam.transform.localEulerAngles = new Vector3(pitch, yaw, 0f);
    }
}
