using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Option : MonoBehaviour {

	public static int nowScene;

	public static int soundvolume = 50;
	public static Vector2 screensize = new Vector2(1920, 1080);

    //public float timer;
    
    private Vector3 touchedPos;
    
	void OnLevelWasLoaded(){
        nowScene = SceneManager.GetActiveScene().buildIndex;
	}

	public void Start(){
		DontDestroyOnLoad (gameObject);
        //timer = 0;
    }

	// Update is called once per frame
	public void Update () {
        /*timer += Time.deltaTime;
		if (timer >= 3) {
			SceneManager.LoadScene ("CharacterSelect");
			timer = -9999;
		}*/

        if (Input.GetMouseButtonDown(0))
        {
            GameObject.Find("Cursor Manager(Clone)").GetComponent<CursorControl>().click(0, 1);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            GameObject.Find("Cursor Manager(Clone)").GetComponent<CursorControl>().click(0, 3);
        }
        else if (Input.GetMouseButtonDown(2))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, Mathf.Infinity);

            if (hit.collider.tag == "BUTTON") {
                hit.collider.GetComponent<ButtonAction>().clicked();
            }
        }
    }
}
