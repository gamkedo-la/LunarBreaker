using UnityEngine;
using System.Collections;

// $CTK this works only if the xform isn't touched by any other components
public class Wobble : MonoBehaviour {

	// just in case we don't want starting pos
	public float offsetX=0.0f; // $CTK
	public float offsetY=0.0f; // $CTK
	public float offsetZ=0.0f; // $CTK

	public float speedX=3.5f;
	public float speedY=3.5f;
	public float speedZ=3.5f;

	// how big is the wobble?
	public float magnitudeX=1;
	public float magnitudeY=1;
	public float magnitudeZ=1;

	// so every object starts at a diff phase
	private float timeoffsetX=0; 
	private float timeoffsetY=0; 
	private float timeoffsetZ=0; 

	// we orbit the starting pos
	private float startingX=0;
	private float startingY=0;
	private float startingZ=0;
	
	// Use this for initialization
	void Start () {
		timeoffsetX=Random.Range(-5f, 5f);
		timeoffsetY=Random.Range(-5f, 5f);
		timeoffsetZ=Random.Range(-5f, 5f);

		startingX=transform.localPosition.x;
		startingY=transform.localPosition.y;
		startingZ=transform.localPosition.z;
	}
	
	// Update is called once per frame
	void Update () {
		float x=offsetX+startingX+magnitudeX*Mathf.Sin(speedX*Time.time+timeoffsetX); // $CTK
		float y=offsetY+startingY+magnitudeY*Mathf.Sin(speedY*Time.time+timeoffsetY); // $CTK
		float z=offsetZ+startingZ+magnitudeZ*Mathf.Sin(speedZ*Time.time+timeoffsetZ); // $CTK
		transform.localPosition=new Vector3(x, y, z);
	}
}
