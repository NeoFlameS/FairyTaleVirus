using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

public class Monster : MonoBehaviour {

    public byte mon_type;
    public byte mon_state;

    public int mon_hpMax;
    public int mon_hpNow;
    public int mon_basemovespd;
    public int mon_movespd;
    public byte[] mon_ability = new byte[4];
    public int mon_mana;
    public int mon_infect;
    public int mon_grade;

    public int target_count = 0;
    public bool died = false;

    public Vector3 goal;

    public Monster() {
        mon_hpMax = 0;
        mon_hpNow = 0;
        mon_basemovespd = 0;
        mon_movespd = 0;
        for (int i = 0; i < 4; ++i) {
            mon_ability[i] = 0;
        }
        mon_mana = 0;
        mon_infect = 0;
        mon_grade = 0;
    }

    public void init()
    {
        mon_hpMax = 1;
        mon_hpNow = 1;
        mon_basemovespd = 10;
        mon_movespd = 0;
        for (int i = 0; i < 4; ++i)
        {
            mon_ability[i] = 0;
        }
        mon_mana = 0;
        mon_infect = 0;
        mon_grade = 0;

        goal = GameObject.Find("Oak_Tree").transform.position;
    }

    public bool Damaged(int dmg)
    {
        mon_hpNow -= dmg;
        if (mon_hpNow <= 0)
        {
            GameObject.Find("ManaSystem").GetComponent<ManaSystem>().Dead_sign();//5.19일 홍승준 수정
            return true;
        }
        return false;
    }

    public void Move(){
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(goal);
    }
}