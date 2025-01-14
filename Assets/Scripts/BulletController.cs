using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{

    public float moveSpeed, lifeTime;
    public Rigidbody rb;
    public GameObject impactFX;

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

    private void OnTriggerEnter(Collider other)
    {

        if(other.gameObject.tag == "Enemy")
        {
            Destroy(other.gameObject);
        }


        Destroy(gameObject);
        //moves particle effect back slightly towards the player so that the effect doesn't go inside of the object it is hitting
        Instantiate(impactFX,transform.position + (transform.forward *(-moveSpeed * Time.deltaTime)),transform.rotation);
    }

}
