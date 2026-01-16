using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float Speed = 150f;
    [SerializeField] InputAction horizontal;
    [SerializeField] InputAction vertical;
    [SerializeField] InputAction mouseinput;
    float yaw = 0f;
    Rigidbody rb;
    void OnEnable()
    {
        horizontal.Enable();
        vertical.Enable();
        mouseinput.Enable();
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
        yaw = transform.eulerAngles.y;
    }


    public void FixedUpdate()
    {
        playermovement();
        playerotation();
    }

    private void playermovement()
    {
        float h = horizontal.IsPressed() ? horizontal.ReadValue<float>() : 0f;
        float v = vertical.IsPressed() ? vertical.ReadValue<float>() : 0f;
        Vector3 localMove = new Vector3(h, 0f, v).normalized * Speed * Time.deltaTime;
        Vector3 worldMove = transform.TransformDirection(localMove);
        worldMove.y = rb.linearVelocity.y;
        rb.linearVelocity = worldMove;
    }


    private void playerotation()
    {
        Vector2 mouseinputvector = mouseinput.ReadValue<Vector2>();
        yaw += mouseinputvector.x;
        Quaternion targetrotation = Quaternion.Euler(0, yaw * Time.deltaTime, 0);
        rb.MoveRotation(targetrotation);
    }
}