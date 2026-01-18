using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Updated PlayerRayInteract - Pick up medicine, carry it, place on table
/// </summary>
public class PlayerRayInteract : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactDistance = 5f;
    public Color highlightColor = Color.yellow;

    [Header("Input Settings")]
    [SerializeField] private InputAction interactAction; // E key

	private Transform cam;
	private GameObject lastHighlighted;
	private Material lastMaterial;
	private Color originalColor;
	private SelectableObject currentSelectable;
	private GameObject carriedMedicine;
	private Vector3 originalScale;
	private bool isCarryingMedicine = false;
	private CarryingPosition carryingPosition;

	void Start()
	{
		if (Camera.main != null)
		{
			cam = Camera.main.transform;
		}
		carryingPosition = GetComponent<CarryingPosition>();
	}

	void OnEnable()
	{
		interactAction.Enable();
	}

	void OnDisable()
	{
		interactAction.Disable();
	}

	void Update()
	{
		if (cam == null)
			return;

		// Update carried medicine position using CarryingPosition
		if (isCarryingMedicine && carriedMedicine != null && carryingPosition != null)
		{
			carryingPosition.AnimateCarriedObject(carriedMedicine.transform);
		}

		// Raycast for interactions
		Ray ray = new Ray(cam.position, cam.forward);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, interactDistance))
		{
			GameObject hitObj = hit.collider.gameObject;

			// Check if looking at medicine (and not carrying one)
			if (hitObj.CompareTag("Selectable") && !isCarryingMedicine)
			{
				if (lastHighlighted != hitObj)
				{
					RemoveHighlight();
					HighlightObject(hitObj);

					// Show description and prompt
					SelectableObject selectable = hitObj.GetComponent<SelectableObject>();
					if (selectable != null)
					{
						currentSelectable = selectable;
						DescriptionUI.Instance?.ShowDescription(
							selectable.GetObjectName(),
							selectable.GetDescription()
						);

						PickupPromptUI.Instance?.ShowPrompt("Press E to pick up");
					}
				}

				// Press E to pick up
				if (interactAction.WasPressedThisFrame() && currentSelectable != null)
				{
					PickupMedicine(currentSelectable);
				}

				return;
			}
			// Check if looking at table (and carrying medicine)
			else if (hitObj.CompareTag("Table") && isCarryingMedicine)
			{
				if (lastHighlighted != hitObj)
				{
					RemoveHighlight();
					HighlightObject(hitObj);

					// Show place prompt
					PickupPromptUI.Instance?.ShowPrompt($"Press E to place {carriedMedicine.name}");
				}

				// Press E to place
				if (interactAction.WasPressedThisFrame())
				{
					PlaceMedicineOnTable(hitObj.GetComponent<TablePlacement>(), hit.point);
				}

				return;
			}
		}

		RemoveHighlight();
	}

	void HighlightObject(GameObject obj)
	{
		if (!obj.CompareTag("Selectable"))
			return;
		Renderer rend = obj.GetComponent<Renderer>();
		if (rend != null)
		{
			lastHighlighted = obj;
			lastMaterial = rend.material;
			originalColor = lastMaterial.color;
			lastMaterial.color = highlightColor;
		}
	}

	void RemoveHighlight()
	{
		if (lastHighlighted != null && lastMaterial != null)
		{
			lastMaterial.color = originalColor;
			lastHighlighted = null;
			lastMaterial = null;
		}
		
		// Hide UI
		if (currentSelectable != null && !isCarryingMedicine)
		{
			DescriptionUI.Instance?.HideDescription();
			PickupPromptUI.Instance?.HidePrompt();
			currentSelectable = null;
		}
		
		if (isCarryingMedicine)
		{
			PickupPromptUI.Instance?.HidePrompt();
		}
	}

	void PickupMedicine(SelectableObject medicine)
	{
		// Store original scale
		originalScale = medicine.transform.localScale;

		// Disable physics
		Rigidbody rb = medicine.GetComponent<Rigidbody>();
		if (rb != null)
		{
			rb.isKinematic = true;
		}

		Collider col = medicine.GetComponent<Collider>();
		if (col != null)
		{
			col.enabled = false;
		}

		// Use CarryingPosition to attach to hand
		if (carryingPosition != null)
		{
			carryingPosition.AttachToHand(medicine.transform, originalScale);
		}

		// Update state
		carriedMedicine = medicine.gameObject;
		isCarryingMedicine = true;

		// Hide UI
		DescriptionUI.Instance?.HideDescription();
		PickupPromptUI.Instance?.HidePrompt();
		currentSelectable = null;

		// Show feedback
		PickupPromptUI.Instance?.ShowPickupFeedback($"Picked up {medicine.GetObjectName()}");

		Debug.Log($"Picked up {medicine.GetObjectName()}");
	}

	// Carrying animation is now handled by CarryingPosition

	void PlaceMedicineOnTable(TablePlacement table, Vector3 hitPoint)
	{
		if (carriedMedicine == null) return;

		// Get placement position from table
		Vector3 placePosition;
		Quaternion placeRotation;
		
		if (table != null)
		{
			// Use table's placement system
			placePosition = table.GetNextPlacementPosition();
			placeRotation = table.GetPlacementRotation();
		}
		else
		{
			// Place at hit point with offset
			placePosition = hitPoint + Vector3.up * 0.1f;
			placeRotation = Quaternion.identity;
		}

		// Unparent from hand
		carriedMedicine.transform.SetParent(null);
		
		// Set position and rotation
		carriedMedicine.transform.position = placePosition;
		carriedMedicine.transform.rotation = placeRotation;
		
		// Restore original scale
		carriedMedicine.transform.localScale = originalScale;

		// Re-enable physics (optional - can keep static on table)
		Rigidbody rb = carriedMedicine.GetComponent<Rigidbody>();
		if (rb != null)
		{
			rb.isKinematic = true; // Keep kinematic so it doesn't fall
		}
		
		Collider col = carriedMedicine.GetComponent<Collider>();
		if (col != null)
		{
			col.enabled = true;
		}

		// Mark as placed
		SelectableObject selectable = carriedMedicine.GetComponent<SelectableObject>();
		if (selectable != null)
		{
			selectable.isPlacedOnTable = true;
		}

		// Register with table
		if (table != null)
		{
			table.RegisterPlacedMedicine(carriedMedicine);
		}

		// Play sound removed

		// Show feedback
		PickupPromptUI.Instance?.ShowPickupFeedback($"Placed {carriedMedicine.name} on table");

		Debug.Log($"Placed {carriedMedicine.name} on table");

		// Clear carried state
		string medicineName = carriedMedicine.name;
		carriedMedicine = null;
		isCarryingMedicine = false;

		// Hide prompt
		PickupPromptUI.Instance?.HidePrompt();

		// Check if all medicines placed
		if (table != null)
		{
			table.CheckAllMedicinesPlaced();
		}
	}

	// No unused code remains; all UI feedback and interaction logic is preserved as requested.
}