using System.Linq;  // Add this line here
using System.Collections.Generic;
using UnityEngine;

public class MachineUIInputSlot : MonoBehaviour
{
    public MachineUIDragItem.ItemType acceptedType;
    public MachineUIDragItem currentItem;
    
    public System.Action onContentChanged;

    public bool CanAccept(MachineUIDragItem item)
    {
        if (currentItem != null) return false;
        if (item.itemType != acceptedType) return false;

        // Check for duplicates in the same panel/machine
        MachineUIPanel panel = GetComponentInParent<MachineUIPanel>();
        if (panel != null)
        {
            // Check all slots in the panel for the same itemID
            bool alreadyInMaterialSlots = panel.materialSlots.Any(s => s.currentItem != null && s.currentItem.itemID == item.itemID);
            bool alreadyInApiSlot = panel.apiSlot != null && panel.apiSlot.currentItem != null && panel.apiSlot.currentItem.itemID == item.itemID;
            
            if (alreadyInMaterialSlots || alreadyInApiSlot)
            {
                Debug.LogWarning($"[UI] Machine already contains {item.itemID}!");
                return false;
            }
        }

        return true;
    }

    public void AcceptItem(MachineUIDragItem item)
    {
        currentItem = item;
        // We do NOT reparent the item — it stays at its spawn position
        // The slot just tracks what was dropped into it
        onContentChanged?.Invoke();
    }

    public void ClearSlot()
    {
        if (currentItem != null)
        {
            currentItem.ReturnToSpawn(); // returns item to its home position
            currentItem = null;
        }
    }
}
