
using UnityEngine;


/// <summary>
/// Attach to your table GameObject - Manages medicine placement at a specific spot
/// </summary>
using System.Collections.Generic;
public class TablePlacement : MonoBehaviour
{
    [Tooltip("Placement spot on the table")]
    public Transform placementSpot; // Assign in Inspector

    private List<GameObject> placedMedicines = new List<GameObject>();

    /// <summary>
    /// Returns the next position where the medicine should be placed.
    /// </summary>
    public Vector3 GetNextPlacementPosition()
    {
        if (placementSpot != null)
            return placementSpot.position;
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
            return rend.bounds.center + Vector3.up * (rend.bounds.extents.y + 0.1f);
        return transform.position + Vector3.up * 1f;
    }

    /// <summary>
    /// Returns the rotation for placing the medicine.
    /// </summary>
    public Quaternion GetPlacementRotation()
    {
        return Quaternion.Euler(0, transform.eulerAngles.y, 0);
    }

    /// <summary>
    /// Registers a placed medicine (for win condition or tracking).
    /// </summary>
    public void RegisterPlacedMedicine(GameObject medicine)
    {
        if (!placedMedicines.Contains(medicine))
        {
            placedMedicines.Add(medicine);
        }
    }

    /// <summary>
    /// Checks if all medicines are placed (stub for compatibility).
    /// </summary>
    public void CheckAllMedicinesPlaced()
    {
        // Implement win condition logic if needed
    }
}