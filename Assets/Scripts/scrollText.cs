using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrollText : MonoBehaviour
{
    
	public float speed = 100f;
	public float maxdist = 2000; // then destroy self
	public float totalDist = 0f;
	public Vector3 startPos;
	
	// Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        totalDist += Time.deltaTime * speed;
		RectTransform rt = GetComponent<RectTransform>();
		rt.position = new Vector2(startPos.x,startPos.y+totalDist);
		//transform.position.Set(startPos.x,startPos.y+totalDist,startPos.z);
		
		if (totalDist > maxdist) {
			gameObject.SetActive(false);
		}
    }
}
