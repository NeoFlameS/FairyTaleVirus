using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class SelectManager : MonoBehaviour {

    //UI 데이터
    
    CS_SKILLSET_PACKET sc;

    int cur;
    //UI 객체
    public Text[] sel_skilltext;
	// Use this for initialization
	void Start () {

        
        sc.sk_id = new byte[4];
        char color = GameObject.Find("NetWorkManager").GetComponent<NetWorkManager>().My_Info.color;
        switch (color) {
            case 'b':
                sc.id = 0;
                break;
            case 'y':
                sc.id = 1;
                break;
            case 'r':
                sc.id = 2;
                break;
            case 'g':
                sc.id = 3;
                break;
        }

        int i=0;
        for (i=0;i<4;i++) {
            sc.sk_id[i] = 125;
        }
        cur = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Skill_btn1() {
        if (cur >= 4) { return; }//4개 선택 완료한 후에 선택 불가
        sc.sk_id[cur] = 0;
        sel_skilltext[cur].text = "1번 스킬 선택";
        cur++;
        
    }
    public void Skill_btn2()
    {
        if (cur >= 4) { return; }
        sc.sk_id[cur] = 1;
        sel_skilltext[cur].text = "2번 스킬 선택";
        cur++;
    }
    public void Skill_btn3()
    {
        if (cur >= 4) { return; }
        sc.sk_id[cur] = 2;
        sel_skilltext[cur].text = "3번 스킬 선택";
        cur++;
    }
    public void Skill_btn4()
    {
        if (cur >= 4) { return; }
        sc.sk_id[cur] = 3;
        sel_skilltext[cur].text = "4번 스킬 선택";
        cur++;
    }
    public void Skill_btn5()
    {
        if (cur >= 4) { return; }
        sc.sk_id[cur] = 4;
        sel_skilltext[cur].text = "5번 스킬 선택";
        cur++;
    }
    public void Skill_btn6()
    {
        if (cur >= 4) { return; }
        sc.sk_id[cur] = 5;
        sel_skilltext[cur].text = "6번 스킬 선택";
        cur++;
    }
    public void Select_completebtn()
    {
        GameObject.Find("NetWorkManager").GetComponent<NetWorkManager>().GameDataSend(sc,NetworkController.CS_SKILL);
        Debug.Log((short)sc.sk_id[0]+ (short)sc.sk_id[1]+ (short)sc.sk_id[2]+ (short)sc.sk_id[3]);
        SceneManager.LoadScene("Connected");
    }
    public void Reselect_btn()
    {
        cur = 0;
        int i = 0;
        for (i = 0; i < 4; i++) {
            sel_skilltext[i].text = "New Select";
        }
        
    }


}
