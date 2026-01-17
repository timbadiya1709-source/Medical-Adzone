using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Header("Info for UI")]
    public string title = "Item";
    // [TextArea] public string description = "Description here.";
    public Sprite icon; // optional

    // Optional callback when selected
    public void OnSelected()
    {
        // Add any object-specific logic here (play sound, disable physics, etc.)
        Debug.Log($"{name} selected.");
    }
}
