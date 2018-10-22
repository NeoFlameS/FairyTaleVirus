using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PopUpSkillManager : MonoBehaviour {

    public SetSkillSelect[] sk = new SetSkillSelect[4];
    CharacterSet CS;
    MobileNetwork mn;

    
    // Use this for initialization
    void Start () {
		CS=GameObject.Find("Selected Character Status(Clone)").GetComponent<CharacterSet>();
        SC_TYPE_PACKET sc = new SC_TYPE_PACKET();

        mn = GameObject.Find("Network Manager(Clone)").GetComponent<MobileNetwork>();

        for (int i = 0; i < mn.Return_PlayerCount(); i++)
        {
            mn.SignalSend(i, NetworkController.CS_SKILL);
            sc.type = CS.Ch[i].ch_type;
            //mn.GameDataSend(i, sc, NetworkController.CS_SKILL);//type send 6.16일 추가
            Debug.Log("Send " + i + " Player Type : " + Convert.ToString(CS.Ch[i].ch_type));
        }
    }
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < 4; ++i) {
            if (CS.skillcheck[i]) {
                selected_skill(i,CS.Ch[i]);
                CS.skillcheck[i] = false;
            }
        }
	}

    public void init(Character[] c, string[] nick)
    {
        for (int i = 0; i < 4; ++i) {
            sk[i].init(c[i], nick[i]);
        }
    }

    public void selected_skill(int id,Character s) {
        sk[id].setskills(s.skill);
    }
}
