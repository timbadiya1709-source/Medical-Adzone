using UnityEngine;

public class GroundDetection : MonoBehaviour
{
    [Header("Ground Detection Settings")]
    [SerializeField] public Transform groundCheck; // Assign a child transform at player's feet
    [SerializeField] public float groundDistance = 0.3f;
    [SerializeField] public LayerMask groundMask;
    private bool isGrounded;

    void Start()
    {
        if (groundCheck == null)
        {
            // Create a groundCheck transform at the bottom of the capsule if not assigned
            GameObject gc = new GameObject("GroundCheck");
            gc.transform.SetParent(transform);
            gc.transform.localPosition = new Vector3(0, -0.5f, 0); // Adjust for capsule height
            groundCheck = gc.transform;
        }
    }

    void FixedUpdate()
    {
        CheckGround();
    }

    /// <summary>
    /// Performs ground detection using a sphere check
    /// </summary>
    private void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    /// <summary>
    /// Returns whether the object is currently grounded
    /// </summary>
    public bool IsGrounded()
    {
        return isGrounded;
    }
}
