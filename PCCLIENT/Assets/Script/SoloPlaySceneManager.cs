using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoloPlaySceneManager : MonoBehaviour {

    public const char REDHAT = (char)1;
    public const char ALICE = (char)2;

    public GameObject POPUP_Difficulty;
    public GameObject POPUP_Skill;
    public GameObject ActiveSet;
    public GameObject img_Ready;

    public SetSkillSelect[] pskillselect = new SetSkillSelect[4];

    public void Start()
    {
        CharacterSet pCS = GameObject.Find("Selected Character Status(Clone)").GetComponent<CharacterSet>();
        for (int i = 0; i < 4; ++i) {
            pskillselect[i].init(pCS.Ch[i], pCS.nickname[i]);
        }
    }

    public void selectBook(char booktype) {
        switch (booktype) {
            case REDHAT:
                //책 내용 이미지 불러오기
                break;
            case ALICE:
                break;
            default:
                //error!!!
                break;
        }

        POPUP_Difficulty.active = true;
        img_Ready.active = true;
        ActiveSet.active = false;
    }

    public void cancleBook()
    {
        POPUP_Difficulty.active = false;
        img_Ready.active = false;
        ActiveSet.active = true;
    }

    public void selectSkill()
    {
        POPUP_Difficulty.active = false;
        POPUP_Skill.active = true;
    }

    public void cancleSkill()
    {
        POPUP_Difficulty.active = true;
        POPUP_Skill.active = false;
    }
    

}