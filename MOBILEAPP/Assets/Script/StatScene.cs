using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;
public class StatScene : MonoBehaviour {
    // 씬 오브젝트 관련
    public StatusBox[] stat=new StatusBox[4];
    bool recv = false;
    // 네트워크 통신 관련
    NetWorkManager nm;


    //게임 데이터 관련
    SC_CHARACTERINFOSET_PACKET scp;
    // Use this for initialization
    void Start () {
        scp = new SC_CHARACTERINFOSET_PACKET();
        scp.characterinfo = new SC_CHARACTERINFO_PACKET[4];

        stat[0] = GameObject.Find("StatusBox1").GetComponent<StatusBox>();
        stat[1] = GameObject.Find("StatusBox2").GetComponent<StatusBox>();
        stat[2] = GameObject.Find("StatusBox3").GetComponent<StatusBox>();
        stat[3] = GameObject.Find("StatusBox4").GetComponent<StatusBox>();
        
        nm = GameObject.Find("NetWorkManager").GetComponent<NetWorkManager>();
        nm.SendRequestSignal(NetworkController.CS_REQCHR);
    }
	
	// Update is called once per frame
	void Update () {
        int i = 0;
        if (nm.recv_charinfo) {
            //리시브 된 데이터가 있을 때 데이터 업데이트
            scp = nm.scinf;
            for (i = 0; i < 4; i++)
            {
                stat[i].MSTR.text = "" + (short)scp.characterinfo[i].ch_str;
                stat[i].MSPD.text = "" + scp.characterinfo[i].ch_movespd;
                stat[i].MINT.text = "" + (short)scp.characterinfo[i].ch_int;
                stat[i].MVIT.text = "" + (short)scp.characterinfo[i].ch_vit;
                stat[i].MATK.text = "" + (short)scp.characterinfo[i].ch_atk;
            }
        }
	}

    public void BackBtn() {
        SceneManager.LoadScene("Connected");
    }
}
