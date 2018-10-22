using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class UpgradeManager : MonoBehaviour {

    NetWorkManager nm;
    byte id;
	// Use this for initialization
	void Start () {
        nm = GameObject.Find("NetWorkManager").GetComponent<NetWorkManager>();
        switch (nm.My_Info.color) {
            case 'b':
                id = 0;
                break;
            case 'y':
                id = 1;
                break;
            case 'r':
                id = 2;
                break;
            case 'g':
                id = 3;
                break;
            default:
                id = 125;
                break;
        }
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    public void Str_btn() {
        CS_UPGRADE_PACKET up = new CS_UPGRADE_PACKET();
        up.id = id;
        up.up_sg = 0;
        nm.GameDataSend(up,NetworkController.CS_UPGRADE);
    }
    public void Atk_btn() {
        CS_UPGRADE_PACKET up = new CS_UPGRADE_PACKET();
        up.id = id;
        up.up_sg = 1;
        nm.GameDataSend(up, NetworkController.CS_UPGRADE);
    }
    public void Vit_btn() {
        CS_UPGRADE_PACKET up = new CS_UPGRADE_PACKET();
        up.id = id;
        up.up_sg = 2;
        nm.GameDataSend(up, NetworkController.CS_UPGRADE);
    }
    public void Int_btn() {
        CS_UPGRADE_PACKET up = new CS_UPGRADE_PACKET();
        up.id = id;
        up.up_sg = 3;
        nm.GameDataSend(up, NetworkController.CS_UPGRADE);
    }
    public void Back_btn() {
        SceneManager.LoadScene("Connected");
    }
}
