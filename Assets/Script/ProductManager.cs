// ProductManager.cs
// Attach this to an empty GameObject in your scene (make it DontDestroyOnLoad)

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ProductManager : MonoBehaviour
{
    public static ProductManager Instance;

    [Header("UI References")]
    public Button collectButton;
    public Text productCountText; // For displaying count in other scene

    private List<ProductCollector> activeProducts = new List<ProductCollector>();
    private int productCount = 0;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadProductCount();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // Setup collect button
        if (collectButton != null)
        {
            collectButton.onClick.AddListener(OnCollectButtonClicked);
            collectButton.gameObject.SetActive(false); // Hide initially
        }

        UpdateProductCountUI();
    }

    public void RegisterProduct(ProductCollector product)
    {
        if (!activeProducts.Contains(product))
        {
            activeProducts.Add(product);
            UpdateCollectButtonVisibility();
        }
    }

    public void UnregisterProduct(ProductCollector product)
    {
        activeProducts.Remove(product);
        UpdateCollectButtonVisibility();
    }

    private void OnCollectButtonClicked()
    {
        // Collect the first product in the list
        if (activeProducts.Count > 0)
        {
            ProductCollector product = activeProducts[0];
            if (product != null)
            {
                product.CollectProduct();
            }
        }
    }

    public void ProductCollected()
    {
        productCount++;
        SaveProductCount();
        UpdateProductCountUI();
    }

    private void UpdateCollectButtonVisibility()
    {
        if (collectButton != null)
        {
            collectButton.gameObject.SetActive(activeProducts.Count > 0);
        }
    }

    private void UpdateProductCountUI()
    {
        if (productCountText != null)
        {
            productCountText.text = "Products: " + productCount.ToString();
        }
    }

    private void SaveProductCount()
    {
        PlayerPrefs.SetInt("ProductCount", productCount);
        PlayerPrefs.Save();
    }

    private void LoadProductCount()
    {
        productCount = PlayerPrefs.GetInt("ProductCount", 0);
    }

    public int GetProductCount()
    {
        return productCount;
    }

    // Optional: Reset count (useful for testing)
    public void ResetProductCount()
    {
        productCount = 0;
        SaveProductCount();
        UpdateProductCountUI();
    }
}