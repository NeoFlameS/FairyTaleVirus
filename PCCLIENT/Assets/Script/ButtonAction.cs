using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonAction : MonoBehaviour {

	public string Scene;
	public int ButtonType;

	public void clicked(){
		switch (ButtonType) {
		case 0:
			//NEWTWORK DATA SAVE
			//OPTION DATA SAVE
			//CURSOR DATA SAVE
			SceneManager.LoadScene (Scene);
			break;
		case 1:
			Application.Quit ();
			break;
		case 2:
			//character make
		case 3:
			//character select
		case 4:
			//character delete
		case 5:
			//select cancle
		default:
			break;
		}
	}

}
