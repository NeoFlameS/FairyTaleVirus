using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class ManaSystem : MonoBehaviour {

    public Text ManaRate;

    int dead_count;//죽은 몹을 카운트 한다.
    int mana; //보유중인 마나
    MainGameSystem MGS;
    CharacterSet chs;

    Queue up_packet = new Queue();
	// Use this for initialization
	void Start () {
        
        MobileNetwork mb = GameObject.Find("Network Manager(Clone)").GetComponent<MobileNetwork>();
        MGS = GameObject.Find("GAME SYSTEM").GetComponent<MainGameSystem>();
        chs = GameObject.Find("Selected Character Status(Clone)").GetComponent<CharacterSet>();

        mb.Get_InGameSystem();//6.17 수정

        int player = mb.Return_PlayerCount();
        int i = 0;
        for (i = 0; i < player; i++) {
            mb.SignalSend(i,NetworkController.SC_IN_GAME);
        }
        
        dead_count = 0;
        mana = 5;
        
	}

    public void Dead_sign() {//몹이 죽었을 때 카운트
        dead_count++;
        if (dead_count >= 10) {//일단 열마리당 마나 1씩 획득
            mana++;
            dead_count -= 10;
            GameObject.Find("ResultData").GetComponent<ResultDataSet>().mana++;
        }
    }

   
    public bool Upgrade_reciev(object up) {
        Debug.Log("UPgrade Recieve");
        lock (up_packet) { 

        CS_UPGRADE_PACKET cur = new CS_UPGRADE_PACKET();
        cur = (CS_UPGRADE_PACKET)up;

        up_packet.Enqueue(cur);

            Debug.Log("UP id : " + cur.id + " / UP type : " + cur.up_sg);
            Debug.Log("현재 큐 개수 : "+up_packet.Count);
        }
        
        return true;
    }
    // Update is called once per frame
    void Update()
    {
        ManaRate.text = "마나 : " + mana;
        CS_UPGRADE_PACKET cs;
        lock (up_packet)
        {
            if (up_packet.Count > 0)
            {
                cs = (CS_UPGRADE_PACKET)up_packet.Dequeue();
                if (mana > 0)
                {
                    mana -= 1;
                    Proc_Upgrade(cs.id, cs.up_sg);
                }
                int id = cs.id;
                Debug.Log("Id : "+id+" upgrade type: "+cs.up_sg);
            }
        }
    }

    void Proc_Upgrade(byte id, byte type) {
        switch (type) {
            case 0://str
                MGS.PC[id].ch.ch_str++;
                chs.log[id].ch_str++;
                MGS.PC[id].ch.ch_atk += 2;
                chs.log[id].ch_atk += 2;
                break;
            case 1://atk
                MGS.PC[id].ch.ch_str++;
                chs.log[id].ch_str++;
                MGS.PC[id].ch.ch_atk+=2;
                chs.log[id].ch_atk+=2;
                break;
            case 2://vit
                MGS.PC[id].ch.ch_vit++;
                chs.log[id].ch_vit++;
                break;
            case 3://int
                MGS.PC[id].ch.ch_int++;
                chs.log[id].ch_int++;
                break;
            case 4://mind//오후 3시 추가 
                MGS.PC[id].ch.ch_mid++;
                chs.log[id].ch_mid++;
                break;
            default:
                return;
        }
       
    }
}

