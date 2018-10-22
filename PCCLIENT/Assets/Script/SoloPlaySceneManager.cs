using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class SoloPlaySceneManager : MonoBehaviour {

    public const char REDHAT = (char)1;
    public const char ALICE = (char)2;
    public const char SCROOGI = (char)2;

    public GameObject POPUP_Difficulty;
    public Image inbook;
    public GameObject POPUP_Skill;
    public GameObject POPUP_Skill_Manual;
    public GameObject ActiveSet;
    public GameObject img_Ready;

    public PlayerIcon[] PI = new PlayerIcon[4];
    CharacterSet CS;
    
    int difficulty;
    public byte choosebook;

    MobileNetwork mn;

    public void Start()
    {
        choosebook = 0;
        difficulty = 0;

        mn = GameObject.Find("Network Manager(Clone)").GetComponent<MobileNetwork>();
        GameObject.Find("Select Manager(Clone)").GetComponent<SelectManager>().not_show = true;
       
        CursorControl CC = GameObject.Find("Cursor Manager(Clone)").GetComponent<CursorControl>();
        CS = GameObject.Find("Selected Character Status(Clone)").GetComponent<CharacterSet>();

        for (int i = 0; i < 4; ++i)
        {
            PI[i].init();

            if (125 != CS.Ch[i].ch_type) PI[i].connected = true;
            PI[i].nickname = CC.nickname[i];
            CS.nickname[i] = CC.nickname[i];
            PI[i].level = CS.Ch[i].clearedround;
            PI[i].ch_type = CS.Ch[i].ch_type;

            PI[i].show();
        }
    }

    public void selectBook(byte booktype) {
        POPUP_Difficulty.SetActive(true);
        img_Ready.SetActive(true);
        ActiveSet.SetActive(false);
        
        inbook.sprite = Resources.Load<Sprite>("UI/Scene_stageselect_"+ booktype) as Sprite;
        choosebook = booktype;
    }

    public void cancleBook()
    {
        POPUP_Difficulty.SetActive(false);
        img_Ready.SetActive(false);
        ActiveSet.SetActive(true);
    }

    public void selectSkill()
    {
        GameObject.Find("GameOptionPrefab").GetComponent<Option>().difficulty = difficulty;
        mn.Get_Select_character();

        POPUP_Skill.GetComponent<PopUpSkillManager>().init(CS.Ch, CS.nickname);
        
        POPUP_Difficulty.SetActive(false);
        POPUP_Skill.SetActive(true);
    }

    public void cancleSkill()
    {
        POPUP_Difficulty.SetActive(true);
        POPUP_Skill.SetActive(false);
    }

    public void openManual()
    {
        POPUP_Skill_Manual.SetActive(true);
    }
    public void cancleManual()
    {
        POPUP_Skill_Manual.SetActive(false);
    }

    public void changeDifficulty(char dif) {
        difficulty = dif;
    }
    
}