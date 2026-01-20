using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float Speed = 5f;
    public float MouseSensitivity = 1f;
    [SerializeField] InputAction horizontal;
    [SerializeField] InputAction vertical;
    [SerializeField] InputAction mouseinput;
    [SerializeField] Camera playerCamera; // Assign in inspector
    float yaw = 0f;
    float pitch = 0f;
    Rigidbody rb;
    private GroundDetection groundDetection;
    private bool isGrounded;

    void OnEnable()
    {
        horizontal.Enable();
        vertical.Enable();
        mouseinput.Enable();
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        groundDetection = GetComponent<GroundDetection>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
        yaw = transform.eulerAngles.y;
        if (playerCamera != null)
            pitch = playerCamera.transform.localEulerAngles.x;
    }


    public void FixedUpdate()
    {
        if (groundDetection != null)
            isGrounded = groundDetection.IsGrounded();
        playermovement();
        playerotation();
        cameraupdown();
    }

    private void playermovement()
    {
        float h = horizontal.IsPressed() ? horizontal.ReadValue<float>() : 0f;
        float v = vertical.IsPressed() ? vertical.ReadValue<float>() : 0f;
        Vector3 localMove = new Vector3(h, 0f, v).normalized * Speed * Time.deltaTime;
        Vector3 worldMove = transform.TransformDirection(localMove);

        // Only move horizontally if grounded
        if (isGrounded)
        {
            worldMove.y = rb.linearVelocity.y;
            rb.linearVelocity = worldMove;
        }
        else
        {
            // Optionally, apply gravity or restrict movement when not grounded
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }

    private void playerotation()
    {
        Vector2 mouseinputvector = mouseinput.ReadValue<Vector2>();
        yaw += mouseinputvector.x * MouseSensitivity * Time.deltaTime;
        Quaternion targetrotation = Quaternion.Euler(0, yaw, 0);
        rb.MoveRotation(targetrotation);
    }

    private void cameraupdown()
    {
        if (playerCamera == null) return;
        Vector2 mouseinputvector = mouseinput.ReadValue<Vector2>();
        pitch -= mouseinputvector.y * MouseSensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, -90f, 90f);
        playerCamera.transform.localEulerAngles = new Vector3(pitch, 0f, 0f);
    }
}