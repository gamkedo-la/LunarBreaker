using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] float maxHealth = 10;
    [SerializeField] Slider healthSlider;

    float currentHealth = 10;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthSlider.value = currentHealth / maxHealth;
    }

    public void GetDamage(float damage)
    {
        currentHealth -= damage;
        healthSlider.value = currentHealth / maxHealth;
    }
}
