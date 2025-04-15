using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTurretController : MonoBehaviour
{

    public Rigidbody rigidbody;
    private bool chasing;
    private float distanceToStop = 7f, distanceToChase = 32f, distanceToLose = 45f;
    private Vector3 targetPoint;
    private static Transform playerTransform;
    private static MusicFader musicController;
    private bool strafeCW = false;
    private float moveSpeed = 6.0f;
    private float strafeSpeed = 5.0f;
    private float distRandomOffset;
    private float viewAngle = 25.0f; // should roughly match light cone
    private float sleepDistance = 220.0f;
    private bool sleeping = false;
    private float wanderRange = 10.0f;
    private float sprayFireAng = 2.0f;

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
        if(musicController == null)
        {
            musicController = GameObject.Find("Music").GetComponent<MusicFader>();
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
            yield return new WaitForSeconds( Random.Range(1.5f,4.0f) );
        }
    }

    IEnumerator FireRound()
    {
        while(true)
        {
            if(chasing)
            {
                Quaternion fireDir = muzzleLoc.rotation;
                fireDir *= Quaternion.AngleAxis(Random.Range(-sprayFireAng, sprayFireAng), muzzleLoc.up);
                fireDir *= Quaternion.AngleAxis(Random.Range(-sprayFireAng, sprayFireAng), muzzleLoc.right);
                GameObject.Instantiate(bullet, muzzleLoc.position, fireDir);
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
                    chasing = true;
                }
            }
        }
        else
        {
            if (musicController)
            {
                musicController.CombatMusicBump(); // keep refreshing time until after combat/escape
            }

            if (Vector3.Distance(transform.position,targetPoint) > distanceToLose + distRandomOffset ||
                LineOfSightToPlayer() == false)
            {
                if(chasing)
                {
                    chasing = false;
                }
            }
        }


    }
}
