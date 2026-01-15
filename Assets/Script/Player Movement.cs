using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float Speed = 150f;
    [SerializeField] InputAction horizontal;
    [SerializeField] InputAction vertical;  
    Rigidbody rb;
    void OnEnable()
    {
        horizontal.Enable();
        vertical.Enable();
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    void FixedUpdate()
    {
        if (horizontal.IsPressed())
        {
            rb.linearVelocity = new Vector3(horizontal.ReadValue<float>(), 0, 0) * Speed * Time.deltaTime;
        }
        if (vertical.IsPressed())
        {
            rb.linearVelocity = new Vector3(0, 0, vertical.ReadValue<float>()* Speed * Time.deltaTime);
        }
        if (horizontal.IsPressed() && vertical.IsPressed())
        {
            rb.linearVelocity = new Vector3(horizontal.ReadValue<float>(), 0, vertical.ReadValue<float>()) * Speed * Time.deltaTime;
        }
            else if (!horizontal.IsPressed() && !vertical.IsPressed())
        {
            rb.linearVelocity = Vector3.zero;
        }
    }
}