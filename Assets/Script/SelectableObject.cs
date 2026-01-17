using UnityEngine;

public class SelectableObject : MonoBehaviour
{
    [Header("Object Information")]
    [TextArea(3, 6)]
    public string description = "Enter description here";
    
    public string objectName = "Object";

    // Optional: Add icon or image
    public Sprite icon;

    // Get the description
    public string GetDescription()
    {
        return description;
    }

    public string GetObjectName()
    {
        return objectName;
    }
}
