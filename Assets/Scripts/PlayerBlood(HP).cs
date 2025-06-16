using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BloodManager : MonoBehaviour
{
    [Header("Blood Settings")]
    public float maxBlood = 100f;
    public float currentBlood;
    public float bloodLossPerUnit = 0.1f;

    private bool isDead = false;
    private DeathScreen deathScreen;


    [Header("UI")]
    public Slider bloodSlider;

    private Vector3 lastPosition;

    void Start()
    {

        deathScreen = FindObjectOfType<DeathScreen>();

        currentBlood = maxBlood;
        lastPosition = transform.position;

        // Set up the slider
        if (bloodSlider != null)
        {
            bloodSlider.maxValue = maxBlood;
            bloodSlider.value = currentBlood;
        }
    }

    void Update()
    {
        // Calculate movement distance
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);
        if (distanceMoved > 0)
        {
            currentBlood -= distanceMoved * bloodLossPerUnit;
            currentBlood = Mathf.Clamp(currentBlood, 0, maxBlood);
        }

        lastPosition = transform.position;

        // Update UI
        if (bloodSlider != null)
        {
            bloodSlider.value = currentBlood;
        }

        if (currentBlood <= 0 && !isDead)
        {
            isDead = true;

            if (deathScreen != null)
            {
                deathScreen.TriggerDeath();
            }
            else
            {
                Debug.LogWarning("No DeathScreen found!");
            }
        }
    }

    public void AddBlood(float amount)
    {
        Debug.Log($"Adding {amount} blood. Current before: {currentBlood}");

        currentBlood += amount;
        currentBlood = Mathf.Clamp(currentBlood, 0, maxBlood);

        if (bloodSlider != null)
        {
            bloodSlider.value = currentBlood;
        }

        Debug.Log($"New blood amount: {currentBlood}");
    }

}

