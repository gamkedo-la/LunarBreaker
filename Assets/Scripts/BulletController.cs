using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{

    public float moveSpeed, lifeTime;
    public Rigidbody rb;
    public GameObject impactFX;

    public int damage = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = transform.forward * moveSpeed;

        lifeTime -= Time.deltaTime;

        if(lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision coll)
    {
        PlayerHealth phScript = coll.gameObject.GetComponent<PlayerHealth>();
        if(phScript)
        {
            phScript.GetDamage(1.0f);
        }
        Destroy(gameObject);
        Instantiate(impactFX, transform.position + (transform.forward * (-moveSpeed * Time.deltaTime)), transform.rotation);
    }

}
