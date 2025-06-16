using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodFountain : MonoBehaviour
{
    public float bloodAmount = 30f;
    private bool hasBeenUsed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasBeenUsed) return;

        Debug.Log("Something entered the fountain trigger: " + other.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the blood fountain!");

            // Try to find BloodManager in the Player or its children/parents
            BloodManager bloodManager = other.GetComponent<BloodManager>();
            if (bloodManager == null)
                bloodManager = other.GetComponentInChildren<BloodManager>();
            if (bloodManager == null)
                bloodManager = other.GetComponentInParent<BloodManager>();

            if (bloodManager != null)
            {
                bloodManager.AddBlood(bloodAmount);
                hasBeenUsed = true;
                Debug.Log("Blood added from fountain.");
                // Optional: Visual change or destroy fountain
                // Destroy(gameObject); or change color
            }
            else
            {
                Debug.LogWarning("BloodManager not found on Player!");
            }
        }
    }
}


