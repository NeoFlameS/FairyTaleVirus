using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class SelectManager : MonoBehaviour {

    //UI 데이터
    
    CS_SKILLSET_PACKET sc;
    byte this_page;
    int cur;
    bool change_page = false;
    //UI 객체
    public Text[] sel_skilltext;
    public Text Page;
    public Text[] Skill_button;
    //고유 스킬 결정 시
    bool select_own = false;

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
            sc.sk_id[i] = 200;//default
        }
        cur = 0;
        this_page=0;
	}
	
	// Update is called once per frame
	void Update () {
        Update_Button();

    }
    
    public void Skill_btn1() {
        if (cur >= 4) { return; }//4개 선택 완료한 후에 선택 불가

        byte number = (byte)(0 + (this_page * 4));

        for (int i = 0; i < cur; i++) {//중복 선택 불가
            if (sc.sk_id[i] == number) { return; }
        }

        sc.sk_id[cur] = number;
        sel_skilltext[cur].text = ""+ (Convert.ToInt16(number) + 1) + "번 스킬 선택";
        cur++;
    }
    public void Skill_btn2()
    {
        if (cur >= 4) { return; }//4개 선택 완료한 후에 선택 불가

        byte number = (byte)(1 + (this_page * 4));

        for (int i = 0; i < cur; i++)//중복 선택 불가
        {
            if (sc.sk_id[i] == number) { return; }
        }

        sc.sk_id[cur] = number;
        sel_skilltext[cur].text = "" + (Convert.ToInt16(number) + 1) + "번 스킬 선택";
        cur++;
    }
    public void Skill_btn3()
    {
        if (cur >= 4) { return; }//4개 선택 완료한 후에 선택 불가
        if (this_page == 1 && select_own) { return; }// 고유 스킬이 이미 하나가 선택 되어있을때
        byte number = (byte)(2 + (this_page * 4));

        for (int i = 0; i < cur; i++)//중복 선택 불가
        {
            if (sc.sk_id[i] == number) { return; }
        }

        sc.sk_id[cur] = number;
        sel_skilltext[cur].text = "" + (Convert.ToInt16(number) + 1) + "번 스킬 선택";
        if (this_page == 1) { select_own = true; }
        
        cur++;
    }

    public void Skill_btn4()
    {
        if (cur >= 4) { return; }//4개 선택 완료한 후에 선택 불가
        if (this_page == 1 && select_own) { return; }// 고유 스킬이 이미 하나가 선택 되어있을때
        byte number = (byte)(3 + (this_page * 4));

        for (int i = 0; i < cur; i++)//중복 선택 불가
        {
            if (sc.sk_id[i] == number) { return; }
        }

        sc.sk_id[cur] = number;
        sel_skilltext[cur].text = "" + (Convert.ToInt16(number)+1)+ "번 스킬 선택";
        if (this_page == 1) { select_own = true; }
        cur++;
    }

    /// <summary>
    /// /아직 안지우고 남긴것들 이후 지울것
    /// </summary>
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
        byte type = GameObject.Find("NetWorkManager").GetComponent<NetWorkManager>().My_type;

        for (int i = 0; i< 4; i++) {
            if (sc.sk_id[i] == 200) { return; }//  다 선택 안됬을 때 안되게함
        }

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
        select_own = false;
    }

    public void Next_page() {
        if (this_page == 1) { this_page = 0; }
        else { this_page++; }
        Page.text = (this_page+1) + "/2 Page";
        change_page = true;
    }

    public void Previous_page()
    {
        if (this_page == 0) { this_page = 1; }
        else { this_page--; }
        Page.text = (this_page + 1) + "/2 Page";
        change_page = true;
    }
    void Update_Button() {
        if (change_page) {
            for (int i = 0; i < 4; i++) {
                Skill_button[i].text = "No." + (i + (this_page * 4) + 1) + "Skill";
            }
            change_page = false;
        }
        
    }


}
