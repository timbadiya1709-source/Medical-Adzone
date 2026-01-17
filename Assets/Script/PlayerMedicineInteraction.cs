using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRayInteract : MonoBehaviour
{
    public Camera playerCamera;
    public float maxDistance = 3f;
    public LayerMask interactableMask;
    [SerializeField] InputAction interact; // bind to E
    public UIController uiController;

    void OnEnable() => interact.Enable();
    void OnDisable() => interact.Disable();

    void Update()
    {
        if (playerCamera == null) return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, interactableMask))
        {
            // optional: show "Press E" prompt here
            if (interact.triggered)
            {
                string tag = hit.collider.tag;
                uiController?.ShowForTag(tag);
            }
        }
        else
        {
            // optional: hide prompt
            if (interact.triggered)
            {
                uiController?.HideAll();
            }
        }

        // optional: close UI with Escape
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            uiController?.HideAll();
    }
}
