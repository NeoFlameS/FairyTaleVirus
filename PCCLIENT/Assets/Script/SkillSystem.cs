using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System;

public class SkillSystem : MonoBehaviour
{

    public const byte E_DMG = 1;
    public const byte E_Status_Down = 2;
    public const byte E_Status_Up = 3;
    public const byte E_Make = 4;
    public const byte E_Move = 5;
    public const byte E_DiceKill = 6;

    public const byte STATUS_STR = 1;
    public const byte STATUS_VIT = 2;
    public const byte STATUS_INT = 3;
    public const byte STATUS_MIND = 4;
    public const byte STATUS_2_SV = 5;
    public const byte STATUS_2_SI = 6;
    public const byte STATUS_2_SM = 7;
    public const byte STATUS_2_VI = 8;
    public const byte STATUS_2_VM = 9;
    public const byte STATUS_2_IM = 10;
    public const byte STATUS_3_SVI = 11;
    public const byte STATUS_3_SVM = 12;
    public const byte STATUS_3_SIM = 13;
    public const byte STATUS_3_VIM = 14;
    public const byte STATUS_4 = 15;
    public const byte STATUS_NONE = 16;

    public const byte BUF_ATK_UP = 1;
    public const byte BUF_ATKSPD_UP = 2;

    public const byte TS_AtkSPD = 1;
    public const byte TS_MoveSPD = 2;
    public const byte TS_AtkDMG = 3;
    public const byte TS_None = 4;
    public const byte TS_Stamina = 5;

    public const int EF_BLUESPELL = 0;
    public const int EF_INFERNO = 1;
    public const int EF_ICE = 2;
    public const int EF_BETERFLY = 3;
    public const int EF_VENNOM = 4;

    public Transform[] TP_Point;
    public Transform[] MON_TP_Point;

    float timer;
    public skill[][] sk;
    PlayerCharacter[] g_PC;


    public ParticleSystem[] Eft = new ParticleSystem[5];

