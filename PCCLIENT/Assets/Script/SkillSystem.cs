using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System;

public class SkillSystem : MonoBehaviour
{
 
     public const byte E_TARGET_DMG = 1;
     public const byte E_AREA_DMG = 2;
     public const byte E_TARGET_SLOW = 3;
     public const byte E_AREA_SLOW = 4;
     public const byte E_SELF_ATK_FAST = 5;
     public const byte E_SELF_ATK_POWERUP = 6;
 
     public const byte STATUS_STR = 1;
     public const byte STATUS_VIT = 2;
     public const byte STATUS_INT = 3;
     public const byte STATUS_MIND = 4;
     public const byte STATUS_NONE = 5;
 
     public const byte BUF_ATK_UP = 1;
     public const byte BUF_ATKSPD_UP = 2;
 
 
     float timer;
     skill[][] sk = new skill[4][];
 
     public void init(int[][] skillset)
     {
         for (int i = 0; i< 4; ++i)
         {
             sk[i] = new skill[4];
             for (int j = 0; j< 4; ++j)
             {
                 sk[i][j].sk_type = skillset[i][j];
             }
         }
 
         try
         {
             XmlDocument doc = new XmlDocument();
             doc.Load(Application.persistentDataPath + "/SkillData.xml");
             XmlElement root = doc.DocumentElement;
 
             XmlNodeList nodes = root.ChildNodes;
 
             for (int i = 0; i< 4; ++i) {
                 for (int j = 0; j< 4; ++j)
                 {
                     XmlNode node = root[sk[i][j].sk_type.ToString()];
                     sk[i][j].sk_cooltime = float.Parse(node["Cooltime"].InnerText);
                     sk[i][j].sk_canusetime = sk[i][j].sk_cooltime;
                     sk[i][j].sk_effecttime = float.Parse(node["EffectTime"].InnerText);
                     sk[i][j].sk_effect = byte.Parse(node["Effect"].InnerText);
                     sk[i][j].sk_basedmg = byte.Parse(node["BaseDmg"].InnerText);
                     sk[i][j].sk_statusmultipledmg = float.Parse(node["StatusMultiplyDmg"].InnerText);
                     sk[i][j].sk_basedstatus = byte.Parse(node["StatusMultiply"].InnerText);
                     //파티클 시스템 필요~
                 }
             }
         }
         catch (Exception e)
         {
             Debug.Log(e);
             //error;
         }
     }
 
     public bool use_skill(PlayerCharacter player, Monster target, byte characternumber, int keynumber)
     {
         //버튼을 누름
         //스킬사용 전달 >
         //스킬 사용 ! 명령
         //쿨타임 체크
         //실패시 ? => 실패 이펙트 표기?
         if (timer > sk[characternumber][keynumber].sk_canusetime)
         {
             return false;
         }
 
         int e_value = sk[characternumber][keynumber].sk_basedmg;
 
         switch (sk[characternumber][keynumber].sk_basedstatus) {
             case STATUS_STR:
                 e_value += (int)(player.ch.ch_str* sk[characternumber][keynumber].sk_statusmultipledmg);
                 break;
             case STATUS_VIT:
                 e_value += (int)(player.ch.ch_vit* sk[characternumber][keynumber].sk_statusmultipledmg);
                 break;
             case STATUS_INT:
                 e_value += (int)(player.ch.ch_int* sk[characternumber][keynumber].sk_statusmultipledmg);
                 break;
             case STATUS_MIND:
                 e_value += (int)(player.ch.ch_mid* sk[characternumber][keynumber].sk_statusmultipledmg);
                 break;
             case STATUS_NONE:
                 break;
         }
 
         //성공시 스킬 타겟에 사용 ( 캐릭터의 타겟 or 주변 광범위 등 )
         //스킬 이펙트 뿜뿜
         switch (sk[characternumber][keynumber].sk_effect) {
             case E_TARGET_DMG:
                 target.Damaged(e_value);
                 //use instatiate effect at target.position ~
                 break;
             case E_AREA_DMG:
                 {
                     List<Monster> ml = new List<Monster>();
                     //에어리어 내 몬스터들 검색
                     foreach (Monster m in ml)
                     {
                         target.Damaged(e_value);
                         //use instatiate effect at target.position ~
                     }
                     break;
                 }
             case E_TARGET_SLOW:
                 target.Slow(e_value);
                 //use instatiate effect at target.position ~
                 break;
             case E_AREA_SLOW:
                 {
                     List<Monster> ml = new List<Monster>();
                     //에어리어 내 몬스터들 검색
                     foreach (Monster m in ml)
                     {
                         target.Slow(e_value);
                         //use instatiate effect at target.position ~
                     }
                     break;
                 }
             case E_SELF_ATK_FAST:
                 player.UseBuff(BUF_ATKSPD_UP, e_value, (int)sk[characternumber][keynumber].sk_effecttime);
                 break;
             case E_SELF_ATK_POWERUP:
                 player.UseBuff(BUF_ATK_UP, e_value, (int)sk[characternumber][keynumber].sk_effecttime);
                 break;
         }
 
         //쿨타임 돌리고~
         sk[characternumber][keynumber].sk_canusetime = timer + sk[characternumber][keynumber].sk_cooltime;
         return true;
     }
         // Use this for initialization
     void Start()
    {
        timer = 0;
    }

    // Update is called once per frame
    void Update () {
        timer += Time.deltaTime;
	}
}

struct skill
{     public int sk_type;
     public float sk_cooltime;
     public float sk_canusetime;
     public float sk_effecttime;
     public int sk_basedmg;
     public byte sk_basedstatus;
     public float sk_statusmultipledmg;
     public byte sk_effect;
     public Particle sk_particle;
}