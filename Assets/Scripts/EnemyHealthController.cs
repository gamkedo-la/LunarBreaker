using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthController : MonoBehaviour
{
    public GameObject deathPrefabEffect;
    public int currentHealth = 5;
    private EnemyController myControllerDrone;
    private CameraTurretController myControllerCamera;

    void Start()
    {
        myControllerDrone = GetComponent<EnemyController>();
        // super lazy hack to work for either, should be a separate shared component, will refactor if we get more enemy types -ChrisD
        myControllerCamera = GetComponent<CameraTurretController>();
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
            if(myControllerDrone)
            {
                myControllerDrone.DamageAlert();
            } else
            {
                myControllerCamera.DamageAlert();
            }
        }
    }


}
