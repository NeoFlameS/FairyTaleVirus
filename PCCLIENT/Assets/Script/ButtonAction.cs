using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonAction : MonoBehaviour {
    public GameObject SelectedCharacterStatus;
    SelectManager p_SM;

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
		case 2://Character select next button
                CursorControl CC = GameObject.Find("Cursor Manager(Clone)").GetComponent<CursorControl>();
                if (false == CC.characterallselected()) break;

                p_SM = GameObject.Find("Select Manager(Clone)").GetComponent<SelectManager>();
                p_SM.SelfSave();

                SceneManager.LoadScene(Scene);
                break; 
		case 3://character select back
                p_SM = GameObject.Find("Select Manager(Clone)").GetComponent<SelectManager>();
                p_SM.SelfSave();

                SceneManager.LoadScene(Scene);
                break;
            case 4:
			//character delete
		case 5:
			//select cancle
		default:
			break;
        }
    }
}