    public void init(int[][] skillset, PlayerCharacter[] PC)
     {
        sk = new skill[4][];
         for (int i = 0; i< 4; ++i)
         {
             sk[i] = new skill[4];
             for (int j = 0; j< 4; ++j)
             {
                sk[i][j] = new skill();
                if (skillset[i] == null) continue;//5.31 홍승준 추가
                sk[i][j].sk_type = skillset[i][j];
             }
         }

        g_PC = PC;
         try
         {
             XmlDocument doc = new XmlDocument();
             doc.Load(Application.persistentDataPath + "/SkillData.xml");
             XmlElement root = doc.DocumentElement;
 
             XmlNodeList nodes = root.ChildNodes;

            for (int i = 0; i < 4; ++i)
            {
                for (int j = 0; j < 4; ++j)
                {
                    XmlNode node = null;
                    foreach (XmlNode nd in nodes)
                    {
                        if (sk[i][j].sk_type.ToString() == nd.Attributes["type"].Value as string)
                        {
                            node = nd;
                            break;
                        }
                    }
                    sk[i][j].sk_name = node.Attributes["Name"].Value as string;
                    sk[i][j].sk_cooltime = float.Parse(node["Cooltime"].InnerText);
                    sk[i][j].sk_canusetime = sk[i][j].sk_cooltime;
                    sk[i][j].sk_effecttime = float.Parse(node["EffectTime"].InnerText);
                    sk[i][j].sk_effect = byte.Parse(node["Effect"].InnerText);
                    sk[i][j].sk_basedmg = byte.Parse(node["BaseDmg"].InnerText);
                    for (int k = 1; k < 5; ++k)
                    {
                        sk[i][j].sk_statusmultipledmg = float.Parse(node["StatusMultiplyDmg" + k].InnerText);
                    }
                    sk[i][j].sk_basedstatus = byte.Parse(node["StatusMultiply"].InnerText);
                    sk[i][j].sk_range = byte.Parse(node["Range"].InnerText);
                    sk[i][j].sk_targetStatus = byte.Parse(node["TargetStatus"].InnerText);
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
 
     public bool use_skill(PlayerCharacter player, Monster target, byte characternumber, int keynumber, List<Monster> mm)
     {
        //버튼을 누름
        //스킬사용 전달 >
        //스킬 사용 ! 명령
        //쿨타임 체크
        //실패시 ? => 실패 이펙트 표기?
        Debug.Log("SKILL BUTTON CLICKED @@@@@@@");
         if (timer < sk[characternumber][keynumber].sk_canusetime)
         {
            Debug.Log("Timer : " + timer +" #################");
            Debug.Log("CoolTime : " + sk[characternumber][keynumber].sk_canusetime + " #################");
            return false;
         }

        skill S = sk[characternumber][keynumber];
         int e_value = S.sk_basedmg;

        switch (S.sk_basedstatus)
        {
            case STATUS_STR:
                e_value += (int)(player.ch.ch_str * S.sk_statusmultipledmg);
                break;
            case STATUS_VIT:
                e_value += (int)(player.ch.ch_vit * S.sk_statusmultipledmg);
                break;
            case STATUS_INT:
                e_value += (int)(player.ch.ch_int * S.sk_statusmultipledmg);
                break;
            case STATUS_MIND:
                e_value += (int)(player.ch.ch_mid * S.sk_statusmultipledmg);
                break;
            case STATUS_2_IM:
                e_value += (int)((player.ch.ch_mid + player.ch.ch_int) * S.sk_statusmultipledmg);
                break;
            case STATUS_2_SI:
                e_value += (int)((player.ch.ch_str + player.ch.ch_int) * S.sk_statusmultipledmg);
                break;
            case STATUS_2_SV:
                e_value += (int)((player.ch.ch_str + player.ch.ch_vit) * S.sk_statusmultipledmg);
                break;
            case STATUS_2_VI:
                e_value += (int)((player.ch.ch_vit + player.ch.ch_int) * S.sk_statusmultipledmg);
                break;
            case STATUS_2_VM:
                e_value += (int)((player.ch.ch_vit + player.ch.ch_mid) * S.sk_statusmultipledmg);
                break;
            case STATUS_2_SM:
                e_value += (int)((player.ch.ch_str + player.ch.ch_mid) * S.sk_statusmultipledmg);
                break;
            case STATUS_3_SIM:
                e_value += (int)((player.ch.ch_str + player.ch.ch_int + player.ch.ch_mid) * S.sk_statusmultipledmg);
                break;
            case STATUS_3_SVI:
                e_value += (int)((player.ch.ch_str + player.ch.ch_int + player.ch.ch_vit) * S.sk_statusmultipledmg);
                break;
            case STATUS_3_SVM:
                e_value += (int)((player.ch.ch_str + player.ch.ch_vit + player.ch.ch_mid) * S.sk_statusmultipledmg);
                break;
            case STATUS_3_VIM:
                e_value += (int)((player.ch.ch_vit + player.ch.ch_int + player.ch.ch_mid) * S.sk_statusmultipledmg);
                break;
            case STATUS_4:
                e_value += (int)((player.ch.ch_str + player.ch.ch_int + player.ch.ch_mid + player.ch.ch_vit) * S.sk_statusmultipledmg);
                break;
            case STATUS_NONE:
                break;
        }
        Debug.Log("SKILL STATUS CHECK @@@@@@@");

        //성공시 스킬 타겟에 사용 ( 캐릭터의 타겟 or 주변 광범위 등 )
        //스킬 이펙트 뿜뿜
        switch (S.sk_effect) {
            case E_DMG:
                {
                    if (null == target) return false;
                    else Debug.Log("HELLO");
                    //단일판정
                    if (0 == S.sk_range) {
                        //Aura ( single )
                        Instantiate(Eft[EF_BLUESPELL], player.transform.position, Quaternion.identity);
                        if (true == target.Damaged(e_value))
                        {
                            if (target.target_count <= 1)
                            {
                                Destroy(target.gameObject);
                                player.ch.target = null;
                                mm.Remove(target);
                            }
                            else
                            {
                                target.target_count -= 1;
                                player.ch.target = null;
                            }
                        }
                        //use instatiate effect at target.position ~
                    }
                    else
                    {
                        List<Monster> ml = new List<Monster>();
                        //에어리어 내 몬스터들 검색
                        Instantiate(Eft[EF_INFERNO], player.transform.position, Quaternion.identity);
                        for (int i = 0; i < mm.Count; ++i)
                        {
                            if (Vector3.Distance(player.transform.position, mm[i].transform.position) <= S.sk_range) ml.Add(mm[i]);
                        }

                        foreach (Monster m in ml)
                        {
                            if (true == m.Damaged(e_value))
                            {
                                if (m.target_count <= 1)
                                {
                                    Destroy(m.gameObject);
                                    player.ch.target = null;
                                    mm.Remove(m);
                                }
                                else
                                {
                                    m.target_count -= 1;
                                    player.ch.target = null;
                                }
                            }
                            //use instatiate effect at target.position ~
                        }
                    }

                    break;
                }
             case E_Status_Down:
                 {
                    if (null == target) return false;
                    else Debug.Log("HELLO");
                    Instantiate(Eft[EF_ICE], player.transform.position, Quaternion.identity);
                    if (2 == S.sk_targetStatus) {
                        if (0 == S.sk_range)
                        {
                            target.Slow(e_value);

                        }
                        else {
                            List<Monster> ml = new List<Monster>();
                            //에어리어 내 몬스터들 검색
                            for (int i = 0; i < mm.Count; ++i)
                            {
                                if (Vector3.Distance(player.transform.position, mm[i].transform.position) <= S.sk_range) ml.Add(mm[i]);
                            }

                            foreach (Monster m in ml)
                            {
                                m.Slow(e_value);
                                //use instatiate effect at target.position ~
                            }
                        }
                    }
                    break;
                 }
             case E_Status_Up:
                {
                    Instantiate(Eft[EF_BETERFLY], player.transform.position, Quaternion.identity);
                    switch (S.sk_targetStatus) {
                        case TS_AtkDMG:
                            if (0 == S.sk_range)
                            {
                                player.UseBuff(BUF_ATK_UP, e_value, (int)S.sk_effecttime);
                            }
                            else {
                                for (int i = 0; i < 4; ++ i) {
                                    if (g_PC[i] == player) continue;
                                    if (g_PC[i] == null) continue;
                                    if (S.sk_range >= Vector3.Distance(g_PC[i].transform.position, player.transform.position))
                                        g_PC[i].UseBuff(BUF_ATK_UP, e_value, (int)S.sk_effecttime);
                                }
                            }
                            break;
                        case TS_AtkSPD:
                            if (0 == S.sk_range)
                            {
                                player.UseBuff(BUF_ATKSPD_UP, e_value, (int)S.sk_effecttime);
                            }
                            else
                            {
                                for (int i = 0; i < 4; ++i)
                                {
                                    if (g_PC[i] == player) continue;
                                    if (g_PC[i] == null) continue;
                                    if (S.sk_range >= Vector3.Distance(g_PC[i].transform.position, player.transform.position))
                                        g_PC[i].UseBuff(BUF_ATKSPD_UP, e_value, (int)S.sk_effecttime);
                                }
                            }
                            break;
                        case TS_MoveSPD:
                            break;
                        case TS_Stamina:
                            if (0 == S.sk_range)
                            {
                                player.stamina += e_value;
                                if (player.stamina >= player.ch.ch_stamina) player.stamina = player.ch.ch_stamina;
                            }
                            else
                            {
                                for (int i = 0; i < 4; ++i)
                                {
                                    if (g_PC[i] == player) continue;
                                    if (g_PC[i] == null) continue;
                                    if (S.sk_range >= Vector3.Distance(g_PC[i].transform.position, player.transform.position)) {
                                        player.stamina += e_value;
                                        if (player.stamina >= player.ch.ch_stamina) player.stamina = player.ch.ch_stamina;
                                    }
                                }
                            }
                            break;
                    }

                    break;
                }
             case E_Make:
                {
                    Instantiate(Eft[EF_VENNOM], player.transform.position, Quaternion.identity);
                    //리소스에서 프리팹 생성!
                    break;
                 }
             case E_Move:
                if (null == target) return false;
                else Debug.Log("HELLO");
                Instantiate(Eft[EF_VENNOM], player.transform.position, Quaternion.identity);
                if (6 == S.sk_targetStatus)
                {
                    int num = (int)UnityEngine.Random.Range(0, TP_Point.Length);
                    player.transform.position = TP_Point[num].position;
                }
                else
                {
                    int num = (int)UnityEngine.Random.Range(0, 4);
                    target.transform.position = MON_TP_Point[num].position;
                }
                 
                 break;
             case E_DiceKill:
                if (null == target) return false;
                else Debug.Log("HELLO");
                Instantiate(Eft[EF_VENNOM], player.transform.position, Quaternion.identity);
                if (UnityEngine.Random.Range(0, 1) < 0.5)
                {
                    
                    if (true == target.Damaged(99999))
                    {
                        if (target.target_count <= 1)
                        {
                            Destroy(target.gameObject);
                            player.ch.target = null;
                            mm.Remove(target);
                        }
                        else
                        {
                            target.target_count -= 1;
                            player.ch.target = null;
                        }
                    }
                }
                break;
         }
 
         //쿨타임 돌리고~
         S.sk_canusetime = timer + S.sk_cooltime;

        Debug.Log("BYE!!");
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

public class skill
{
     public string sk_name;
     public int sk_type;
     public float sk_cooltime;
     public float sk_canusetime;
     public float sk_effecttime;
    public float sk_range;
     public int sk_basedmg;
     public byte sk_basedstatus;
     public float sk_statusmultipledmg;
     public byte sk_effect;
    public int sk_targetStatus;
     public GameObject sk_particle;

    public skill() {
        sk_name = "";
        sk_type = 0;
        sk_cooltime = 0;
        sk_canusetime = 0;
        sk_effecttime = 0;
        sk_range = 0;
        sk_basedmg = 0;
        sk_basedstatus = 0;
        sk_statusmultipledmg = 0;
        sk_effect = 0;
        sk_targetStatus = 0;
        sk_particle = null; 
    }
}