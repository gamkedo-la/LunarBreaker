using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    public Rigidbody rigidbody;
    private bool chasing;
    private float distanceToStop = 7f, distanceToChase = 32f, distanceToLose = 45f;
    private Vector3 targetPoint;
    private static Transform playerTransform;
    private bool strafeCW = false;
    private float moveSpeed = 6.0f;
    private float strafeSpeed = 5.0f;
    private float distRandomOffset;
    private float viewAngle = 25.0f; // should roughly match light cone

    public GameObject searchCone;
    public GameObject seeYouLight;

    // Start is called before the first frame update
    void Start()
    {
        distRandomOffset = Random.RandomRange(0.0f,7.0f);
        if (playerTransform == null)
        {
            playerTransform = GameObject.Find("Player").transform;
        }
        StartCoroutine(SwitchStrafeDir());
        UpdateLightMode();
    }

    void UpdateLightMode()
    {
        searchCone.SetActive(!chasing);
        seeYouLight.SetActive(chasing);
    }

    IEnumerator SwitchStrafeDir()
    {
        while(true)
        {
            strafeCW = !strafeCW;
            yield return new WaitForSeconds( Random.RandomRange(1.0f,3.0f) );
        }
    }

    private void FixedUpdate() // since slerp uses % it isn't linear
    {
        if(chasing)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(targetPoint - transform.position), 0.2f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        targetPoint = playerTransform.position;
        targetPoint.y = transform.position.y;

        if (!chasing)
        {
            if (Vector3.Distance(transform.position, targetPoint) < distanceToChase + distRandomOffset &&
                Quaternion.Angle(transform.rotation,
                        Quaternion.LookRotation(playerTransform.position-transform.position))<viewAngle)
            {
                if (chasing==false)
                {
                    chasing = true;
                    UpdateLightMode();
                }
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, targetPoint) > distanceToStop + distRandomOffset)
            {
                rigidbody.velocity = transform.forward * moveSpeed;
            } else
            {
                rigidbody.velocity = (strafeCW ? -1.0f : 1.0f)*transform.right * strafeSpeed;
            }

            if(Vector3.Distance(transform.position,targetPoint) > distanceToLose + distRandomOffset)
            {
                if(chasing)
                {
                    chasing = false;
                    UpdateLightMode();
                }
            }
        }


    }
}
