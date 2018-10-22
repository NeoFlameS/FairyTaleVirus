using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerCharacter : MonoBehaviour
{

    public Character ch;
    float timer_canidle;
    float timer_atk;
    float[] timer_skillcool = new float[4];
    float atkspd;
    public int stamina;
    int dmg;

    List<BUFF> b = new List<BUFF>();

    public NavMeshAgent nm;
    public Animator anim;

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
        ch.ch_movespd = 10;
        ch.ch_atkspd = 1;
        ch.ch_stamina = 0;

        ch.nextorder = Character.IDLE;
        ch.skillnumber = 0;

        ch.skill = new int[4];
        ch.point = transform.position;
        ch.skillnumber = 0;

        stamina = ch.ch_stamina;
        atkspd = ch.ch_atkspd;
        dmg = ch.ch_atk;

        nm = GetComponent<NavMeshAgent>();
        SS = GameObject.Find("GAME SYSTEM").GetComponent<SkillSystem>();
        anim = GetComponent<Animator>();
    }

    public void init(Character c)
    {
        ch = c;

        stamina = ch.ch_stamina;
        atkspd = ch.ch_atkspd;
        dmg = ch.ch_atk;

        nm = GetComponent<NavMeshAgent>();
        SS = GameObject.Find("GAME SYSTEM").GetComponent<SkillSystem>();
        anim = GetComponent<Animator>();
    }

    //5.14 홍승준 함수 추가
    public SC_CHARACTERINFO_PACKET CharacterInfo()
    {
        SC_CHARACTERINFO_PACKET sc = new SC_CHARACTERINFO_PACKET();
        sc.ch_atk = (byte)ch.ch_atk;
        sc.ch_atkspd = ch.ch_atkspd;
        sc.ch_int = ch.ch_int;
        sc.ch_matk = (byte)ch.ch_matk;
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
        anim.SetInteger("State", ch.state);
        //TESETTESTSETSETSETSETSET
        Instantiate(TESTTMP, ch.target.transform.position, TESTTMP.transform.rotation);

        if (true == ch.target.Damaged(dmg))
        {
            if (ch.target.target_count <= 1)
            {
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
                            dmg -= (int)b[i].value;
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
    }

    public void move(Vector3 mP)
    {
        ch.point = new Vector3();
        ch.point = mP;
        if (ch.state != Character.IDLE)
        {
            ch.nextorder = Character.MOVE;
            ch.point = mP;
            return;
        }
        ch.state = Character.MOVE;
        //모션
        anim.SetInteger("State", ch.state);
        anim.SetFloat("Blend", 1);
        nm.speed = ch.ch_movespd;
        nm.SetDestination(mP);
    }

    public void idle()
    {
        ch.state = Character.IDLE;
        anim.SetInteger("State", ch.state);
        anim.SetFloat("Blend", 0);
        //모션
    }

    public void m_UseSkill(byte key) {
        ch.nextorder = Character.SKILL;
        ch.skillnumber = key;
    }

    public void UseSkill(byte key, float timer, List<Monster> L)
    {
        if (ch.state != Character.IDLE)
        {
            ch.nextorder = Character.SKILL;
            ch.skillnumber = key;
            return;
        }

        ch.state = Character.SKILL;
        if (false == SS.use_skill(this, ch.target, ch.id, key, L)) {
            ch.state = Character.IDLE;
            return;
        }
        anim.SetInteger("State", ch.state);
        Debug.Log("SKILL USING!!!");
        //스킬 사용~
        //skill[key];
        //모션
    }

    public void UseBuff(byte type, int value, int duration)
    {
        BUFF bf = new BUFF();
        bf.type = type;
        bf.duration = duration;

        switch (type)
        {
            case SkillSystem.BUF_ATK_UP:
                {
                    bf.value = dmg * value / 100;
                    dmg += value;
                    break;
                }
            case SkillSystem.BUF_ATKSPD_UP:
                {
                    bf.value = atkspd * value / 100;
                    atkspd += value;
                    break;
                }
        }

        b.Add(bf);
    }

    public void AnimAlert(byte State) {
        ch.state = State;
        anim.SetInteger("State", ch.state);
        anim.SetFloat("Blend", 0);
    }

    public void LocalUpdate(List<Monster> mm, float timer)
    {
        //find
        if (ch.target == null)
        {
            foreach (Monster m in mm)
            {
                if (null != m)
                {
                    if (225 > (m.transform.position.x - transform.position.x) * (m.transform.position.x - transform.position.x)
                        + (m.transform.position.z - transform.position.z) * (m.transform.position.z - transform.position.z))
                    {
                        ch.target = m;
                        ch.target.target_count += 1;
                        break;
                    }
                }

            }
        }

        //lost
        if (ch.target != null)
        {
            if (225 <= (ch.target.transform.position.x - transform.position.x) * (ch.target.transform.position.x - transform.position.x)
                        + (ch.target.transform.position.z - transform.position.z) * (ch.target.transform.position.z - transform.position.z))
            {
                ch.target.target_count -= 1;
                ch.target = null;
            }
            else if (true == ch.target.died)
            {
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

        if (ch.state == Character.MOVE)
        {
            if (2 >= nm.remainingDistance)
            {
                idle();
            }
        }

        if (Character.IDLE == ch.state)
        {
            //start next order
            switch (ch.nextorder)
            {
                case Character.MOVE:
                    move(ch.point);
                    ch.nextorder = Character.IDLE;
                    break;
                case Character.IDLE:
                    if (timer_atk <= timer && ch.target != null)
                    {
                        Attack(mm);
                        timer_atk = timer + 1 / ch.ch_atkspd;
                        ch.nextorder = Character.IDLE;
                    }
                    break;
                case Character.SKILL:
                    UseSkill(ch.skillnumber, timer, mm);
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
        public float value;
        public int duration;
    }
}
