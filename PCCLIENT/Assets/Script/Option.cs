using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class Option : MonoBehaviour {

	public static string nowScene;

	public static int soundvolume = 50;
	public static Vector2 screensize = new Vector2(1920, 1080);

	//public float timer;

	void OnLevelWasLoaded(){
        nowScene = EditorSceneManager.GetActiveScene().name;
	}

	void Start(){
		DontDestroyOnLoad (gameObject);
		//timer = 0;
	}

	// Update is called once per frame
	void Update () {
		/*timer += Time.deltaTime;
		if (timer >= 3) {
			SceneManager.LoadScene ("CharacterSelect");
			timer = -9999;
		}*/
	}
}
