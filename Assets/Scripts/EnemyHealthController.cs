using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthController : MonoBehaviour
{
    public GameObject deathPrefabEffect;
    public int currentHealth = 5;
    private EnemyController myController;

    void Start()
    {
        myController = GetComponent<EnemyController>();
    }

    public void DamageEnemy(int damageAmount)
    {
        currentHealth -= damageAmount;

        if(currentHealth <= 0)
        {
            Instantiate(deathPrefabEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        } else
        {
            myController.DamageAlert();
        }
    }


}
