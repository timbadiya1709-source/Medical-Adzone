using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRayInteract : MonoBehaviour
{
	public float interactDistance = 5f;
	public Color highlightColor = Color.yellow;

	private Transform cam;
	private GameObject lastHighlighted;
	private Material lastMaterial;
	private Color originalColor;
	private SelectableObject currentSelectable;

	void Start()
	{
		if (Camera.main != null)
		{
			cam = Camera.main.transform;
		}
	}

	void Update()
	{
		if (cam == null)
			return;

		Ray ray = new Ray(cam.position, cam.forward);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, interactDistance))
		{
			GameObject hitObj = hit.collider.gameObject;
			if (hitObj.CompareTag("Selectable"))
			{
				if (lastHighlighted != hitObj)
				{
					RemoveHighlight();
					HighlightObject(hitObj);
					
					// Show description
					SelectableObject selectable = hitObj.GetComponent<SelectableObject>();
					if (selectable != null)
					{
						currentSelectable = selectable;
						DescriptionUI.Instance?.ShowDescription(
							selectable.GetObjectName(), 
							selectable.GetDescription()
						);
					}
				}
				return;
			}
		}
		RemoveHighlight();
	}

	void HighlightObject(GameObject obj)
	{
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
		
		// Hide description
		if (currentSelectable != null)
		{
			DescriptionUI.Instance?.HideDescription();
			currentSelectable = null;
		}
	}
}