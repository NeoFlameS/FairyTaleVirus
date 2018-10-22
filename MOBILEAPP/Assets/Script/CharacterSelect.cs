using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterSelect : MonoBehaviour {
    int type = 0;
    byte model = 0;

    public GameObject model1, model2;
    public GameObject[] Type_text = new GameObject[4];

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
        Type.text = "" + (type + 1) + " 빨간망토";
        Model.text = "" + (model + 1) + " 빨간망토";
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        
	}

    public void TypeChange() {
        switch (type)
        {
            case 0:
                Type.text = "" + (type + 1) + " 빨간망토";
                break;
            case 1:
                Type.text = "" + (type + 1) + " 사서";
                break;
            case 2:
                Type.text = "" + (type + 1) + " 앨리스";
                break;
            case 3:
                Type.text = "" + (type + 1) + " 스크루지";
                break;
            default:
                break;
        }

        for (int i = 0; i < 4; ++i) {
            if (i == type)
            {
                Type_text[i].SetActive(true);
            }
            else {
                Type_text[i].SetActive(false);
            }
        }
    }

    public void ModelChange() {
        switch (model)
        {
            case 0:
                Model.text= "" + (model + 1)+" 빨간망토";
                model1.SetActive(true);
                model2.SetActive(false);
                break;
            case 1:
                Model.text = "" + (model + 1) + " 사서";
                model1.SetActive(false);
                model2.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void Type_Right() {
        type++;
        if (type > 3) {
            type = 0;
        }
        TypeChange();
    }

    public void Type_Left()
    {
        type--;
        if(type < 0){
            type = 3;
        }
        TypeChange();
    }

    public void Model_Right()
    {
        model = 1;
        ModelChange();
    }

    public void Model_Left()
    {
        model = 0;
        ModelChange();
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
