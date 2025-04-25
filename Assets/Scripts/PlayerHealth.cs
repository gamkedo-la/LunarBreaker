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
    [SerializeField] DamageVisualFeedback damageVisualFeedback;

    private float timeBeforeNextDamage = 0.0f;
    private float timeBeforeNextDamageDelay = 0.4f;

    float currentHealth = 10;
    bool isDead = false;

    LevelManager levelManager;

    public AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthSlider.value = currentHealth / maxHealth;
        isDead = false;
        levelManager = FindObjectOfType<LevelManager>();
        healthBar.color = fullHealthColor;
    }

    private void Update()
    {
        if(timeBeforeNextDamage > 0.0f)
        {
            timeBeforeNextDamage -= Time.deltaTime;
        }
    }

    public void GetDamage(float damage)
    {
        if(timeBeforeNextDamage > 0.0f)
        {
            return;
        }
        audioManager.PlayRandSound(AudioManager.SoundType.playerDamaged, gameObject);
        timeBeforeNextDamage = timeBeforeNextDamageDelay;
        currentHealth -= damage;
        // Debug.Log("player hit for " + damage + " now " + currentHealth);
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        damageVisualFeedback.ShowImage();

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
