using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTurretController : MonoBehaviour
{
    private bool chasing;
    private float distanceToChase = 32f, distanceToLose = 45f;
    private Vector3 targetPoint;
    private static Transform playerTransform;
    private static AudioSource playerAlarmSound;
    private static MusicFader musicController;
    private static AudioManager bulletSoundMaker;
    private float distRandomOffset;
    private float viewAngle = 25.0f; // should roughly match light cone
    private float sprayFireAng = 2.0f;

    public Transform scanEdgeA;
    public Transform scanEdgeB;
    public GameObject cameraModel;
    private bool scanningTowardA = false;
    private float scanProgressPerc = 0.0f;
    private float scanProgressWaiting = 0.0f;
    private float scanProgressWaitLengthMin = 1.5f;
    private float scanProgressWaitLengthAddedRand = 1.0f;
    private Quaternion returnFromOrientation;
    private Quaternion returnOrientation;
    private float restoreProgress = 1.0f;
    float cameraScanSpeed = 0.17f;
    public GameObject bullet;
    public Transform muzzleLoc;

    public GameObject searchCone;
    public GameObject seeYouLight;
    public GameObject blinkLight;

    Quaternion nervousSearchFacing;

    // Start is called before the first frame update
    void Start()
    {
        cameraModel.SetActive(true);
        scanEdgeA.gameObject.SetActive(false);
        scanEdgeB.gameObject.SetActive(false);

        nervousSearchFacing = transform.rotation;
        distRandomOffset = Random.Range(0.0f, 7.0f);
        if (playerTransform == null)
        {
            playerTransform = GameObject.Find("Player").transform;
            playerAlarmSound = playerTransform.GetComponent<AudioSource>();
        }
        if (musicController == null)
        {
            GameObject musicGO = GameObject.Find("Music");
            if (musicGO)
            {
                musicController = musicGO.GetComponent<MusicFader>();
            }
        }
        if(bulletSoundMaker == null)
        {
            bulletSoundMaker = Camera.main.GetComponent<AudioManager>();
        }

        StartCoroutine(FireRound());
        StartCoroutine(BlinkLightTimer());
        UpdateLightMode();
    }

    void UpdateLightMode()
    {
        searchCone.SetActive(!chasing);
        seeYouLight.SetActive(chasing);
    }

    IEnumerator BlinkLightTimer()
    {
        float blinkTimeOn = 0.25f;
        float blinkTimeOff = 0.85f;

        // get cameras blinking out of sync
        blinkLight.SetActive(false);
        yield return new WaitForSeconds(Random.Range(0.0f, blinkTimeOff));

        while (true)
        {
            blinkLight.SetActive(true);
            yield return new WaitForSeconds(blinkTimeOn);
            blinkLight.SetActive(false);
            yield return new WaitForSeconds(blinkTimeOff);
        }
    }

    IEnumerator FireRound()
    {
        while (true)
        {
            if (chasing)
            {
                Quaternion fireDir = muzzleLoc.rotation;
                fireDir *= Quaternion.AngleAxis(Random.Range(-sprayFireAng, sprayFireAng), muzzleLoc.up);
                fireDir *= Quaternion.AngleAxis(Random.Range(-sprayFireAng, sprayFireAng), muzzleLoc.right);
                GameObject.Instantiate(bullet, muzzleLoc.position, fireDir);
                bulletSoundMaker.PlayRandSound(AudioManager.SoundType.mac10, gameObject);
                bulletSoundMaker.PlayRandSound(AudioManager.SoundType.shells, gameObject);
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
        } else
        {
            if(restoreProgress<1.0f)
            {
                restoreProgress += Time.deltaTime * cameraScanSpeed;
                if(restoreProgress>1.0f)
                {
                    restoreProgress = 1.0f;
                }
                transform.rotation = Quaternion.Slerp(returnFromOrientation, returnOrientation, restoreProgress);
                return;
            }
            else if (scanProgressWaiting > 0.0f)
            {
                scanProgressWaiting -= Time.deltaTime;
            }
            else if (scanningTowardA)
            {
                scanProgressPerc += Time.deltaTime * cameraScanSpeed;
                if (scanProgressPerc > 1.0f)
                {
                    scanProgressPerc = 1.0f;
                    scanningTowardA = false;
                    scanProgressWaiting = scanProgressWaitLengthMin + Random.Range(0.0f, scanProgressWaitLengthAddedRand);
                }
            }
            else
            {
                scanProgressPerc -= Time.deltaTime * cameraScanSpeed;
                if (scanProgressPerc < 0.0f)
                {
                    scanProgressPerc = 0.0f;
                    scanningTowardA = true;
                    scanProgressWaiting = scanProgressWaitLengthMin + Random.Range(0.0f, scanProgressWaitLengthAddedRand);
                }
            }
            transform.rotation = Quaternion.Slerp(scanEdgeA.rotation, scanEdgeB.rotation, scanProgressPerc);
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
        returnOrientation = Quaternion.Slerp(scanEdgeA.rotation, scanEdgeB.rotation, scanProgressPerc);
        UpdateLightMode();
    }

    // Update is called once per frame
    void Update()
        {
        targetPoint = playerTransform.position + Vector3.up * 0.25f;

        if (!chasing)
        {
            if (Vector3.Distance(transform.position, targetPoint) < distanceToChase + distRandomOffset &&
                Quaternion.Angle(transform.rotation,
                        Quaternion.LookRotation(playerTransform.position-transform.position))<viewAngle)
            {
                if (chasing==false && LineOfSightToPlayer())
                {
                    chasing = true;
                    playerAlarmSound.Play();
                    returnOrientation = Quaternion.Slerp(scanEdgeA.rotation, scanEdgeB.rotation, scanProgressPerc);

                    UpdateLightMode();
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
                    returnFromOrientation = transform.rotation;
                    restoreProgress = 0.0f;

                    UpdateLightMode();
                }
            }
        }


    }
}
