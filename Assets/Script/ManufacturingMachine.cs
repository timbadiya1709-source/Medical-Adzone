using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;

public class ManufacturingMachine : MonoBehaviour
{
    [Header("Machine Settings")]
    public float productionTime = 10f; // Customizable production time in seconds
    public GameObject productPrefab; // The product to spawn
    public Transform productSpawnPoint; // Where the product appears
    
    [Header("UI")]
    public TextMeshProUGUI timerText;
    
    [Header("Required Materials")]
    public List<DraggableRawMaterial.MaterialType> requiredMaterials = new List<DraggableRawMaterial.MaterialType>()
    {
        DraggableRawMaterial.MaterialType.Type1,
        DraggableRawMaterial.MaterialType.Type2,
        DraggableRawMaterial.MaterialType.Type3,
        DraggableRawMaterial.MaterialType.Type4
    };
    
    private List<DraggableRawMaterial.MaterialType> collectedMaterials = new List<DraggableRawMaterial.MaterialType>();
    private bool isProducing = false;
    private double productionEndTime; // Using double for precision with real time
    private string machineId;
    
    private void Awake()
    {
        // Generate unique ID for this machine
        machineId = gameObject.name + "_" + transform.position.ToString();
        
        // Load saved state if exists
        LoadMachineState();
    }

    private void Start()
    {
        UpdateTimerDisplay();
    }

    private void Update()
    {
        if (isProducing)
        {
            double currentTime = GetCurrentRealTime();
            double remainingTime = productionEndTime - currentTime;
            
            if (remainingTime <= 0)
            {
                // Production complete
                CompleteProduction();
            }
            else
            {
                UpdateTimerDisplay(remainingTime);
            }
        }
    }

    public void AddMaterial(DraggableRawMaterial.MaterialType materialType)
    {
        if (isProducing)
        {
            Debug.Log("Machine is already producing!");
            return;
        }
        
        // Check if this material is required and not already added
        if (requiredMaterials.Contains(materialType) && !collectedMaterials.Contains(materialType))
        {
            collectedMaterials.Add(materialType);
            Debug.Log($"Material {materialType} added. Total: {collectedMaterials.Count}/{requiredMaterials.Count}");
            
            // UPDATE THE UI DISPLAY
            UpdateTimerDisplay();
            
            // Check if all materials are collected
            if (AreAllMaterialsCollected())
            {
                StartProduction();
            }
        }
        else
        {
            Debug.Log($"Material {materialType} is not needed or already added!");
        }
    }

    private bool AreAllMaterialsCollected()
    {
        foreach (var requiredMaterial in requiredMaterials)
        {
            if (!collectedMaterials.Contains(requiredMaterial))
            {
                return false;
            }
        }
        return true;
    }

    private void StartProduction()
    {
        isProducing = true;
        productionEndTime = GetCurrentRealTime() + productionTime;
        
        Debug.Log("Production started!");
        SaveMachineState();
    }

    private void CompleteProduction()
    {
        isProducing = false;
        
        // Spawn the product
        if (productPrefab != null && productSpawnPoint != null)
        {
            // Instantiate the product at the spawn point
            GameObject spawnedProduct = Instantiate(productPrefab, productSpawnPoint.position, productSpawnPoint.rotation);
            spawnedProduct.SetActive(true);
            Debug.Log("Product is ready and spawned!");
        }
        else
        {
            Debug.LogWarning("Product Prefab or Spawn Point is not assigned!");
        }
        
        UpdateTimerDisplay();
        
        // Reset machine
        collectedMaterials.Clear();
        ClearMachineState();
    }

    private void UpdateTimerDisplay(double remainingTime = 0)
    {
        if (timerText != null)
        {
            if (isProducing)
            {
                TimeSpan timeSpan = TimeSpan.FromSeconds(remainingTime);
                timerText.text = $"Time: {timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
            }
            else if (AreAllMaterialsCollected())
            {
                timerText.text = "Product Ready!";
            }
            else
            {
                timerText.text = $"Materials: {collectedMaterials.Count}/{requiredMaterials.Count}";
            }
        }
    }

    // Get real-world time that persists across scenes
    private double GetCurrentRealTime()
    {
        return (DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds;
    }

    // Save machine state to PlayerPrefs
    private void SaveMachineState()
    {
        PlayerPrefs.SetInt(machineId + "_isProducing", isProducing ? 1 : 0);
        PlayerPrefs.SetString(machineId + "_endTime", productionEndTime.ToString());
        
        // Save collected materials
        string materialsString = string.Join(",", collectedMaterials);
        PlayerPrefs.SetString(machineId + "_materials", materialsString);
        
        PlayerPrefs.Save();
    }

    // Load machine state from PlayerPrefs
    private void LoadMachineState()
    {
        if (PlayerPrefs.HasKey(machineId + "_isProducing"))
        {
            isProducing = PlayerPrefs.GetInt(machineId + "_isProducing") == 1;
            
            if (isProducing)
            {
                string endTimeString = PlayerPrefs.GetString(machineId + "_endTime");
                if (double.TryParse(endTimeString, out double savedEndTime))
                {
                    productionEndTime = savedEndTime;
                    
                    // Check if production should already be complete
                    if (GetCurrentRealTime() >= productionEndTime)
                    {
                        CompleteProduction();
                    }
                }
            }
            
            // Load collected materials
            string materialsString = PlayerPrefs.GetString(machineId + "_materials");
            if (!string.IsNullOrEmpty(materialsString))
            {
                string[] materialStrings = materialsString.Split(',');
                collectedMaterials.Clear();
                foreach (string matString in materialStrings)
                {
                    if (Enum.TryParse(matString, out DraggableRawMaterial.MaterialType matType))
                    {
                        collectedMaterials.Add(matType);
                    }
                }
            }
        }
    }

    // Clear saved state
    private void ClearMachineState()
    {
        PlayerPrefs.DeleteKey(machineId + "_isProducing");
        PlayerPrefs.DeleteKey(machineId + "_endTime");
        PlayerPrefs.DeleteKey(machineId + "_materials");
        PlayerPrefs.Save();
    }

    private void OnDestroy()
    {
        // Save state when object is destroyed (scene change)
        if (isProducing)
        {
            SaveMachineState();
        }
    }
}