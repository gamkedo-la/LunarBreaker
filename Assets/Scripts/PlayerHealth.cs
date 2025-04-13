using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] float maxHealth = 10;
    [SerializeField] Slider healthSlider;
    [SerializeField] float timeBeforeReloadAfterDeath = 2.0f;
    [SerializeField] Image healthBar;
    [SerializeField] Color fullHealthColor = Color.green;
    [SerializeField] Color midHealthColor = Color.blue;
    [SerializeField] Color lowHealthColor = Color.red;
    [SerializeField] Animator gunAnimator;

    float currentHealth = 10;
    bool isDead = false;

    LevelManager levelManager;




    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthSlider.value = currentHealth / maxHealth;
        isDead = false;
        levelManager = FindObjectOfType<LevelManager>();
        healthBar.color = fullHealthColor;
    }

    public void GetDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        healthSlider.value = currentHealth / maxHealth;

        if (healthSlider.value <= 0.25)
        {
            healthBar.color = lowHealthColor;
        }
        else if (healthSlider.value <= 0.5)
        {
            healthBar.color = midHealthColor;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void GetHealth(float health)
    {
        currentHealth += health;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        healthSlider.value = currentHealth / maxHealth;

        if (healthSlider.value > 0.5)
        {
            healthBar.color = fullHealthColor;
        }
        else if (healthSlider.value > 0.25)
        {
            healthBar.color = midHealthColor;
        }
    }

    public void Die()
    {
        isDead = true;
        levelManager.ReloadCurrentSceneAfterTime(timeBeforeReloadAfterDeath);
        gunAnimator.SetTrigger("isDead");
    }

    public bool IsDead()
    {
        return isDead;
    }
}
