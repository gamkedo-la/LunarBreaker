// note: this needs a collider set to TRIGGER

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour
{
    public Transform door1;
    public Transform door2;
	public float speed = 2.0f;
	public float dist = 2.0f;
	
	public AudioClip openSound;
	public AudioClip closeSound;
	private AudioSource sfx;
	
	
	bool open = false;
	Vector3 door1startpos;
	Vector3 door2startpos;
	Vector3 door1endpos;
	Vector3 door2endpos;
	
	// Start is called before the first frame update
    void Start()
    {
        sfx = GetComponent<AudioSource>();
		if (door1) door1startpos = door1.localPosition;
        if (door2) door2startpos = door2.localPosition;
		// FIXME: these could orient properly rather than hardcoded X axis
        if (door1) door1endpos = new Vector3(door1startpos.x+dist,door1startpos.y,door1startpos.z);
        if (door2) door2endpos = new Vector3(door2startpos.x-dist,door2startpos.y,door2startpos.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (open) {
			if (door1) door1.localPosition = Vector3.Lerp(door1.localPosition,door1endpos,Time.deltaTime*speed);
			if (door2) door2.localPosition = Vector3.Lerp(door2.localPosition,door2endpos,Time.deltaTime*speed);
		} else {
			if (door1) door1.localPosition = Vector3.Lerp(door1.localPosition,door1startpos,Time.deltaTime*speed);
			if (door2) door2.localPosition = Vector3.Lerp(door2.localPosition,door2startpos,Time.deltaTime*speed);
		}
    }
	
	void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Debug.Log("opening door");
			open = true;
			if (openSound && sfx) {
				sfx.clip = openSound;
				sfx.Play();
			}
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Debug.Log("closing door");
            open = false;
			if (closeSound && sfx) {
				sfx.clip = closeSound;
				sfx.Play();
			}
        }
    }	
}
