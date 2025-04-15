using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    public Rigidbody rigidbody;
    private bool chasing;
    private float distanceToStop = 10f, distanceToChase = 25f, distanceToLose = 40f;
    private Vector3 targetPoint;
    public Transform playerTransform;
    private bool strafeCW = false;
    private float moveSpeed = 6.0f;
    private float strafeSpeed = 5.0f;



    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;
        StartCoroutine(SwitchStrafeDir());
    }

    IEnumerator SwitchStrafeDir()
    {
        while(true)
        {
            strafeCW = !strafeCW;
            yield return new WaitForSeconds( Random.RandomRange(1.0f,3.0f) );
        }
    }

    // Update is called once per frame
    void Update()
    {

        targetPoint = playerTransform.position;
        targetPoint.y = transform.position.y;

        if (!chasing)
        {
            if (Vector3.Distance(transform.position, targetPoint) < distanceToChase)
            {
                chasing = true;
            }
        }
        else
        {

            transform.LookAt(targetPoint);

            if (Vector3.Distance(transform.position, targetPoint) > distanceToStop)
            {
                rigidbody.velocity = transform.forward * moveSpeed;
            } else
            {
                rigidbody.velocity = (strafeCW ? -1.0f : 1.0f)*transform.right * strafeSpeed;
            }

            if(Vector3.Distance(transform.position,targetPoint) > distanceToLose)
            {
                chasing = false;
            }
        }


    }
}
