using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float Speed = 5f;
    [SerializeField] InputAction horizontal;
    [SerializeField] InputAction vertical;
    Rigidbody rb;
    private GroundDetection groundDetection;
    private bool isGrounded;

    void OnEnable()
    {
        horizontal.Enable();
        vertical.Enable();
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        groundDetection = GetComponent<GroundDetection>();
    }

    public void FixedUpdate()
    {
        if (groundDetection != null)
            isGrounded = groundDetection.IsGrounded();
        playermovement();
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
}