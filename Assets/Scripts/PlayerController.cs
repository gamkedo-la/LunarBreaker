using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

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

    //raycast shooting
    public float damage = 10f;
    public float range = 100f;


    //vfx
    public GameObject vfx_muzzleflash_m10;
    public GameObject vfx_muzzleflash_rev;
    public GameObject vfx_bullet_hole;
    public GameObject vfx_bullet_spark;
    public GameObject damagePrefabEffect;

    //audio
    public AudioManager audioManager;



    //gun
    private bool usingMac10 = true;
    public Transform mac10Barrel;
    public Transform nagentBarrel;

    public GameObject mac10Holder;
    public GameObject nagantRevolverHolder;

    // Health
    PlayerHealth playerHealth;

    public void Awake()
    {
        instance = this;
        playerHealth = GetComponent<PlayerHealth>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("9 KEY TELEPORTS TO END OR TEST AREA - delete TeleportTest gameobject for release");

        isFiring = false;
        isReloading = false;
        isSprinting = false;
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

    IEnumerator Shoot()
    {
        do
        {
            isFiring = true;
            //audioManager.PlayGunshot(this.transform.parent.gameObject);
            if (usingMac10)
            {
                vfx_muzzleflash_m10.GetComponent<VisualEffect>().Play();
            }
            else
            {
                vfx_muzzleflash_rev.GetComponent<VisualEffect>().Play();
            }

            Transform gunBarrel = currentGunTransform();
            RaycastHit hit;

            Quaternion fireSprayDir = Quaternion.identity;
            if (usingMac10) {
                float sprayFireAng = 7.0f;
                fireSprayDir = Quaternion.AngleAxis(Random.Range(-sprayFireAng, sprayFireAng), Vector3.up)
                    * Quaternion.AngleAxis(Random.Range(-sprayFireAng, sprayFireAng), Vector3.right);
            }

            if (Physics.Raycast(gunBarrel.position, fireSprayDir * gunBarrel.forward, out hit, range))
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

            yield return new WaitForSeconds(usingMac10 ? 0.15f : 0.6f);
            isFiring = false;
            yield return new WaitForSeconds(0.05f);
        } while (Input.GetMouseButton(0) && usingMac10);
    }

    void SetGunMac10(bool equipMac10)
    {
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
        aimCursor.position = Vector3.Lerp(aimCursor.position, world2Screen, 0.8f);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerHealth.IsDead()) return;

        if (Input.GetKey(KeyCode.Alpha1))
        {
            SetGunMac10(true);
        }

        if (Input.GetKey(KeyCode.Alpha2))
        {
            SetGunMac10(false);
        }

        if (Input.GetKey(KeyCode.Alpha9))
        {
            GameObject gotoGO = GameObject.Find("TeleportTest");
            CharacterController controller = GetComponent<CharacterController>();
            if (gotoGO && controller)
            {
                controller.enabled = false;
                transform.position = gotoGO.transform.position;
                controller.enabled = true;
            }

        }

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
        camTrans.rotation = Quaternion.Euler(camTrans.rotation.eulerAngles + new Vector3(-mouseInput.y, 0f, 0f));

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
