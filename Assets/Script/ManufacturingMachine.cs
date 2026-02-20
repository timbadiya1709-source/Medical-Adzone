using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;

public class ManufacturingMachine : MonoBehaviour
{
    [Header("Machine Settings")]
    public float productionTime = 10f;
    public GameObject productPrefab;
    public Transform productSpawnPoint;

    [Header("UI")]
    public TextMeshProUGUI timerText;

    [Header("UI System")]
    public MachineUIPanel machineUIPanel;
    
    private bool isProducing = false;
    private bool isProductWaiting = false;
    private double productionEndTime;
    private string machineId;
    private string currentProducedMedicineID;

    // Bump this whenever save format changes to wipe stale PlayerPrefs data
    private const int SaveVersion = 3;

    // Tracks whether we spawned a product during LoadMachineState (in Awake)
    private GameObject pendingProduct = null;

    private void Awake()
    {
        // Use a more stable ID that includes local position and name
        machineId = $"{gameObject.name}_{transform.localPosition.x:F2}_{transform.localPosition.y:F2}_{transform.localPosition.z:F2}";

        int savedVersion = PlayerPrefs.GetInt(machineId + "_saveVersion", -1);
        if (savedVersion != SaveVersion)
        {
            ClearMachineState();
        }

        LoadMachineState();
    }

    private void Start()
    {
        // If LoadMachineState spawned a waiting product, register it now
        if (pendingProduct != null)
        {
            ProductCollector collector = pendingProduct.GetComponent<ProductCollector>();
            if (collector != null)
            {
                collector.medicineID = currentProducedMedicineID;
                if (ProductManager.Instance != null)
                    ProductManager.Instance.RegisterProduct(collector);
            }
            pendingProduct = null;
        }

        UpdateTimerDisplay();
    }

    private void OnMouseDown()
    {
        if (isProducing || isProductWaiting) return;
        
        if (machineUIPanel != null)
        {
            machineUIPanel.Show(this);
        }
    }

    public void StartProductionFromUI(string apiID)
    {
        currentProducedMedicineID = apiID;
        isProducing = true;
        productionEndTime = GetCurrentRealTime() + productionTime;
        
        Debug.Log($"[Machine:{gameObject.name}] Started producing: {currentProducedMedicineID}");
        SaveMachineState();
    }

    private void Update()
    {
        if (isProducing)
        {
            double remaining = productionEndTime - GetCurrentRealTime();
            if (remaining <= 0)
                CompleteProduction();
            else
                UpdateTimerDisplay(remaining);
        }
    }

    public bool IsMachineBusy()
    {
        return isProducing || isProductWaiting;
    }

    private void CompleteProduction()
    {
        isProducing = false;

        if (!isProductWaiting)
        {
            isProductWaiting = true;

            if (productPrefab != null && productSpawnPoint != null)
            {
                GameObject spawned = Instantiate(
                    productPrefab,
                    productSpawnPoint.position,
                    productSpawnPoint.rotation
                );
                spawned.SetActive(true);

                ProductCollector collector = spawned.GetComponent<ProductCollector>();
                if (collector != null)
                {
                    collector.parentMachine = this;
                    collector.medicineID = currentProducedMedicineID;
                    if (ProductManager.Instance != null)
                        ProductManager.Instance.RegisterProduct(collector);
                }

                Debug.Log($"{gameObject.name}: {currentProducedMedicineID} produced.");
            }
            
            SaveMachineState();
        }

        UpdateTimerDisplay();
    }

    public void OnProductCollected()
    {
        isProductWaiting = false;
        currentProducedMedicineID = "";
        ClearMachineState();
        UpdateTimerDisplay();
    }

    private void UpdateTimerDisplay(double remainingTime = 0)
    {
        if (timerText == null) return;

        if (isProducing)
        {
            TimeSpan t = TimeSpan.FromSeconds(remainingTime);
            timerText.text = $"{currentProducedMedicineID}: {t.Minutes:D2}:{t.Seconds:D2}";
        }
        else if (isProductWaiting)
        {
            timerText.text = "Ready!";
        }
        else
        {
            timerText.text = "click here";
        }
    }

    private double GetCurrentRealTime()
    {
        return (DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds;
    }

    private void SaveMachineState()
    {
        PlayerPrefs.SetInt(machineId + "_saveVersion",      SaveVersion);
        PlayerPrefs.SetInt(machineId + "_isProducing",      isProducing      ? 1 : 0);
        PlayerPrefs.SetInt(machineId + "_isProductWaiting", isProductWaiting ? 1 : 0);
        PlayerPrefs.SetString(machineId + "_endTime",       productionEndTime.ToString());
        PlayerPrefs.SetString(machineId + "_productID",     currentProducedMedicineID);
        PlayerPrefs.Save();
    }

    private void LoadMachineState()
    {
        if (!PlayerPrefs.HasKey(machineId + "_saveVersion"))
        {
            isProducing = false;
            isProductWaiting = false;
            return;
        }

        if (PlayerPrefs.GetInt(machineId + "_saveVersion") != SaveVersion) return;
        
        currentProducedMedicineID = PlayerPrefs.GetString(machineId + "_productID", "");
        isProducing      = PlayerPrefs.GetInt(machineId + "_isProducing", 0)      == 1;
        isProductWaiting = PlayerPrefs.GetInt(machineId + "_isProductWaiting", 0) == 1;

        if (isProducing)
        {
            if (double.TryParse(PlayerPrefs.GetString(machineId + "_endTime"), out double savedEnd))
            {
                productionEndTime = savedEnd;
                if (GetCurrentRealTime() >= productionEndTime)
                    CompleteProduction();
            }
            else
            {
                isProducing = false;
                ClearMachineState();
            }
        }
        else if (isProductWaiting)
        {
            if (productPrefab != null && productSpawnPoint != null)
            {
                GameObject spawned = Instantiate(
                    productPrefab,
                    productSpawnPoint.position,
                    productSpawnPoint.rotation
                );
                spawned.SetActive(true);

                ProductCollector collector = spawned.GetComponent<ProductCollector>();
                if (collector != null)
                {
                    collector.parentMachine = this;
                    collector.medicineID = currentProducedMedicineID;
                }

                pendingProduct = spawned;
            }
        }
    }

    private void ClearMachineState()
    {
        PlayerPrefs.DeleteKey(machineId + "_saveVersion");
        PlayerPrefs.DeleteKey(machineId + "_isProducing");
        PlayerPrefs.DeleteKey(machineId + "_isProductWaiting");
        PlayerPrefs.DeleteKey(machineId + "_endTime");
        PlayerPrefs.DeleteKey(machineId + "_productID");
        PlayerPrefs.Save();
    }
}
