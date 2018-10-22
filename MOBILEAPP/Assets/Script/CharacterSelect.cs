using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterSelect : MonoBehaviour {
    int type = 0;
    byte model = 0;

    public Text Type;
    public Text Model;
    byte id = 0;
    NetWorkManager nm;
    // Use this for initialization
    void Start () {

        nm = GameObject.Find("NetWorkManager").GetComponent<NetWorkManager>();

        switch (nm.My_Info.color)
        {
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
	void FixedUpdate () {
        Type.text = "" + (type+1);
        Model.text = "" + (model+1);
	}

    public void Type_Right() {
        type++;
        if (type > 3) {
            type = 0;
        }
    }
    public void Type_Left()
    {
        type--;
        if(type < 0){
            type = 3;
        }
    }
    public void Model_Right()
    {
        model = 1;
    }
    public void Model_Left()
    {
        model = 0;
    }
    public void Ok_Btn() {
       
        CS_SELECT_PACKET cs = new CS_SELECT_PACKET();

        cs.id = id;
        cs.model = model;
        cs.type = (byte)type;

        nm.GameDataSend(cs,NetworkController.SC_SELECT);//셀렉트 송신 
        
        SceneManager.LoadScene("Connected");
    }
}
