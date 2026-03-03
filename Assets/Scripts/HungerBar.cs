using System;
using UnityEngine;
using UnityEngine.UI; // for UI

public class HungerBar : MonoBehaviour
{
    public Slider hungerBarSlider; // ref to hunger bar
    public float maxHunger = 100f;
    private float currentHunger;
    public float hungerDepletionRate = .5f; // how fast hunger depletes

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHunger = maxHunger;
        hungerBarSlider.maxValue = maxHunger;
        hungerBarSlider.value = currentHunger;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHunger > 0)
        {
            DrainHungerBar(hungerDepletionRate*Time.deltaTime);

        }
        else
        {
            Debug.Log("Player died!");
        }
    }
    public void DrainHungerBar(float drainBar) // - to hunger bar
    {
        currentHunger -= drainBar;
        currentHunger = Mathf.Clamp(currentHunger, 0, maxHunger);//hunger cannot go below 0 or above 100
        UpdateHungerBar();
    }

    public void Eat(float eatFood) // ++ to hunger bar
    {
        currentHunger += eatFood;
        currentHunger = Mathf.Clamp(currentHunger, 0, maxHunger);
        UpdateHungerBar();
    }

    private void UpdateHungerBar()
    {
        hungerBarSlider.value = currentHunger;
    }
}
