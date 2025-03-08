using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Mac10Controller : MonoBehaviour
{
    public float swayAmount;
    public float swaySmoothing;

    public PlayerController playerController;


    private void Update()
    {
        Vector2 mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * playerController.mouseSensitivity;
        Quaternion targetRotation = Quaternion.AngleAxis(mouseInput.x,Vector3.up) * Quaternion.AngleAxis(-mouseInput.y, Vector3.right);

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, swaySmoothing * Time.deltaTime);
    }
}


