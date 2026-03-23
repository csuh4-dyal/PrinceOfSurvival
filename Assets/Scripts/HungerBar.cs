using UnityEngine;
using UnityEngine.UI;
using System;

public class HungerBar : MonoBehaviour
{
    public static event Action OnPlayerStarved;

    [Header("Hunger Settings")]
    public float maxHunger = 100f;
    public float currentHunger;
    public float hungerDrainRate = 2f; // per second

    [Header("UI")]
    public Image hungerFillImage; // assign UI Image (Fill type)

    void Start()
    {
        currentHunger = maxHunger;
        UpdateUI();
    }

    void Update()
    {
        DrainHunger();
    }

    void DrainHunger()
    {
        if (currentHunger <= 0) return;

        currentHunger -= hungerDrainRate * Time.deltaTime;
        currentHunger = Mathf.Clamp(currentHunger, 0, maxHunger);

        UpdateUI();

        if (currentHunger <= 0)
        {
            Debug.Log("Player starved!");
            OnPlayerStarved?.Invoke();
        }
    }

    public void AddHunger(float amount)
    {
        currentHunger += amount;
        currentHunger = Mathf.Clamp(currentHunger, 0, maxHunger);

        UpdateUI();
    }

    void UpdateUI()
    {
        if (hungerFillImage != null)
        {
            hungerFillImage.fillAmount = currentHunger / maxHunger;
        }
    }
}
