using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    private float sleepDistance = 220.0f;
    private bool sleeping = false;
    private float wanderRange = 10.0f;
    public NavMeshAgent agent;
    public GameObject bullet;
    public Transform muzzleLoc;

    public GameObject searchCone;
    public GameObject seeYouLight;


    Quaternion nervousSearchFacing;

    // Start is called before the first frame update
    void Start()
    {
        nervousSearchFacing = transform.rotation;
        distRandomOffset = Random.Range(0.0f,7.0f);
        if (playerTransform == null)
        {
            playerTransform = GameObject.Find("Player").transform;
        }
        StartCoroutine(SwitchStrafeOrSearchDir());
        StartCoroutine(FireRound());
        UpdateLightMode();
    }

    void UpdateLightMode()
    {
        searchCone.SetActive(!chasing);
        seeYouLight.SetActive(chasing);
    }

    IEnumerator SwitchStrafeOrSearchDir()
    {
        while(true)
        {
            if (chasing || sleeping)
            {
                strafeCW = !strafeCW;
            }
            else
            {
                agent.SetDestination(PickNearbyGoal(wanderRange));
                // nervousSearchFacing *= Quaternion.AngleAxis(Random.RandomRange(-55.0f, 55.0f), Vector3.up);
            }
            yield return new WaitForSeconds( Random.Range(1.5f,4.0f) );
        }
    }

    IEnumerator FireRound()
    {
        while(true)
        {
            if(chasing)
            {
                GameObject.Instantiate(bullet, muzzleLoc.position, muzzleLoc.rotation);
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

    private void FixedUpdate() // since slerp uses % it isn't linear
    {
        if (chasing)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(targetPoint - transform.position), 0.1f);
        }
    }

    Vector3 PickNearbyGoal(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return transform.position;
    }

    void UseNavMesh(bool useNav)
    {
        if (useNav)
        {
            chasing = false;
            rigidbody.isKinematic = true;
            agent.enabled = true;
            agent.SetDestination(PickNearbyGoal(wanderRange));
            UpdateLightMode();
        }
        else
        {
            chasing = true;
            rigidbody.isKinematic = false;
            if(agent.enabled)
            {
                agent.ResetPath();
            }
            agent.enabled = false;
            UpdateLightMode();
        }
    }

    bool LineOfSightToPlayer()
    {
        RaycastHit rhInfo;

        Physics.Raycast(playerTransform.position, transform.position - playerTransform.position,
                out rhInfo);

        return rhInfo.collider.gameObject == gameObject;
    }

    public void DamageAlert()
    {
        chasing = true;
        UseNavMesh(false);
        UpdateLightMode();
    }

    // Update is called once per frame
    void Update()
        {
        /* // not sure this is working yet, leaving out until tested
        float distFromPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distFromPlayer > sleepDistance)
        {
            if(sleeping==false)
            {
                chasing = false; // turn off particles
                UpdateLightMode();
            }
            sleeping = true;
            return;
        } else if(sleeping)
        {
            UseNavMesh(true);
        }*/

        targetPoint = playerTransform.position;
        // targetPoint.y = transform.position.y;

        if (!chasing)
        {
            if (Vector3.Distance(transform.position, targetPoint) < distanceToChase + distRandomOffset &&
                Quaternion.Angle(transform.rotation,
                        Quaternion.LookRotation(playerTransform.position-transform.position))<viewAngle)
            {
                if (chasing==false && LineOfSightToPlayer())
                {
                    UseNavMesh(false);
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

            RaycastHit rhInfo;

            if(Physics.Raycast(transform.position, Vector3.down, out rhInfo))
            {
                float hoverDist = Vector3.Distance(transform.position, rhInfo.point);
                float hoverMin = 4.5f;
                float hoverMax = 7.0f;
                if (hoverDist < hoverMin)
                {
                    rigidbody.velocity += transform.up * 2.0f;
                }
                else if (hoverDist > hoverMax)
                {
                    rigidbody.velocity += transform.up * -2.0f;
                }
            }



            if (Vector3.Distance(transform.position,targetPoint) > distanceToLose + distRandomOffset ||
                LineOfSightToPlayer() == false)
            {
                if(chasing)
                {
                    UseNavMesh(true);
                }
            }
        }


    }
}
