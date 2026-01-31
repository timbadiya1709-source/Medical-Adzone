using UnityEngine;
using UnityEngine.InputSystem;

public class Tourch : MonoBehaviour
{
    [SerializeField] GameObject lightSource;
    [SerializeField] InputAction lightaction;
    private bool ison = false;

    void OnEnable()
    {
        lightaction.Enable();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(lightaction.triggered)
        {
            if (!ison)
            {
            lightSource.SetActive(true);
            ison = true;
            }
            else
            {
                lightSource.SetActive(false);
                ison = false;
            }
        }
    }
}
