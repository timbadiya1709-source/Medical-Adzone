using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Attach to your table GameObject - Manages medicine placement
/// </summary>
public class TablePlacement : MonoBehaviour
{
	[Header("Placement Settings")]
	[Tooltip("Placement spots on the table")]
	public Transform[] placementSpots; // Assign in Inspector
	
	[Header("Auto-Generate Placement Spots")]
	public bool autoGenerateSpots = true;
	public int numberOfSpots = 3;
	public float spotSpacing = 0.5f;
	public Vector3 spotOffset = new Vector3(0, 0.1f, 0); // Height above table

	[Header("Win Condition")]
	public int totalMedicinesToPlace = 3;
	
	private List<GameObject> placedMedicines = new List<GameObject>();
	private int currentSpotIndex = 0;

	void Start()
	{
		// Auto-generate placement spots if enabled
		if (autoGenerateSpots && (placementSpots == null || placementSpots.Length == 0))
		{
			GeneratePlacementSpots();
		}

		// Validate spots
		if (placementSpots == null || placementSpots.Length == 0)
		{
			Debug.LogWarning($"TablePlacement on {gameObject.name}: No placement spots assigned!");
		}
	}

	void GeneratePlacementSpots()
	{
		// Get table bounds
		Renderer tableRenderer = GetComponent<Renderer>();
		if (tableRenderer == null)
		{
			Debug.LogError("Table needs a Renderer to auto-generate spots!");
			return;
		}

		Bounds bounds = tableRenderer.bounds;
		Vector3 center = bounds.center;
		
		// Create parent for spots
		Transform spotsParent = transform.Find("PlacementSpots");
		if (spotsParent == null)
		{
			GameObject spotsObj = new GameObject("PlacementSpots");
			spotsObj.transform.SetParent(transform);
			spotsParent = spotsObj.transform;
		}

		// Clear existing spots
		foreach (Transform child in spotsParent)
		{
			DestroyImmediate(child.gameObject);
		}

		// Generate spots in a line
		placementSpots = new Transform[numberOfSpots];
		float totalWidth = (numberOfSpots - 1) * spotSpacing;
		float startX = center.x - (totalWidth / 2f);

		for (int i = 0; i < numberOfSpots; i++)
		{
			GameObject spot = new GameObject($"Spot_{i + 1}");
			spot.transform.SetParent(spotsParent);
			
			Vector3 position = new Vector3(
				startX + (i * spotSpacing),
				bounds.max.y + spotOffset.y,
				center.z + spotOffset.z
			);
			
			spot.transform.position = position;
			placementSpots[i] = spot.transform;

			// Optional: Add visual marker (gizmo only)
			spot.AddComponent<PlacementSpotMarker>();
		}

		Debug.Log($"Generated {numberOfSpots} placement spots on {gameObject.name}");
	}

	public Vector3 GetNextPlacementPosition()
	{
		if (placementSpots == null || placementSpots.Length == 0)
		{
			// Fallback: place on top of table
			Renderer rend = GetComponent<Renderer>();
			if (rend != null)
			{
				return rend.bounds.center + Vector3.up * (rend.bounds.extents.y + 0.1f);
			}
			return transform.position + Vector3.up * 1f;
		}

		// Get next available spot
		if (currentSpotIndex < placementSpots.Length)
		{
			Vector3 pos = placementSpots[currentSpotIndex].position;
			currentSpotIndex++;
			return pos;
		}

		// All spots filled, place at last spot
		return placementSpots[placementSpots.Length - 1].position;
	}

	public Quaternion GetPlacementRotation()
	{
		// Face forward or match table rotation
		return Quaternion.Euler(0, transform.eulerAngles.y, 0);
	}

	public void RegisterPlacedMedicine(GameObject medicine)
	{
		if (!placedMedicines.Contains(medicine))
		{
			placedMedicines.Add(medicine);
			Debug.Log($"Medicine placed on table: {medicine.name}. Total: {placedMedicines.Count}/{totalMedicinesToPlace}");
		}
	}

	public void CheckAllMedicinesPlaced()
	{
		if (placedMedicines.Count >= totalMedicinesToPlace)
		{
			OnAllMedicinesPlaced();
		}
	}

	void OnAllMedicinesPlaced()
	{
		Debug.Log("🎉 All medicines placed on table! Quest complete!");
		
		// Add your win condition logic here:
		// - Show victory screen
		// - Unlock door
		// - Trigger next objective
		// - Award points
		
		// Example: Show feedback
		PickupPromptUI.Instance?.ShowPickupFeedback("All medicines placed! Well done!");
		
		// Example: Call game manager
		// GameManager.Instance?.OnQuestComplete();
	}

	// Reset for testing
	public void ResetTable()
	{
		placedMedicines.Clear();
		currentSpotIndex = 0;
	}

	// Get placement status
	public int GetPlacedCount()
	{
		return placedMedicines.Count;
	}

	public bool IsComplete()
	{
		return placedMedicines.Count >= totalMedicinesToPlace;
	}

	// Gizmos for visualization
	void OnDrawGizmos()
	{
		if (placementSpots == null || placementSpots.Length == 0)
			return;

		Gizmos.color = Color.green;
		foreach (Transform spot in placementSpots)
		{
			if (spot != null)
			{
				Gizmos.DrawWireSphere(spot.position, 0.1f);
				Gizmos.DrawLine(spot.position, spot.position + Vector3.up * 0.2f);
			}
		}
	}

	void OnDrawGizmosSelected()
	{
		if (placementSpots == null || placementSpots.Length == 0)
			return;

		// Draw placement spots with numbers
		for (int i = 0; i < placementSpots.Length; i++)
		{
			if (placementSpots[i] != null)
			{
				Gizmos.color = i < currentSpotIndex ? Color.red : Color.green;
				Gizmos.DrawSphere(placementSpots[i].position, 0.15f);
			}
		}
	}
}

/// <summary>
/// Helper class for visualizing placement spots
/// </summary>
public class PlacementSpotMarker : MonoBehaviour
{
	void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(transform.position, 0.1f);
	}
}