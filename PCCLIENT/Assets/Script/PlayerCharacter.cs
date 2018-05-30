using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerCharacter : MonoBehaviour {

    public Character ch;
    float timer_canidle;
    float timer_atk;
    float[] timer_skillcool = new float[4];
    float atkspd;
    int dmg;

    List<BUFF> b;

    public NavMeshAgent nm;

    public GameObject TESTTMP;
    public SkillSystem SS;

    public void init()
    {
        ch = new Character();
        ch.state = Character.IDLE;
        ch.target = null;

        ch.whereisplaying = 0;
        ch.clearedround = 0;
        ch.remainpoint = 0;

        ch.id = 0;
        ch.ch_type = 0;
        ch.ch_atk = 10;
        ch.ch_str = 0;
        ch.ch_vit = 0;
        ch.ch_int = 0;
        ch.ch_mid = 0;
        ch.ch_matk = 0;
        ch.ch_movespd = 20;
        ch.ch_atkspd = 1;
        ch.ch_stamina = 0;

        ch.nextorder = Character.IDLE;
        ch.skillnumber = 0;
        
        ch.skill = new int[4];
        ch.point = transform.position;
        ch.skillnumber = 0;

        atkspd = ch.ch_atkspd;
        dmg = ch.ch_atk;
        
        nm = GetComponent<NavMeshAgent>();
    }

    public void init(PlayerCharacter c)
    {
        ch = c;

        atkspd = ch.ch_atkspd;
        dmg = ch.ch_atk;

        nm = GetComponent<NavMeshAgent>();
    }

    //5.14 홍승준 함수 추가
    public SC_CHARACTERINFO_PACKET CharacterInfo() {
        SC_CHARACTERINFO_PACKET sc= new SC_CHARACTERINFO_PACKET();
        sc.ch_atk = ch.ch_atk;
        sc.ch_atkspd = ch.ch_atkspd;
        sc.ch_int = ch.ch_int;
        sc.ch_matk = ch.ch_matk;
        sc.ch_mid = ch.ch_mid;
        sc.ch_movespd = ch.ch_movespd;
        sc.ch_stamina = ch.ch_stamina;
        sc.ch_str = ch.ch_str;
        sc.ch_type = ch.ch_type;
        sc.ch_vit = ch.ch_vit;
        return sc;
    } 
    //
    public void Attack(List<Monster> mm)
    {
        if (ch.state != Character.IDLE)
        {
            return;
        }
        ch.state = Character.ATK;
        //모션
        //TESETTESTSETSETSETSETSET
        Instantiate(TESTTMP, ch.target.transform.position, TESTTMP.transform.rotation);

        if (true == ch.target.Damaged(dmg)) {
            if (ch.target.target_count <= 1){
                Destroy(ch.target.gameObject);
                ch.target = null;
                mm.Remove(ch.target);
            }
            else
            {
                ch.target.target_count -= 1;
                ch.target = null;
            }
        }

        for (int i = 0; i < b.Count;)
        {
            if (b[i].atk())
            {

                switch (b[i].type)
                {
                    case SkillSystem.BUF_ATK_UP:
                        {
                            dmg -= b[i].value;
                            break;
                        }
                    case SkillSystem.BUF_ATKSPD_UP:
                        {
                            atkspd -= b[i].value;
                            break;
                        }
                }

                b.Remove(b[i]);
                continue;
            }
            i += 1;
        }

        ch.state = Character.IDLE;
    }

    public void move(Vector3 mP)
    {
        Debug.Log("recv");
        ch.point = new Vector3();
        ch.point = mP;
        Debug.Log(ch.state);
        if (ch.state != Character.IDLE)
        {
            Debug.Log("error");
            ch.nextorder = Character.MOVE;
            ch.point = mP;
            return;
        }
        ch.state = Character.MOVE;
        //모션
        nm.speed = ch.ch_movespd;
        nm.SetDestination(mP);
    }

    public void idle()
    {
        ch.state = Character.IDLE;
        //모션
    }

    public void UseSkill(byte key, float timer)
    {
        if (ch.state == Character.MOVE) {
            nm.Stop();
            idle();
            return;
        }
        if (ch.state != Character.IDLE)
        {
            ch.nextorder = Character.SKILL;
            ch.skillnumber = key;
            return;
        }

        SS.use_skill(this, ch.target, ch.id, key);
        //스킬 사용~
        //skill[key];
        //모션
    }

         public void UseBuff(byte type, int value, int duration)
    {
         BUFF bf = new BUFF();
         bf.type = type;
         bf.value = value;
         bf.duration = duration;
         
                 switch (type)
        {
             case SkillSystem.BUF_ATK_UP: {
             dmg += value;
                                 break;
                             }
                     case SkillSystem.BUF_ATKSPD_UP: {
             atkspd += value;
                                 break;
                             }
                 }
         
     }

public void LocalUpdate(List<Monster> mm, float timer)
    {
        //find
        if (ch.target == null) {
            foreach (Monster m in mm) {
                if (null != m)
                {
                    if (15 > Vector2.Distance(m.transform.position, transform.position))
                    {
                        ch.target = m;
                        ch.target.target_count += 1;
                        break;
                    }
                }
                
            }
        }

        //lost
        if (ch.target != null) {
            if (15 < Vector2.Distance(ch.target.transform.position, transform.position))
            {
                ch.target.target_count -= 1;
                ch.target = null;
            }
            else if (true == ch.target.died) {
                if (ch.target.target_count >= 2)
                {
                    ch.target.target_count -= 1;
                    ch.target = null;
                }
                else
                {
                    mm.Remove(ch.target);
                    Destroy(ch.target.gameObject);
                    ch.target = null;
                }
            }
        }
        
        if (ch.state == Character.MOVE) {
            if ( 0.1 <= nm.remainingDistance) idle();
        }

        if (Character.IDLE == ch.state) {
            //start next order
            switch (ch.nextorder) {
                case Character.MOVE:
                    move(ch.point);
                    ch.nextorder = Character.IDLE;
                    break;
                case Character.IDLE:
                    if (timer_atk <= timer && ch.target != null) {
                        Attack(mm);
                        timer_atk = timer + 1 / ch.ch_atkspd;
                        ch.nextorder = Character.IDLE;
                    }
                    break;
                case Character.SKILL:
                    UseSkill(ch.skillnumber, timer);
                    ch.nextorder = Character.IDLE;
                    break;
            }
        }

    }

     struct BUFF
    {
     public bool atk()
        {
             duration -= 1;
                     if (0 <= duration) return true;
                     else return false;
                 }
     public byte type;
     public int value;
     public int duration;
 }
}
