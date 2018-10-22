using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerIcon : MonoBehaviour {

    public string nickname;
    public int level;
    public byte ch_type;

    public Text text_level;
    public Text text_nickname;
    public Image img_character;
    public Image img_border;

    public bool connected;


    public void Start()
    {
        init();
    }

    public void init()
    {
        nickname = "NONE";
        level = 0;
        ch_type = 125;
        connected = false;
    }

    public void show()
    {
        if (false == connected)
        {
            text_level.text = level.ToString();
            text_nickname.text = nickname;
            
            img_character.sprite = Resources.Load<Sprite>("UI/ui_character_default") as Sprite;
            img_border.sprite = Resources.Load<Sprite>("UI/ui_Character_icon2") as Sprite;
            return;
        }

        string filename;
        text_level.text = level.ToString();
        text_nickname.text = nickname;
        
        filename = "UI/ui_character_" + ch_type;
        img_character.sprite = Resources.Load<Sprite>(filename) as Sprite;
    }
}
