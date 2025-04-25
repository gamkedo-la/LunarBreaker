using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scrollText : MonoBehaviour
{
	public GameObject mainMenu; // not defined = not in main menu, overriding to reset game from end screen
	public float speed = 100f;
	public float maxdist = 2000; // then destroy self
	public float totalDist = 0f;
	public Vector3 startPos;
	
	// Start is called before the first frame update
    void Start()
    {
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		startPos = GetComponent<RectTransform>().anchoredPosition;
	}

	public void Reset()
    {
		totalDist = 0.0f;
	}

	public void RestartGame()
    {
		SceneManager.LoadScene("MainMenu");
	}

    // Update is called once per frame
    void Update()
    {
        totalDist += Time.deltaTime * speed;
		RectTransform rt = GetComponent<RectTransform>();
		rt.anchoredPosition = new Vector2(startPos.x,startPos.y+totalDist);
		//transform.position.Set(startPos.x,startPos.y+totalDist,startPos.z);
		
		if (totalDist > maxdist) {
			transform.parent.gameObject.SetActive(false);
			if(mainMenu)
            {
				mainMenu.SetActive(true);
			} else
            {
				RestartGame();
			}
		}
	}
}
