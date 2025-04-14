using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Mac10Controller : MonoBehaviour
{
    public float swayAmount;
    public float swaySmoothing;
    public PlayerController playerController;

    private Quaternion initialRotation;

    private void Start()
    {
        initialRotation = transform.localRotation;
    }

    private void Update()
    {
        // Get raw mouse input
        Vector2 mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * playerController.mouseSensitivity;

        // Apply sway amount multiplier
        float swayX = mouseInput.x * swayAmount;
        float swayY = -mouseInput.y * swayAmount; // Negative to match typical FPS controls

        // Calculate target rotation with both axes considered
        Quaternion targetRotation = initialRotation * Quaternion.Euler(swayY, swayX, 0);

        // Smooth the rotation
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, swaySmoothing * Time.deltaTime);

    }
}


