using UnityEngine;

public class PatientMovement : MonoBehaviour
{
    [SerializeField]Vector3 Target;
    [SerializeField] float Speed = 20f;
    BoxCollider boxcollider;

    void Start()
    {
        boxcollider = GetComponent<BoxCollider> ();
        boxcollider.enabled = false;
    }
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, Target, Speed * Time.deltaTime);

        if (transform.position == Target)
        {
            boxcollider.enabled = true;
        }
        else
        {
            boxcollider.enabled = false;
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        
    }
}
