using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]private Slider healthSlider;
    private Health playerHealth;
    private float currentHealth;
    void Start()
    {
        playerHealth = GetComponent<Health>();
        currentHealth = playerHealth.GetCurrentHealth();
    }
    private void UpdateHealthSlider()
    {
        healthSlider.value = playerHealth.GetCurrentHealth();
    }
    void Update()
    {
        UpdateHealthSlider();
    }

}
