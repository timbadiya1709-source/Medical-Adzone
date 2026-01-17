using UnityEngine;
using TMPro;

/// <summary>
/// Simple version - Shows "Press E" prompts only (no feedback messages)
/// </summary>
public class PickupPromptUI : MonoBehaviour
{
	public static PickupPromptUI Instance { get; private set; }

	[Header("UI References")]
	public GameObject promptPanel;
	public TextMeshProUGUI promptText;

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	void Start()
	{
		if (promptPanel != null)
			promptPanel.SetActive(false);
	}

	public void ShowPrompt(string message)
	{
		if (promptPanel == null || promptText == null) return;

		promptText.text = message;
		promptPanel.SetActive(true);
	}

	public void HidePrompt()
	{
		if (promptPanel != null)
			promptPanel.SetActive(false);
	}

	// Empty method so other scripts don't break
	public void ShowPickupFeedback(string message)
	{
		// Does nothing - no feedback system
		// You can also just remove calls to this method from other scripts
	}
}