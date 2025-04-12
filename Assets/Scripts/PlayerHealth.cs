using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] float maxHealth = 10;
    [SerializeField] Slider healthSlider;
    [SerializeField] float timeBeforeReloadAfterDeath = 2.0f;

    float currentHealth = 10;
    bool isDead = false;

    LevelManager levelManager;

    void Awake()
    {
        levelManager = FindObjectOfType<LevelManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthSlider.value = currentHealth / maxHealth;
        isDead = false;
    }

    public void GetDamage(float damage)
    {
        currentHealth -= damage;
        healthSlider.value = currentHealth / maxHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        isDead = true;
        StartCoroutine(ReloadSceneAfterTime());
    }

    private IEnumerator ReloadSceneAfterTime()
    {
        yield return new WaitForSeconds(timeBeforeReloadAfterDeath);
        levelManager.ReloadCurrentScene();
    }

    public bool IsDead()
    {
        return isDead;
    }
}
