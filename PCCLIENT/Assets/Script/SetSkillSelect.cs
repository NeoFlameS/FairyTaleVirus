using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetSkillSelect : MonoBehaviour {
    public string[] ch_name = { "빨간망토", "사서", "앨리스" };

    public Image img_character;
    public Image[] img_skill = new Image[4];
    public Text nickname;
    public Text character;
    public Text grade;
    public Text[] status = new Text[4];

    public void init(Character c, string nick) {
        if (125 == c.ch_type)
        {
            //기본설정 이미지같은거 가져와~
            character.text = "";
            img_character.sprite = Resources.Load<Sprite>("UI/ui_character_default") as Sprite;
            for (int i = 0; i < 4; ++i)
            {
                img_skill[i].sprite = Resources.Load<Sprite>("UI/ui_character_default") as Sprite;
                status[i].text = "";
            }
            nickname.text = nick;
            grade.text = "";
            return;
        }

        character.text = ch_name[c.ch_type];
        img_character.sprite = Resources.Load<Sprite>("UI/ui_characterbox_" + c.ch_type) as Sprite;
        for (int i = 0; i < 4; ++i){
            img_skill[i].sprite = Resources.Load<Sprite>("UI/ui_skillbox_" + c.skill[i]) as Sprite;
        }
        nickname.text = nick;
        grade.text = c.clearedround.ToString();
        status[0].text = c.ch_str.ToString();
        status[1].text = c.ch_vit.ToString();
        status[2].text = c.ch_int.ToString();
        status[3].text = c.ch_mid.ToString();
    }

    public void setskills(char[] skillset) {
        for (int i = 0; i < 4; ++i) {
            img_skill[i].sprite = Resources.Load<Sprite>("UI/ui_skill_img_" + skillset[i]) as Sprite;
        }
    }
}
