using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitSequenceStart : MonoBehaviour
{
    public Transform elevatorDoor;
    public Transform elevatorDoorDest;
    public Transform elevatorFloor;

    private bool firedYet = false;
    private float endTimer = 6.0f;

    private void OnTriggerEnter(Collider other)
    {
        if(firedYet)
        {
            return;
        }

        PlayerController pcScript = other.GetComponent<PlayerController>();
        if(pcScript)
        {
            firedYet = true;
            AudioSource soundSource = GetComponent<AudioSource>();
            soundSource.Play();
        }
    }

    private void FixedUpdate()
    {
        if(firedYet)
        {
            elevatorDoor.position = Vector3.Lerp(elevatorDoor.position,
                elevatorDoorDest.position, 0.1f);
            elevatorFloor.transform.position += Vector3.up * Time.deltaTime * 2.0f;
            endTimer -= Time.deltaTime;
            if(endTimer < 0.0f)
            {
                SceneManager.LoadScene("StoryEnd");
            }
        }
    }
}
