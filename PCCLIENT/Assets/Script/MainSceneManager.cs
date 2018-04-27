using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneManager : MonoBehaviour {

    public PlayerIcon[] PI = new PlayerIcon[4];
    public GameObject CharacterInfoSet;
    Character[] ch;

    public void Start()
    {
        CursorControl CC = GameObject.Find("Cursor Manager(Clone)").GetComponent<CursorControl>();
        byte[] id = CC.selectedID();
        int count = 0;

        for (int i = 0; i < 4; ++i) {
            if (125 != id[i]) count++;
        }

        SelectManager SM = GameObject.Find("Select Manager(Clone)").GetComponent<SelectManager>();
        ch = SM.LoadToScene(id, count);

        GameObject tmp = GameObject.Find("Selected Character Status(Clone)");
        if (null == tmp) tmp = Instantiate(CharacterInfoSet);

        CharacterSet CS = tmp.GetComponent<CharacterSet>();
        CS.init(ch);

        for (int i = 0; i < 4; ++i) {
            PI[i].init();

            if (125 != ch[i].ch_type) PI[i].connected = true;
            PI[i].nickname = CC.nickname[i];
            PI[i].level = ch[i].clearedround;
            PI[i].ch_type = ch[i].ch_type;

            PI[i].show();
        }
    }
}
