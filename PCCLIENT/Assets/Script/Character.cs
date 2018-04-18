using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

    byte state;
    GameObject target;
    int[] skill = new int[4];

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

    // Use this for initialization
    void Start () {

	}

    void Attack() { }
    void move() { }
    void idle() { }
    void UseSkill() { }
    void LocalUpdate() { }
}