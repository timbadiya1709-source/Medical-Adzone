using UnityEngine;
using UnityEngine.UI;
using TMPro; // If using TextMeshPro (recommended)

/// <summary>
/// Manages the description pop-up UI
/// </summary>
public class DescriptionUI : MonoBehaviour
{
    public static DescriptionUI Instance { get; private set; }

    [Header("UI References")]
    public GameObject descriptionPanel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    
    // If using regular Unity UI Text instead of TextMeshPro:
    // public Text titleText;
    // public Text descriptionText;

    [Header("Animation Settings")]
    public float fadeSpeed = 5f;
    private CanvasGroup canvasGroup;

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
        canvasGroup = descriptionPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = descriptionPanel.AddComponent<CanvasGroup>();
        }
        
        HideDescription();
    }

    public void ShowDescription(string title, string description)
    {
        titleText.text = title;
        descriptionText.text = description;
        descriptionPanel.SetActive(true);
        
        // Fade in
        StopAllCoroutines();
        StartCoroutine(FadeIn());
    }

    public void HideDescription()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOut());
    }

    System.Collections.IEnumerator FadeIn()
    {
        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    System.Collections.IEnumerator FadeOut()
    {
        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }
        canvasGroup.alpha = 0f;
        descriptionPanel.SetActive(false);
    }
}