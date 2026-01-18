using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Updated PlayerRayInteract - Pick up medicine, carry it, place on table
/// AND pick up other objects (Torch, etc.) and drop them at original position
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
	
	// Carried object data
	private GameObject carriedObject;
	private Vector3 originalScale;
	private Vector3 originalPosition; // Store pickup position for non-medicine objects
	private Quaternion originalRotation; // Store pickup rotation
	private bool isCarryingObject = false;
	private bool isCarryingMedicine = false; // Track if carrying medicine specifically
	
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

		// Update carried object position using CarryingPosition
		if (isCarryingObject && carriedObject != null && carryingPosition != null)
		{
			carryingPosition.AnimateCarriedObject(carriedObject.transform);
		}

		// If carrying a non-medicine object, show drop prompt and wait for E
		if (isCarryingObject && !isCarryingMedicine)
		{
			PickupPromptUI.Instance?.ShowPrompt("Press E to drop");
			
			// Press E to drop at original position
			if (interactAction.WasPressedThisFrame())
			{
				DropObjectAtOriginalPosition();
			}
			return;
		}

		// Raycast for interactions
		Ray ray = new Ray(cam.position, cam.forward);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, interactDistance))
		{
			GameObject hitObj = hit.collider.gameObject;

			// Check if looking at medicine (and not carrying anything)
			if (hitObj.CompareTag("Selectable") && !isCarryingObject)
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

				// Press E to pick up medicine
				if (interactAction.WasPressedThisFrame() && currentSelectable != null)
				{
					PickupMedicine(currentSelectable);
				}

				return;
			}
			// Check if looking at pickupable object (Torch, etc.) and not carrying anything
			else if (hitObj.CompareTag("Pickupable") && !isCarryingObject)
			{
				// Don't highlight, but show description
				SelectableObject selectable = hitObj.GetComponent<SelectableObject>();
				if (selectable != null)
				{
					if (currentSelectable != selectable)
					{
						RemoveHighlight();
						currentSelectable = selectable;
						
						DescriptionUI.Instance?.ShowDescription(
							selectable.GetObjectName(),
							selectable.GetDescription()
						);

						PickupPromptUI.Instance?.ShowPrompt("Press E to pick up");
					}

					// Press E to pick up object
					if (interactAction.WasPressedThisFrame())
					{
						PickupObject(selectable);
					}
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
					PickupPromptUI.Instance?.ShowPrompt($"Press E to place {carriedObject.name}");
				}

				// Press E to place medicine
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
		// Only highlight "Selectable" (medicine) objects
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
		if (currentSelectable != null && !isCarryingObject)
		{
			DescriptionUI.Instance?.HideDescription();
			PickupPromptUI.Instance?.HidePrompt();
			currentSelectable = null;
		}
		
		if (isCarryingObject && !isCarryingMedicine)
		{
			// Don't hide prompt when carrying non-medicine (shows "Press E to drop")
			DescriptionUI.Instance?.HideDescription();
		}
		else if (isCarryingMedicine)
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
		carriedObject = medicine.gameObject;
		isCarryingObject = true;
		isCarryingMedicine = true;

		// Hide UI
		DescriptionUI.Instance?.HideDescription();
		PickupPromptUI.Instance?.HidePrompt();
		currentSelectable = null;

		// Show feedback
		PickupPromptUI.Instance?.ShowPickupFeedback($"Picked up {medicine.GetObjectName()}");

		Debug.Log($"Picked up {medicine.GetObjectName()}");
	}

	void PickupObject(SelectableObject obj)
	{
		// Store original transform data
		originalScale = obj.transform.localScale;
		originalPosition = obj.transform.position;
		originalRotation = obj.transform.rotation;

		// Disable physics
		Rigidbody rb = obj.GetComponent<Rigidbody>();
		if (rb != null)
		{
			rb.isKinematic = true;
		}

		Collider col = obj.GetComponent<Collider>();
		if (col != null)
		{
			col.enabled = false;
		}

		// Use CarryingPosition to attach to hand
		if (carryingPosition != null)
		{
			carryingPosition.AttachToHand(obj.transform, originalScale);
		}

		// Update state
		carriedObject = obj.gameObject;
		isCarryingObject = true;
		isCarryingMedicine = false; // Not medicine

		// Hide UI
		DescriptionUI.Instance?.HideDescription();
		PickupPromptUI.Instance?.HidePrompt();
		currentSelectable = null;

		// Show feedback
		PickupPromptUI.Instance?.ShowPickupFeedback($"Picked up {obj.GetObjectName()}");

		Debug.Log($"Picked up {obj.GetObjectName()} - will drop at original position");
	}

	void DropObjectAtOriginalPosition()
	{
		if (carriedObject == null) return;

		// Unparent from hand
		carriedObject.transform.SetParent(null);
		
		// Restore original transform
		carriedObject.transform.position = originalPosition;
		carriedObject.transform.rotation = originalRotation;
		carriedObject.transform.localScale = originalScale;

		// Re-enable physics
		Rigidbody rb = carriedObject.GetComponent<Rigidbody>();
		if (rb != null)
		{
			rb.isKinematic = true; // Keep kinematic
		}
		
		Collider col = carriedObject.GetComponent<Collider>();
		if (col != null)
		{
			col.enabled = true;
		}

		// Show feedback
		PickupPromptUI.Instance?.ShowPickupFeedback($"Dropped {carriedObject.name}");

		Debug.Log($"Dropped {carriedObject.name} at original position");

		// Clear carried state
		carriedObject = null;
		isCarryingObject = false;
		isCarryingMedicine = false;

		// Hide prompt
		PickupPromptUI.Instance?.HidePrompt();
	}

	void PlaceMedicineOnTable(TablePlacement table, Vector3 hitPoint)
	{
		if (carriedObject == null) return;

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
		carriedObject.transform.SetParent(null);
		
		// Set position and rotation
		carriedObject.transform.position = placePosition;
		carriedObject.transform.rotation = placeRotation;
		
		// Restore original scale
		carriedObject.transform.localScale = originalScale;

		// Re-enable physics (optional - can keep static on table)
		Rigidbody rb = carriedObject.GetComponent<Rigidbody>();
		if (rb != null)
		{
			rb.isKinematic = true; // Keep kinematic so it doesn't fall
		}
		
		Collider col = carriedObject.GetComponent<Collider>();
		if (col != null)
		{
			col.enabled = true;
		}

		// Mark as placed
		SelectableObject selectable = carriedObject.GetComponent<SelectableObject>();
		if (selectable != null)
		{
			selectable.isPlacedOnTable = true;
		}

		// Register with table
		if (table != null)
		{
			table.RegisterPlacedMedicine(carriedObject);
		}

		// Show feedback
		PickupPromptUI.Instance?.ShowPickupFeedback($"Placed {carriedObject.name} on table");

		Debug.Log($"Placed {carriedObject.name} on table");

		// Clear carried state
		string medicineName = carriedObject.name;
		carriedObject = null;
		isCarryingObject = false;
		isCarryingMedicine = false;

		// Hide prompt
		PickupPromptUI.Instance?.HidePrompt();

		// Check if all medicines placed
		if (table != null)
		{
			table.CheckAllMedicinesPlaced();
		}
	}
}