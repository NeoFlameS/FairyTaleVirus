using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSet : MonoBehaviour {

    public Character[] Ch = new Character[4];
    public string[] nickname = new string[4];
    int count = 0;

    public void Start()
    {
        DontDestroyOnLoad(this);
    }

    public void init(Character[] c) {
        count = c.Length;
        for (int i = 0; i < count; ++i) {
            Ch[i] = c[i];
        }
    }
}

public struct Character{

    public const byte IDLE = 0;
    public const byte MOVE = 1;
    public const byte SKILL = 2;
    public const byte ATK = 3;

    public byte state;
    public Monster target;
    public int[] skill;

    public byte whereisplaying;
    public byte clearedround;
    public byte remainpoint;

    public byte id;
    public byte ch_type;
    public byte ch_atk;
    public byte ch_str;
    public byte ch_vit;
    public byte ch_int;
    public byte ch_mid;
    public byte ch_matk;
    public short ch_movespd;
    public float ch_atkspd;
    public byte ch_stamina;

    public byte nextorder;
    public Vector3 point;
    public byte skillnumber;

}