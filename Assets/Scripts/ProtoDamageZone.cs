using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtoDamageZone : MonoBehaviour
{
    [SerializeField] float damage = 1.0f;

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerHealth>() == null) return;

        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        playerHealth.GetDamage(damage);
    }
}
