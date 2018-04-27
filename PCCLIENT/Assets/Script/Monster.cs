using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Monster : MonoBehaviour {

    byte mon_type;
    byte mon_state;

    int mon_hpMax;
    int mon_hpNow;
    int mon_basemovespd;
    int mon_movespd;
    byte[] mon_ability;
    int mon_mana;
    int mon_infect;
    int mon_grade;

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

    public bool Damaged(int dmg)
    {
        mon_hpNow -= dmg;
        if (mon_hpNow <= 0)
        {
            Destroy(this);
            return true;
        }
        return false;
    }

    public void Move(){
       //길찾기 알고리즘
    }
}