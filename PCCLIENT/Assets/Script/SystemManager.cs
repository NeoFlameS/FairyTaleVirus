using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SystemManager : MonoBehaviour {
    //FOR ONLY LOGO SCENE
	public float timer;

	void Start () {
        timer = 0;
	}
	
	void Update () {
        timer += Time.deltaTime;
		if (timer >= 3) {
			SceneManager.LoadScene ("Scene_Connecting");
		}
	}
}
