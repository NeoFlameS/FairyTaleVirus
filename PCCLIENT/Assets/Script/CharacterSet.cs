using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class CharacterSet : MonoBehaviour {

    public Character[] Ch = new Character[4];
    public string[] nickname = new string[4];
    public volatile bool[] skillcheck = { false, false, false, false };
    public int count;
    public Upgrade_Log[] log = new Upgrade_Log[4];
    public int player_count;
    public void Start()
    {
        player_count = 0;
        DontDestroyOnLoad(this);

        for (int i = 0; i < 4; ++i) {
            log[i].ch_atk = 0;
            log[i].ch_str = 0;
            log[i].ch_vit = 0;
            log[i].ch_int = 0;
            log[i].ch_mid = 0;//오후 3시 추가 
        }
    }

    public void init(Character[] c) {
        count = c.Length;
        for (int i = 0; i < count; ++i) {
            Ch[i] = c[i];
        }
    }

    public void Select_skillset(CS_SKILLSET_PACKET CS)
    {
        for (int i = 0; i < 4; i++)
        {
            if (CS.sk_id[i] == 7)
            {
                switch (Ch[CS.id].ch_type)
                {
                    case 0:
                        Ch[CS.id].skill[i] = 105;
                        break;
                    case 1:
                        Ch[CS.id].skill[i] = 101;
                        break;
                    case 2:
                        Ch[CS.id].skill[i] = 107;
                        break;
                    case 3:
                        Ch[CS.id].skill[i] = 103;
                        break;
                    default:
                        break;
                }
            }
            else if (CS.sk_id[i] == 6)
            {
                switch (Ch[CS.id].ch_type)
                {
                    case 0:
                        Ch[CS.id].skill[i] = 104;
                        break;
                    case 1:
                        Ch[CS.id].skill[i] = 100;
                        break;
                    case 2:
                        Ch[CS.id].skill[i] = 106;
                        break;
                    case 3:
                        Ch[CS.id].skill[i] = 102;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                Ch[CS.id].skill[i] = Convert.ToInt16(CS.sk_id[i]);
            }
            Debug.Log("ID : " + CS.id + " / " + i + " Skill " + CS.sk_id[i]);
        }
        skillcheck[CS.id] = true;
        player_count++;

    }

    public void Reset_skillset()
    {
        player_count = 0;
    }
    public void All_data_save() {

        for (int i = 0; i < player_count; ++i) {
            Ch[i].ch_atk = Ch[i].ch_atk + log[i].ch_atk;
            Ch[i].ch_int = (byte)(Ch[i].ch_int+log[i].ch_int);
            Ch[i].ch_vit = (byte)(Ch[i].ch_vit+log[i].ch_vit);
            Ch[i].ch_str = (byte)(Ch[i].ch_str+log[i].ch_str);
            Ch[i].clearedround = (byte)(Ch[i].clearedround + 1);
        }

        GameObject.Find("Select Manager(Clone)").GetComponent<SelectManager>().DataSave(Ch, player_count);
    }
    public SC_CHARACTERINFOSET_PACKET CharacterInfo()
    {
        SC_CHARACTERINFOSET_PACKET setpack = new SC_CHARACTERINFOSET_PACKET();
        setpack.characterinfo = new SC_CHARACTERINFO_PACKET[4];

        for (int i = 0; i < 4; ++i) {
            setpack.characterinfo[i].id = Ch[i].id;
            setpack.characterinfo[i].ch_type = Ch[i].id;
            setpack.characterinfo[i].ch_atk = (byte)(Ch[i].ch_atk+log[i].ch_atk);
            setpack.characterinfo[i].ch_str = (byte)(Ch[i].ch_str + log[i].ch_str);
            setpack.characterinfo[i].ch_vit = (byte)(Ch[i].ch_vit + log[i].ch_vit);
            setpack.characterinfo[i].ch_int = (byte)(Ch[i].ch_int + log[i].ch_int);
            setpack.characterinfo[i].ch_mid = (byte)(Ch[i].ch_mid+log[i].ch_mid);//오후 3시 추가 

            setpack.characterinfo[i].ch_matk = (byte)Ch[i].ch_matk;
            setpack.characterinfo[i].ch_movespd = Ch[i].ch_movespd;
            setpack.characterinfo[i].ch_atkspd = Ch[i].ch_atkspd;
            setpack.characterinfo[i].ch_stamina = Ch[i].ch_stamina;
        }

        return setpack;
    }
}

public struct Character{

    public const byte IDLE = 0;
    public const byte MOVE = 1;
    public const byte SKILL = 2;
    public const byte ATK = 3;

    public const byte REDHOOD = 0;
    public const byte LIBRARY = 1;
    public const byte ALICE = 2;
    public const byte SCROOGI = 3;

    public byte state;
    public Monster target;
    public int[] skill;

    public byte whereisplaying;
    public byte clearedround;
    public byte remainpoint;

    public byte id;
    public byte ch_type;
    public int ch_atk;
    public byte ch_str;
    public byte ch_vit;
    public byte ch_int;
    public byte ch_mid;
    public int ch_matk;
    public short ch_movespd;
    public float ch_atkspd;
    public byte ch_stamina;

    public byte nextorder;
    public Vector3 point;
    public byte skillnumber;

}

public struct Upgrade_Log {
    public int ch_atk;
    public int ch_str;
    public int ch_vit;
    public int ch_int;
    public int ch_mid;//오후 3시 추가 
}