using UnityEngine;
using System.Collections.Generic;

public class ManufacturingConstants
{
    public const string RAW_MATERIAL_TAG = "RawMaterial";
    public const string API_TAG = "API";

    public const string BINDER = "Binder";
    public const string LUBRICANT = "Lubricant";
    public const string EXCIPIENT = "Excipient";

    public static readonly List<string> AvailableAPIs = new List<string>
    {
        "Paracetamol",
        "Aspirin",
        "Amoxicillin",
        "Ibuprofen"
    };

    public static readonly List<string> RawMaterialTypes = new List<string>
    {
        BINDER,
        LUBRICANT,
        EXCIPIENT
    };

    // Mapping from API to final Medicine ID (used in counters)
    public static string GetMedicineID(string apiID)
    {
        return apiID; // For now they match, e.g. "Paracetamol" API produces "Paracetamol" medicine
    }
}