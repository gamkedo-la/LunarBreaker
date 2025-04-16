using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public float healthGive = 0.0f;
    public int mac10AmmoGive = 0;
    public int revolverAmmoGive = 0;

    private void OnTriggerEnter(Collider other)
    {
        bool playerTouched = false;

        PlayerHealth phScript = other.GetComponent<PlayerHealth>();        
        if(phScript)
        {
            phScript.GetHealth(healthGive);
            playerTouched = true;
        }
        PlayerController pcScript = other.GetComponent<PlayerController>();
        if(pcScript)
        {
            pcScript.AddAmmoMac10(mac10AmmoGive);
            pcScript.AddAmmoRevolver(revolverAmmoGive);
            playerTouched = true;
        }
        if(playerTouched)
        {
            Destroy(gameObject);
        }
    }
}
