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

    AudioSource AS;
    public AudioClip sound_die;

    public float[] difficulty_value = new float[3];

    public int target_count = 0;
    public bool died = false;
    
    int slowed_value = 0;

    float move_debufstack = 1;
    int move_bufstack = 0;
    public Vector3 goal;

    private void Start()
    {
        AS = GameObject.Find("Main Camera").GetComponent<AudioSource>();
    }

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

        difficulty_value[0] = 1F;
        difficulty_value[1] = 1.3F;
        difficulty_value[2] = 2F;
    }

     public void init(byte round, byte stage, int usernumber, int type, int difficulty)
     {
         int hpfactor = (int)((1 + (((round + stage - 1) * (round + stage - 1) / 4 + 1) / 10)) * 40 * difficulty_value[difficulty]);
         mon_type = (byte)type;
         float randnum = (type + 2) / 2;

         double hpfactor2 = 1 + ((usernumber - 1) * 0.7);

         mon_hpMax = (int)(hpfactor* randnum * hpfactor2);
         mon_hpNow = (int)(hpfactor * randnum * hpfactor2);

         mon_basemovespd = (int)(30-(randnum* 10)+((difficulty_value[difficulty] - 1)*10));
         mon_movespd = mon_basemovespd;
         for (int i = 0; i< 4; ++i)
         {
             mon_ability[i] = 0;
         }
         mon_mana = round;
         mon_infect = round + stage;
         mon_grade = (int)(hpfactor / 40);

        mon_hpMax /= 4;
        mon_hpNow /= 4;

        goal = GameObject.Find("End Point").transform.position;
    }

    public bool Damaged(int dmg)
    {
        mon_hpNow -= dmg;
        if (mon_hpNow <= 0)
        {
            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            agent.isStopped = true;
            died = true;
            AS.PlayOneShot(sound_die);
            GameObject.Find("ManaSystem").GetComponent<ManaSystem>().Dead_sign();//5.19일 홍승준 수정
            GameObject.Find("ResultData").GetComponent<ResultDataSet>().monster++;//0624 홍승준 추가
            return true;
        }
        return false;
    }

    public void Slow(float value)
    {
        mon_movespd = (int)(mon_movespd * value / 100);
        move_debufstack += 1;
    }

     public void Faster(int value)
    {
        mon_movespd += value;
        move_bufstack += 1;
    }

    public void Move(){
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(goal);
    }
}