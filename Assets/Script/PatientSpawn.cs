using System.Collections;
using UnityEngine;

public class PatientSpawn : MonoBehaviour
{    
    [SerializeField] GameObject patientPrefab;
    [SerializeField] Vector3 spawnPoint;
    void Start()
    {
        StartCoroutine(patientspwandelay());
    }
    IEnumerator patientspwandelay()
    {
        while (true)
        {
         
        Instantiate(patientPrefab,spawnPoint, Quaternion.identity);   
        yield return new WaitForSeconds(10f);
        }
    }
}
