using UnityEngine;

public class PattientMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Vector3 targetPosition;        
    [SerializeField] private float speed = 5f;
    [SerializeField] private bool autoStart = true; // Auto start movement on game start
    
    private Rigidbody rb;
    private bool isMoving = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        if (rb == null)
        {
            Debug.LogError("Rigidbody component not found on " + gameObject.name);
            return;
        }
        
        // Configure Rigidbody for smooth movement
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.isKinematic = false;
        
        startPosition = transform.position;
        
        // Auto start for testing
        if (autoStart && targetPosition != Vector3.zero)
        {
            StartMovement(targetPosition);
            Debug.Log("Auto-starting movement to: " + targetPosition);
        }
    }

    void Update()
    {
        if (isMoving)
        {
            MoveToTarget();
        }
    }

    /// <summary>
    /// Starts moving the object from current position to target position
    /// </summary>
    public void StartMovement(Vector3 newTargetPosition)
    {
        targetPosition = newTargetPosition;
        isMoving = true;
        Debug.Log(gameObject.name + " starting movement to: " + targetPosition);
    }

    /// <summary>
    /// Moves the object smoothly towards target position using Rigidbody
    /// </summary>
    private void MoveToTarget()
    {
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
        
        // Check if reached destination (increased threshold for better detection)
        if (distanceToTarget < 0.1f)
        {
            rb.linearVelocity = Vector3.zero;
            isMoving = false;
            OnReachedDestination();
            return;
        }
        
        // Calculate direction and move
        Vector3 direction = (targetPosition - transform.position).normalized;
        Vector3 velocity = direction * speed;
        
        // Apply velocity through Rigidbody
        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
    }

    /// <summary>
    /// Callback function when object reaches destination
    /// Override this or subscribe to an event based on your needs
    /// </summary>
    private void OnReachedDestination()
    {
        Debug.Log(gameObject.name + " reached destination at: " + targetPosition);
        // Add your own logic here
    }

    /// <summary>
    /// Returns if object is currently moving
    /// </summary>
    public bool IsMoving()
    {
        return isMoving;
    }

    /// <summary>
    /// Stops the movement immediately
    /// </summary>
    public void StopMovement()
    {
        isMoving = false;
        rb.linearVelocity = Vector3.zero;
    }
}
