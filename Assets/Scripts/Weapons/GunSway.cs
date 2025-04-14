using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GunSway : MonoBehaviour
{
    public float swayAmount = 20f;
    public float swaySmoothing = 0.04f;
    public PlayerController playerController;
    public float gunBreathingMovement = 2.0f;

    private Quaternion initialRotation;

    private void Start()
    {
        initialRotation = transform.localRotation;
    }

    private void FixedUpdate()
    {
        // Get raw mouse input
        Vector2 mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * playerController.mouseSensitivity;

        // Apply sway amount multiplier
        float swayX = mouseInput.x * swayAmount;
        float swayY = mouseInput.y * swayAmount;

        swayX += Mathf.Cos(Time.timeSinceLevelLoad)* gunBreathingMovement;
        swayY += Mathf.Sin(Time.timeSinceLevelLoad*0.37f) * gunBreathingMovement*0.3f;

        // Calculate target rotation with both axes considered
        Quaternion targetRotation = initialRotation * Quaternion.Euler(swayY, swayX, 0);

        // Smooth the rotation
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, swaySmoothing);

    }
}


