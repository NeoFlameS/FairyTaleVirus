using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonAction : MonoBehaviour {
    public string SceneorPopup;
	public int ButtonType;

	public void clicked(){
        switch (ButtonType)
        {
            case 0:
                //NEWTWORK DATA SAVE
                //OPTION DATA SAVE
                //CURSOR DATA SAVE
                SceneManager.LoadScene(SceneorPopup);
                break;
            case 1:
                Application.Quit();
                break;
            case 2://Character select next button
                {
                    CursorControl CC = GameObject.Find("Cursor Manager(Clone)").GetComponent<CursorControl>();
                    if (false == CC.characterallselected()) break;

                    SelectManager p_SM = GameObject.Find("Select Manager(Clone)").GetComponent<SelectManager>();
                    p_SM.SelfSave();

                    SceneManager.LoadScene(SceneorPopup);
                    break;
                }
            case 3://character select back
                {
                    SelectManager p_SM = GameObject.Find("Select Manager(Clone)").GetComponent<SelectManager>();
                    p_SM.SelfSave();

                    SceneManager.LoadScene(SceneorPopup);
                    break;
                }
            case 4:
                //option popup
                GameObject.Find("OPTION POPUP").active = true;
                GameObject.Find("Button").active = false;
                break;
            case 5:
                //option close
                GameObject.Find("OPTION POPUP").active = false;
                GameObject.Find("Button").active = true;
                break;
            case 6:
                //Difficulty
                GameObject.Find("SoloPlay Scene Manager").GetComponent<SoloPlaySceneManager>().selectBook(SceneorPopup[0]);
                break;
            case 7:
                //difficulty back
                GameObject.Find("SoloPlay Scene Manager").GetComponent<SoloPlaySceneManager>().cancleBook();
                break;
            case 8:
                //skill
                GameObject.Find("SoloPlay Scene Manager").GetComponent<SoloPlaySceneManager>().selectSkill();
                break;
            case 9:
                //skill back
                GameObject.Find("SoloPlay Scene Manager").GetComponent<SoloPlaySceneManager>().cancleSkill();
                break;
            default:
                break;
        }
    }
}
