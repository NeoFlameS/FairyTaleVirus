using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultStatusSet : MonoBehaviour {
    
    public PlayerIcon[] pi = new PlayerIcon[4];
    public ResultStatus[] rs = new ResultStatus[4];
    public GameObject[] st = new GameObject[4]; 
    CharacterSet Chs;
    
	// Use this for initialization
	void Start () {
        

        
        //cursor_main.GetComponent<CursorControl>().Init();

        Chs = GameObject.Find("Selected Character Status(Clone)").GetComponent<CharacterSet>();
        
        int player = GameObject.Find("Network Manager(Clone)").GetComponent<MobileNetwork>().Return_PlayerCount();
        for (int i = 0; i < 4; ++i) {
            pi[i].init();
            pi[i].show();
        }
        for (int i = 0; i < player; ++i) {
            
            st[i].SetActive(true);
            rs[i].init(Chs.Ch[i], Chs.log[i],Chs.nickname[i]);

            pi[i].nickname = Chs.nickname[i];
            pi[i].ch_type = Chs.Ch[i].ch_type;
            pi[i].level = Chs.Ch[i].clearedround;
            pi[i].connected = true;
            pi[i].show();


            
            GameObject.Find("Network Manager(Clone)").GetComponent<MobileNetwork>().SignalSend(i,NetworkController.SC_GAME_RESULT);
        }

        if (GameObject.Find("StageResult").GetComponent<StageResult>().cor_ret() < 20) {
            Chs.All_data_save();
        }
	}

}
