using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public float moveSpeed, gravityModifier, jumpPower, runSpeed;
    public CharacterController charCon;
    public Vector3 moveInput;

    public Transform camTrans;
    public RectTransform aimCursor;
    public Camera UICamera;

    public float mouseSensitivity;
    public bool invertX;
    public bool invertY;

    private bool canJump, canDoubleJump;
    public Transform groundCheckPoint;
    public LayerMask whatIsGround;

    public Animator anim;

    public GameObject bullet;
    public Transform firePoint;
    public bool isFiring;
    public bool isReloading;
    public bool isSprinting;
    private bool wasGrounded = true;

    private float mac10ReloadTime = 0.15f;
    private float revolverReloadTime = 0.6f;

    public TextMeshProUGUI ammoDisplayText;
    private int Mac10Ammo = 10;
    private int RevolverAmmo = 4;

    //raycast shooting
    public float damage = 10f;
    public float range = 100f;


    //vfx
    //public GameObject vfx_muzzleflash_m10;
    public ParticleSystem vfx_muzzleflash_m10_A;
    public ParticleSystem vfx_muzzleflash_m10_B;
    public ParticleSystem vfx_muzzleflash_rev_web;
    public GameObject vfx_bullet_hole;
    public GameObject vfx_bullet_spark;
    public GameObject damagePrefabEffect;

    //audio
    public AudioManager audioManager;

    private AudioSource alarmSawMe;
    private float recentlyMadeLandingOrJumpNoise = 0.0f;
    private float recentlyMadeLandingOrJumpNoiseDelay = 0.3f;

    //gun
    private bool usingMac10 = true;
    public Transform mac10Barrel;
    public Transform nagentBarrel;
    public Transform mac10ShellPort;

    public GameObject mac10Holder;
    public GameObject nagantRevolverHolder;
    public GameObject mac10ShellPrefab;

    public GameObject deadPrefab;
    private bool diedYet = false;

    // Health
    PlayerHealth playerHealth;

    public void Awake()
    {
        instance = this;
        playerHealth = GetComponent<PlayerHealth>();
        alarmSawMe = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Debug.Log("9 and 8 KEY TELEPORTS TO END OR TEST AREA - delete TeleportTest gameobject for release");

        isFiring = false;
        isReloading = false;
        isSprinting = false;

        UpdateAmmoReadout();
    }

    IEnumerator Reloading()
    {
        yield return new WaitForSeconds(1.167f);
        isReloading = false;
    }

    Transform currentGunTransform()
    {
        if (usingMac10)
        {
            return mac10Barrel;
        }
        else
        {
            return nagentBarrel;
        }
    }

    private void UpdateAmmoReadout()
    {
        ammoDisplayText.text = "1: MAC-10 ("+Mac10Ammo+")\n2: Nagant ("+RevolverAmmo+")";
    }

    IEnumerator Shoot()
    {
        float outOfAmmoShakeTime = 0.3f;
        do
        {
            isFiring = true;
            
            if (usingMac10)
            {
                if(Mac10Ammo<=0)
                {
                    float moveDuringNoMac10AmmoTime = outOfAmmoShakeTime;
                    Vector3 macLocalPos = mac10Holder.transform.localPosition;
                    Quaternion macLocalRot = mac10Holder.transform.localRotation;
                    if (audioManager)
                    {
                        audioManager.PlayRandSound(AudioManager.SoundType.emptyGun,
                            this.transform.parent.gameObject);
                    }
                    while (moveDuringNoMac10AmmoTime >= 0.0f)
                    {
                        moveDuringNoMac10AmmoTime -= Time.fixedDeltaTime;
                        float percBack = moveDuringNoMac10AmmoTime / outOfAmmoShakeTime;
                        if (percBack < 0.0f)
                        {
                            percBack = 0.0f;
                        }
                        percBack *= percBack * percBack;
                        mac10Holder.transform.localRotation = macLocalRot * Quaternion.AngleAxis(1.5f * percBack, Vector3.right);
                        mac10Holder.transform.localPosition = macLocalPos + Vector3.forward * 0.1f * percBack;
                        yield return new WaitForFixedUpdate();
                    }
                    isFiring = false;
                    break; // escaping the do-while of this coroutine IEnumerator
                }
                if (audioManager)
                {
                    audioManager.PlayRandSound(AudioManager.SoundType.mac10,
                        this.transform.parent.gameObject);
                }
                Mac10Ammo--;
                //vfx_muzzleflash_m10.GetComponent<VisualEffect>().Play();
                vfx_muzzleflash_m10_A.Play();
                vfx_muzzleflash_m10_B.Play();
            }
            else
            {
                if (RevolverAmmo <= 0)
                {
                    float moveDuringNoRevolverTime = outOfAmmoShakeTime;
                    if (audioManager)
                    {
                        audioManager.PlayRandSound(AudioManager.SoundType.emptyGun,
                            this.transform.parent.gameObject);
                    }
                    while (moveDuringNoRevolverTime >= 0.0f)
                    {
                        moveDuringNoRevolverTime -= Time.fixedDeltaTime;
                        float percBack = moveDuringNoRevolverTime / outOfAmmoShakeTime;
                        if (percBack < 0.0f)
                        {
                            percBack = 0.0f;
                        }
                        percBack *= percBack * percBack;
                        nagantRevolverHolder.transform.localRotation = Quaternion.AngleAxis(1.5f * percBack, Vector3.right);
                        nagantRevolverHolder.transform.localPosition = Vector3.forward * 0.1f * percBack;
                        yield return new WaitForFixedUpdate();
                    }
                    isFiring = false;
                    break; // escaping the do-while of this coroutine IEnumerator
                }
                if (audioManager)
                {
                    for(int i=0;i<3;i++) // overlap a few for bigger revolver hit
                    {
                        audioManager.PlayRandSound(AudioManager.SoundType.revolver,
                            this.transform.parent.gameObject);
                    }
                }
                RevolverAmmo--;
                vfx_muzzleflash_rev_web.Play();
            }


            UpdateAmmoReadout();

            Transform gunBarrel = currentGunTransform();
            RaycastHit hit;

            Quaternion fireSprayDir = Quaternion.identity;
            if (usingMac10) {
                float sprayFireAng = 3.0f;
                fireSprayDir = Quaternion.AngleAxis(Random.Range(-sprayFireAng, sprayFireAng), Vector3.up)
                    * Quaternion.AngleAxis(Random.Range(-sprayFireAng, sprayFireAng), Vector3.right);

                float sprayShellAng = 5.0f;
                Quaternion shellSprayDir = Quaternion.AngleAxis(Random.Range(-sprayShellAng, sprayShellAng), Vector3.up)
                    * Quaternion.AngleAxis(Random.Range(-sprayShellAng, sprayShellAng), Vector3.right);

                GameObject shellGO = Instantiate(mac10ShellPrefab, mac10ShellPort.position, mac10ShellPort.rotation);
                Rigidbody shellRB = shellGO.GetComponent<Rigidbody>();
                float randForceMin = 175.0f;
                float randForceMax = 220.0f;
                shellRB.AddForce(shellSprayDir * mac10ShellPort.forward * Random.Range(randForceMin, randForceMax));
            }

            if (Physics.Raycast(gunBarrel.position - gunBarrel.forward*2.0f, fireSprayDir * gunBarrel.forward, out hit, range))
            {
                EnemyHealthController enemy = hit.transform.GetComponent<EnemyHealthController>();
                if (enemy != null)
                {
                    enemy.DamageEnemy(usingMac10 ? 1 : 5);
                    Instantiate(damagePrefabEffect, hit.point + new Vector3(0.1f, 0.1f, 0.1f), Quaternion.FromToRotation(Vector3.up, hit.normal));
                }
                else if (hit.transform.gameObject.layer == 6)
                {
                    Instantiate(vfx_bullet_hole, hit.point + new Vector3(0.1f, 0.1f, 0.1f), Quaternion.FromToRotation(Vector3.up, hit.normal));
                }
                else
                {
                    Instantiate(vfx_bullet_spark, hit.point + new Vector3(0.1f, 0.1f, 0.1f), Quaternion.FromToRotation(Vector3.up, hit.normal));
                }

            }
            if(usingMac10)
            {
                yield return new WaitForSeconds(mac10ReloadTime);
            } else
            {
                float moveDuringReloadTime = revolverReloadTime;
                while(moveDuringReloadTime >= 0.0f)
                {
                    moveDuringReloadTime -= Time.fixedDeltaTime;
                    float percBack = moveDuringReloadTime/revolverReloadTime;
                    if(percBack < 0.0f)
                    {
                        percBack = 0.0f;
                    }
                    percBack *= percBack * percBack; // simple way to achieve a nonlinear return
                    nagantRevolverHolder.transform.localRotation = Quaternion.AngleAxis(-7.0f* percBack, Vector3.right);
                    nagantRevolverHolder.transform.localPosition = Vector3.back * 1.5f * percBack;
                    yield return new WaitForFixedUpdate();
                }
            }
            
            isFiring = false;
            yield return new WaitForSeconds(0.05f);
        } while (Input.GetMouseButton(0) && usingMac10);
    }

    public void AddAmmoMac10(int howMuch)
    {
        Mac10Ammo += howMuch;
        UpdateAmmoReadout();
    }

    public void AddAmmoRevolver(int howMuch)
    {
        RevolverAmmo += howMuch;
        UpdateAmmoReadout();
    }

    void SetGunMac10(bool equipMac10)
    {
        if(usingMac10 != equipMac10)
        {
            audioManager.PlayRandSound(AudioManager.SoundType.emptyGun,
                            this.transform.parent.gameObject);
        }
        usingMac10 = equipMac10;
        mac10Holder.SetActive(usingMac10);
        nagantRevolverHolder.SetActive(!usingMac10);
    }

    private void FixedUpdate() // for using Lerp predictably
    {

        Transform gunBarrel = currentGunTransform();
        RaycastHit hit;
        Vector3 world2Screen;
        if (Physics.Raycast(gunBarrel.position, gunBarrel.forward, out hit, range))
        {
            world2Screen = UICamera.WorldToScreenPoint(hit.point);
        } else
        {
            world2Screen = UICamera.WorldToScreenPoint(gunBarrel.position + gunBarrel.forward*10.0f);
        }
        aimCursor.position = Vector3.Lerp(aimCursor.position, world2Screen, 0.85f);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerHealth.IsDead())
        {
            if(diedYet == false )
            {
                Instantiate(deadPrefab, transform.position, Quaternion.identity);
                diedYet = true;
            }
            return;
        }

        if (Input.GetKey(KeyCode.Alpha1))
        {
            SetGunMac10(true);
        }

        if (Input.GetKey(KeyCode.Alpha2))
        {
            SetGunMac10(false);
        }

        // DEBUG ONLY!!! ---v
		/*if (Input.GetKey(KeyCode.Alpha9))
        {
            Debug.Log("TELEPORTING TO END BOSS");
			GameObject gotoGO = GameObject.Find("TeleportTest");
            CharacterController controller = GetComponent<CharacterController>();
            if (gotoGO && controller)
            {
                controller.enabled = false;
                transform.position = gotoGO.transform.position;
                controller.enabled = true;
            }

        }
        // DEBUG ONLY!!! ---v
        if (Input.GetKey(KeyCode.Alpha8))
        {
            Debug.Log("TELEPORTING TO FRONT ENTRANCE");
            GameObject gotoGO = GameObject.Find("TeleportTest2");
            CharacterController controller = GetComponent<CharacterController>();
            if (gotoGO && controller)
            {
                controller.enabled = false;
                transform.position = gotoGO.transform.position;
                controller.enabled = true;
            }

        }*/
		
		
        float yStore = moveInput.y;

        // instant stop
        // Vector3 vertMove = transform.forward * Input.GetAxisRaw("Vertical");
        // Vector3 horiMove = transform.right * Input.GetAxisRaw("Horizontal");
        // smooth decel stop
        Vector3 vertMove = transform.forward * Input.GetAxisRaw("Vertical");
        Vector3 horiMove = transform.right * Input.GetAxisRaw("Horizontal");

        moveInput = horiMove + vertMove;
        moveInput.Normalize();

        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveInput = moveInput * runSpeed;
            isSprinting = true;
        }
        else
        {
            moveInput = moveInput * moveSpeed;
            isSprinting = false;
        }

        moveInput.y = yStore;

        moveInput.y += Physics.gravity.y * gravityModifier * Time.deltaTime;


        if (charCon.isGrounded)
        {
            moveInput.y = Physics.gravity.y * gravityModifier * Time.deltaTime;
        }


        canJump = Physics.OverlapSphere(groundCheckPoint.position, .25f, whatIsGround).Length > 0;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (canJump)
            {
                moveInput.y = jumpPower;
                canDoubleJump = true;
            }
            else if (canDoubleJump)
            {
                moveInput.y = jumpPower;
                canDoubleJump = false;
            }
        }

        bool groundedForNoisePurposes = Physics.OverlapSphere(groundCheckPoint.position, 0.5f, whatIsGround).Length > 0;

        if(recentlyMadeLandingOrJumpNoise>0.0f)
        {
            recentlyMadeLandingOrJumpNoise -= Time.deltaTime;
        }
        
        if (wasGrounded != groundedForNoisePurposes)
        {
            if(recentlyMadeLandingOrJumpNoise <= 0.0f)
            {
                recentlyMadeLandingOrJumpNoise = recentlyMadeLandingOrJumpNoiseDelay;
                if (wasGrounded)
                {
                    audioManager.PlayRandSound(AudioManager.SoundType.playerLand,
                                this.transform.parent.gameObject);
                }
                else
                {
                    audioManager.PlayRandSound(AudioManager.SoundType.playerJump,
                                this.transform.parent.gameObject);
                }
            }
            wasGrounded = groundedForNoisePurposes;
        }



        charCon.Move(moveInput * Time.deltaTime);

        //Cam Rotation

        Vector2 mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;

        if (invertX)
        {
            mouseInput.x = -mouseInput.x;
        }
        if (invertY)
        {
            mouseInput.y = -mouseInput.y;
        }
        Mathf.Clamp(mouseInput.x, -90.0f, 90.0f);

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z);
        Quaternion camAngleBeforeChange = camTrans.rotation;
        camTrans.rotation = Quaternion.Euler(camTrans.rotation.eulerAngles + new Vector3(-mouseInput.y, 0f, 0f));
        if(camTrans.up.y < 0.0f) // camera inverted, abrupt repair to bad state
        {
            // Debug.Log("player flipped camera, undoing last change");
            camTrans.rotation = camAngleBeforeChange;
        }

        // Shooting
        if (Input.GetMouseButtonDown(0))
        {
            if (!isFiring)
            {
                StartCoroutine(Shoot());
            }

        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Reloading" + isReloading);
            if (!isReloading)
            {
                isReloading = true;
                StartCoroutine(Reloading());
            }

        }

        anim.SetFloat("moveSpeed", moveInput.magnitude, 0.05f, Time.deltaTime);
        anim.SetBool("onGround", canJump);
        anim.SetBool("isFiring", isFiring);
        anim.SetBool("isReloading", isReloading);
        anim.SetBool("isSprinting", isSprinting);

    }

    public void Teleport(Vector3 position, Quaternion rotation)
    {
        charCon.enabled = false;
        charCon.transform.position = position;
        if (rotation != null)
        {
            charCon.transform.rotation = rotation;
        }
        charCon.enabled = true;
    }
}
