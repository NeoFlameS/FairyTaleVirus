using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ResultStatus : MonoBehaviour {
    public Text[] stat = new Text[6]; //닉네임, str,atk,int,vit
    Character ch;
    Upgrade_Log log;

    public void init(Character c, Upgrade_Log l, string name)
    {
        ch = c;
        log = l;

        stat[0].text = name;
        stat[1].text = "STR : "+ch.ch_str + "+" + log.ch_str;
        stat[2].text = "ATK : " + ch.ch_atk + "+" + log.ch_atk;
        stat[3].text = "INT : " + ch.ch_int + "+" + log.ch_int;
        stat[4].text = "VIT : " + ch.ch_vit + "+" + log.ch_vit;
        stat[5].text = "MIND : " + ch.ch_mid + "+" + log.ch_mid;//오후 3시 추가 
    }
}
