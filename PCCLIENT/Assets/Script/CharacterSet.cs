using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSet : MonoBehaviour {

    Character[] Ch = new Character[4];
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

public class Character{

    public const byte IDLE = 0;
    public const byte MOVE = 1;
    public const byte SKILL = 2;
    public const byte ATK = 3;

    public byte state;
    public GameObject target;
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
    public Vector2 point;
    public byte skillnumber;


    // Use this for initialization
    public Character()
    {
        state = 0;
        target = null;

        whereisplaying = 0;
        clearedround = 0;
        remainpoint = 0;

        id = 0;
        ch_type = 0;
        ch_atk = 0;
        ch_str = 0;
        ch_vit = 0;
        ch_int = 0;
        ch_mid = 0;
        ch_matk = 0;
        ch_movespd = 0;
        ch_atkspd = 0;
        ch_stamina = 0;

        nextorder = 0;
        skillnumber = 0;

        point = new Vector2(0, 0);
        skill = new int[4];
    }

    public void Attack()
    {
        if (state != IDLE)
        {
            return;
        }
        state = ATK;
        //모션
        //원거리냐 근거리냐 판단
        //원거리면 발싸.
        //근거리면 그냥 타겟에 공격판정먹임
    }
    public void move(Vector2 mP)
    {
        point = mP;
        if (state != IDLE)
        {
            nextorder = MOVE;
            return;
        }
        state = MOVE;
        //모션
        //움직임 명령
    }
    public void idle()
    {
        state = IDLE;
        //모션
    }
    public void UseSkill(byte key)
    {
        if (state != IDLE)
        {
            nextorder = SKILL;
            return;
        }
        //스킬 사용~
        //skill[key];
        //모션
    }
    public void LocalUpdate()
    {
        if (target != null && state == IDLE) Attack();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Monster") return;
        if (target != null) return;
        target = collision.gameObject;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Monster" && collision.gameObject == target) target = null;
    }
}