using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public float moveSpeed, gravityModifier, jumpPower, runSpeed;
    public CharacterController charCon;
    private Vector3 moveInput;

    public Transform camTrans;

    public float mouseSensitivity;
    public bool invertX;
    public bool invertY;

    private bool canJump,canDoubleJump;
    public Transform groundCheckPoint;
    public LayerMask whatIsGround;

    public Animator anim;

    public GameObject bullet;
    public Transform firePoint;
    public bool isFiring;
    public bool isReloading;
    public bool isSprinting;

    public void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        isFiring = false;
        isReloading = false;
        isSprinting = false;
    }

    IEnumerator Reloading()
    {
        yield return new WaitForSeconds(1.167f);
        isReloading = false;
    }

    IEnumerator Shoot()
    {

        //shoots bullet towards crosshair
        RaycastHit hit;
        if (Physics.Raycast(camTrans.position, camTrans.forward, out hit, 50f))
        {
            //check for upclose
            if (Vector3.Distance(camTrans.position, hit.point) > 2f)
            {
                firePoint.LookAt(hit.point);
            }

        }
        else
        {
            firePoint.LookAt(camTrans.position + (camTrans.forward * 30f));
        }
        Instantiate(bullet, firePoint.position, firePoint.rotation);
        
        yield return new WaitForSeconds(0.433f);
        isFiring = false;
    }

        // Update is called once per frame
        void Update()
    {
        //moveInput.x = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        //moveInput.z = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

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

        if(invertX)
        {
            mouseInput.x = -mouseInput.x;
        }
        if (invertY)
        {
            mouseInput.y = -mouseInput.y;
        }

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z);
        camTrans.rotation = Quaternion.Euler(camTrans.rotation.eulerAngles + new Vector3(-mouseInput.y,0f,0f));

        // Shooting
        if (Input.GetMouseButtonDown(0))
        {
            if (!isFiring)
            {
                isFiring = true;
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


        /*
        if(Input.GetMouseButtonDown(0))
        {
            firing = true;
            //shoots bullet towards crosshair
            RaycastHit hit;
            if (Physics.Raycast(camTrans.position,camTrans.forward,out hit, 50f))
            {
                //check for upclose
                if (Vector3.Distance(camTrans.position, hit.point) > 2f)
                {
                    firePoint.LookAt(hit.point);
                }
                
            }
            else
            {
                firePoint.LookAt(camTrans.position + (camTrans.forward * 30f));
            }
            Instantiate(bullet, firePoint.position, firePoint.rotation);            
        }
        */

        anim.SetFloat("moveSpeed", moveInput.magnitude, 0.05f, Time.deltaTime);
        anim.SetBool("onGround",canJump);
        anim.SetBool("isFiring", isFiring);
        anim.SetBool("isReloading", isReloading);
        anim.SetBool("isSprinting", isSprinting);

    }
}
