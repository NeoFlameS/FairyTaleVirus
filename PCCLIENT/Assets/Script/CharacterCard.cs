using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCard : MonoBehaviour {
    public string nickname;
    public int clearedround;
    public int ch_type;
    public int[] skillset = new int[4] { -1, -1, -1, -1};
    public byte CCid;
    bool g_clicked = false;

    public Image selectedcolor;
    public Text text_clearedround;
    public Text text_nickname;
    public Image image_character;
    public Image[] image_skill;

    public void init() {
        selectedcolor.color = Color.clear;
        clearedround = -1;
        ch_type = 125;
    }

    public void show() {
        if (ch_type == 125) {
            //TEMP set
            text_nickname.text = "TEMP SLOT";
            text_clearedround.text = "LEVEL : 0";
            image_character.sprite = Resources.Load<Sprite>("UI/ui_character_default") as Sprite;
            for (int i = 0; i < 4; ++i) image_skill[i].sprite = Resources.Load<Sprite>("UI/ui_skill_default") as Sprite;
            return;
        }
        //unity text render change
        text_nickname.text = nickname;
        text_clearedround.text = "LEVEL : " + clearedround.ToString();

        //unity image change
        string filename = "UI/ui_character_" + ch_type;
        image_character.sprite = Resources.Load<Sprite>(filename) as Sprite;
        for (int i = 0; i < 4; ++i){
            filename = "UI/ui_skill_" + skillset[i];
            image_skill[i].sprite = Resources.Load<Sprite>(filename) as Sprite;
        }
    }

    public bool clicked(int c) {
        if (true == g_clicked) return false;
        g_clicked = true;
        switch (c)
        {
            case 0:
                selectedcolor.color = Color.blue;
                break;
            case 1:
                selectedcolor.color = Color.yellow;
                break;
            case 2:
                selectedcolor.color = Color.red;
                break;
            case 3:
                selectedcolor.color = Color.green;
                break;
            default:
                //error
                break;
        }
        return true;
    }

    public void clickcancled() {
        if (false == g_clicked) return;
        selectedcolor.color = Color.clear;
        g_clicked = false;
    }

}