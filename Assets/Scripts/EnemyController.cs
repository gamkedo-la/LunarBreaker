using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    public float moveSpeed;
    public Rigidbody rigidbody;
    private bool chasing;
    public float distanceToChase = 10f, distanceToLose = 15f;
    private Vector3 targetPoint;
    public Transform debugSphere;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        targetPoint = debugSphere.transform.position;
        targetPoint.y = transform.position.y;

        if (!chasing)
        {
            if(Vector3.Distance(transform.position,targetPoint) < distanceToChase)
            {
                chasing = true;
            }
        }
        else
        {

            transform.LookAt(targetPoint);


            rigidbody.velocity = transform.forward * moveSpeed;

            if(Vector3.Distance(transform.position,targetPoint) > distanceToLose)
            {
                chasing = false;
            }
        }


    }
}
