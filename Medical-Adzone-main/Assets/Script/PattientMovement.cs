using UnityEngine;

public class PatientMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private Vector3 entryPosition;
    [SerializeField] private Vector3 treatmentPosition; // Where patient waits for treatment
    [Tooltip("World position the patient moves to before being destroyed. Ignored if Exit Point is set.")]
    [SerializeField] private Vector3 exitPosition;
    [Tooltip("Optional. Use this Transform's position as the exit point instead of Exit Position.")]
    [SerializeField] private Transform exitPoint;
    [SerializeField] private float speed = 5f;
    
    [Header("Requirement Panel")]
    [SerializeField] private GameObject requirementPanel;
    
    [Header("Auto Start")]
    [SerializeField] private bool autoStart = true;
    
    private Rigidbody rb;
    private bool isMoving = false;
    private Vector3 currentTarget;
    private PatientState currentState = PatientState.Entry;
    
    private enum PatientState
    {
        Entry,      // Moving to treatment position
        Waiting,    // Waiting for requirements to be fulfilled
        Exiting,    // Moving to exit
        Done        // Reached exit, ready to be destroyed
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        if (rb == null)
        {
            Debug.LogError("Rigidbody component not found on " + gameObject.name);
            return;
        }
        
        // Configure Rigidbody
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.isKinematic = false;
        
        // Set starting position
        transform.position = entryPosition;
        
        // Hide requirement panel initially
        if (requirementPanel != null)
            requirementPanel.SetActive(false);
        
        // Auto start entry
        if (autoStart)
        {
            StartEntry();
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
    /// Starts the patient entry sequence
    /// </summary>
    public void StartEntry()
    {
        currentState = PatientState.Entry;
        currentTarget = treatmentPosition;
        isMoving = true;
        Debug.Log(gameObject.name + " entering to treatment position");
    }

    /// <summary>
    /// Called by RequirementManager when all requirements are fulfilled
    /// </summary>
    public void StartExit()
    {
        if (currentState != PatientState.Waiting)
            return;

        currentState = PatientState.Exiting;
        currentTarget = exitPoint != null ? exitPoint.position : exitPosition;
        isMoving = true;

        if (requirementPanel != null)
            requirementPanel.SetActive(false);

        Debug.Log(gameObject.name + " requirements fulfilled, exiting to " + currentTarget);
    }

    /// <summary>
    /// Moves the patient smoothly towards current target
    /// </summary>
    private void MoveToTarget()
    {
        float distanceToTarget = Vector3.Distance(transform.position, currentTarget);
        
        // Check if reached destination
        if (distanceToTarget < 0.1f)
        {
            rb.linearVelocity = Vector3.zero;
            isMoving = false;
            OnReachedDestination();
            return;
        }
        
        // Calculate direction and move
        Vector3 direction = (currentTarget - transform.position).normalized;
        Vector3 velocity = direction * speed;
        
        // Apply velocity (maintain Y velocity for gravity)
        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
    }

    /// <summary>
    /// Called when patient reaches current destination
    /// </summary>
    private void OnReachedDestination()
    {
        switch (currentState)
        {
            case PatientState.Entry:
                // Reached treatment position, show panel and wait
                currentState = PatientState.Waiting;
                if (requirementPanel != null)
                    requirementPanel.SetActive(true);
                Debug.Log(gameObject.name + " reached treatment position, waiting for requirements");
                break;
                
            case PatientState.Exiting:
                currentState = PatientState.Done;
                Debug.Log(gameObject.name + " reached exit, destroying");
                Destroy(gameObject);
                break;
        }
    }

    /// <summary>
    /// Returns current state of patient
    /// </summary>
    public bool IsWaiting()
    {
        return currentState == PatientState.Waiting;
    }

    /// <summary>
    /// Stops movement immediately
    /// </summary>
    public void StopMovement()
    {
        isMoving = false;
        rb.linearVelocity = Vector3.zero;
    }
}