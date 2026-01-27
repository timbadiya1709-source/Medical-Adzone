using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HierarchicalPanelButton : MonoBehaviour
{
    [Header("Default Panel")]
    public GameObject defaultPanel; // The panel that opens by default with this button
    
    [Header("Child Buttons")]
    public GameObject[] childButtons; // Additional buttons that appear when this is clicked
    
    [Header("Child Panels")]
    public GameObject[] childPanels; // Panels controlled by child buttons (should match childButtons array)
    
    [Header("Button Reference")]
    public Button toggleButton; // This button itself

    private bool isOpen = false;

    void Start()
    {
        // Hide default panel at start
        if (defaultPanel != null)
            defaultPanel.SetActive(false);

        // Hide all child buttons at start
        if (childButtons != null)
        {
            foreach (GameObject childBtn in childButtons)
            {
                if (childBtn != null)
                    childBtn.SetActive(false);
            }
        }

        // Hide all child panels at start
        if (childPanels != null)
        {
            foreach (GameObject childPanel in childPanels)
            {
                if (childPanel != null)
                    childPanel.SetActive(false);
            }
        }

        // Add button click listener
        if (toggleButton != null)
            toggleButton.onClick.AddListener(TogglePanel);

        // Setup child button listeners
        SetupChildButtons();
    }

    void SetupChildButtons()
    {
        if (childButtons == null || childPanels == null) return;

        for (int i = 0; i < childButtons.Length && i < childPanels.Length; i++)
        {
            if (childButtons[i] != null && childPanels[i] != null)
            {
                Button btn = childButtons[i].GetComponent<Button>();
                int index = i; // Capture index for closure
                
                if (btn != null)
                {
                    btn.onClick.AddListener(() => OpenChildPanel(index));
                }
            }
        }
    }

    void OpenChildPanel(int index)
    {
        // Close all child panels first
        if (childPanels != null)
        {
            foreach (GameObject panel in childPanels)
            {
                if (panel != null)
                    panel.SetActive(false);
            }
        }

        // Close default panel
        if (defaultPanel != null)
            defaultPanel.SetActive(false);

        // Open the selected child panel
        if (index >= 0 && index < childPanels.Length && childPanels[index] != null)
        {
            childPanels[index].SetActive(true);
        }
    }

    void Update()
    {
        // Check if any UI element is active
        if (isOpen)
        {
            // Detect mouse click
            if (Input.GetMouseButtonDown(0))
            {
                // Check if click is outside all panels and buttons
                if (!IsPointerOverAnyUI())
                {
                    CloseAll();
                }
            }
        }
    }

    void TogglePanel()
    {
        isOpen = !isOpen;

        // Toggle child buttons
        if (childButtons != null)
        {
            foreach (GameObject childBtn in childButtons)
            {
                if (childBtn != null)
                    childBtn.SetActive(isOpen);
            }
        }

        if (isOpen)
        {
            // Open default panel when utility button is clicked
            if (defaultPanel != null)
            {
                defaultPanel.SetActive(true);
                Debug.Log("Opening default panel: " + defaultPanel.name);
            }

            // Make sure all other child panels are closed
            if (childPanels != null)
            {
                foreach (GameObject childPanel in childPanels)
                {
                    if (childPanel != null && childPanel != defaultPanel)
                        childPanel.SetActive(false);
                }
            }
        }
        else
        {
            // Close default panel
            if (defaultPanel != null)
                defaultPanel.SetActive(false);

            // Close all child panels
            if (childPanels != null)
            {
                foreach (GameObject childPanel in childPanels)
                {
                    if (childPanel != null)
                        childPanel.SetActive(false);
                }
            }
        }
    }

    void CloseAll()
    {
        isOpen = false;

        // Close default panel
        if (defaultPanel != null)
            defaultPanel.SetActive(false);

        // Close child buttons
        if (childButtons != null)
        {
            foreach (GameObject childBtn in childButtons)
            {
                if (childBtn != null)
                    childBtn.SetActive(false);
            }
        }

        // Close child panels
        if (childPanels != null)
        {
            foreach (GameObject childPanel in childPanels)
            {
                if (childPanel != null)
                    childPanel.SetActive(false);
            }
        }
    }

    bool IsPointerOverAnyUI()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        var raycastResults = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        foreach (RaycastResult result in raycastResults)
        {
            // Check if clicking on default panel
            if (defaultPanel != null && (result.gameObject == defaultPanel || 
                result.gameObject.transform.IsChildOf(defaultPanel.transform)))
            {
                return true;
            }

            // Check if clicking on any child button
            if (childButtons != null)
            {
                foreach (GameObject childBtn in childButtons)
                {
                    if (childBtn != null && (result.gameObject == childBtn || 
                        result.gameObject.transform.IsChildOf(childBtn.transform)))
                    {
                        return true;
                    }
                }
            }

            // Check if clicking on any child panel
            if (childPanels != null)
            {
                foreach (GameObject childPanel in childPanels)
                {
                    if (childPanel != null && (result.gameObject == childPanel || 
                        result.gameObject.transform.IsChildOf(childPanel.transform)))
                    {
                        return true;
                    }
                }
            }

            // Check if clicking on the toggle button itself
            if (toggleButton != null && (result.gameObject == toggleButton.gameObject || 
                result.gameObject.transform.IsChildOf(toggleButton.transform)))
            {
                return true;
            }
        }

        return false;
    }

    void OnDestroy()
    {
        if (toggleButton != null)
            toggleButton.onClick.RemoveListener(TogglePanel);
    }
}