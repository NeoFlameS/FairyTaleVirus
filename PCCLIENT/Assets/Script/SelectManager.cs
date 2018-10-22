using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SelectManager : MonoBehaviour {

    public const int MAX_CHARACTER_SAVE = 9;

    public CharacterCard[] CC = new CharacterCard[MAX_CHARACTER_SAVE];
    SAVEDATA savedata;
    int charactercount;
    //5.18일 수정사항 홍승준
    bool[] select_player = new bool[4];
    byte[] select_slot = new byte[4];
    bool[] show_card = {false,false,false,false,false,false,false,false,false};
    //6.1일 수정 홍승준
    public bool not_show = false;

    public void Start()
    {
        DontDestroyOnLoad(this);
        savedata = new SAVEDATA();
        savedata.CharacterData = new CHARACTERDATA[9];
        
        for (int i = 0; i < 9; ++i)
        {
            string filename = "SLOT1 (" + i + ")";
            GameObject tmp = GameObject.Find(filename);
            if (null != tmp) CC[i] = tmp.GetComponent<CharacterCard>();
        }

        DataLoad();
    }

    public void OnLevelWasLoaded()
    {
        if (2 != SceneManager.GetActiveScene().buildIndex) return;
        for (int i = 0; i < 9; ++i)
        {
            string filename = "SLOT1 (" + i + ")";
            GameObject tmp = GameObject.Find(filename);
            if (null != tmp) CC[i] = tmp.GetComponent<CharacterCard>();
        }
        DataLoad();
    }

    public void SelfSave() {
        FileStream savefile = File.Open(Application.persistentDataPath + "/SavedData.dat", FileMode.Create);
        try
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(savefile, savedata);
        }
        catch (Exception exception)
        {
            Debug.Log(exception.ToString());
        }
    }

    public void DeleteCharacter(CharacterCard cc/*byte ch_id*/)
    {
        /*
        for (int i = 0; i < MAX_CHARACTER_SAVE; ++i) {
            if (savedata.CharacterData[i].id == ch_id) {
                savedata.CharacterData[i].id = 100;
                savedata.CharacterData[i].clearedround = 0;
                savedata.CharacterData[i].remainpoint = 0;
                savedata.CharacterData[i].ch_type = 125;
                savedata.CharacterData[i].ch_atk = 0;
                savedata.CharacterData[i].ch_str = 0;
                savedata.CharacterData[i].ch_vit = 0;
                savedata.CharacterData[i].ch_int = 0;
                savedata.CharacterData[i].ch_mid = 0;
                savedata.CharacterData[i].ch_matk = 0;
                savedata.CharacterData[i].ch_movespd = 0;
                savedata.CharacterData[i].ch_atkspd = 0;
                savedata.CharacterData[i].ch_stamina = 0;
                savedata.CharacterData[i].DefaultSkillSetData = null;
                charactercount--;

                CC[i].init();
                break;
            }
        }*/
        for (int i = 0; i < MAX_CHARACTER_SAVE; i++)
        {
            if (CC[i].Equals(cc))
            {
                savedata.CharacterData[i].id = 100;
                savedata.CharacterData[i].clearedround = 0;
                savedata.CharacterData[i].remainpoint = 0;
                savedata.CharacterData[i].ch_type = 125;
                savedata.CharacterData[i].ch_atk = 0;
                savedata.CharacterData[i].ch_str = 0;
                savedata.CharacterData[i].ch_vit = 0;
                savedata.CharacterData[i].ch_int = 0;
                savedata.CharacterData[i].ch_mid = 0;
                savedata.CharacterData[i].ch_matk = 0;
                savedata.CharacterData[i].ch_movespd = 0;
                savedata.CharacterData[i].ch_atkspd = 0;
                savedata.CharacterData[i].ch_stamina = 0;
                savedata.CharacterData[i].DefaultSkillSetData = null;
                charactercount--;
                CC[i].init();
                break;
            }
        }
    }
    //5.18일 수정사항 홍승준
    public void Before_Make(byte id,byte player_num) {
        select_player[player_num] = true;
        select_slot[player_num] = id;
    }

    public bool MakeCharacter(byte ch_type, byte player_num, object cs)//5.31 홍승준 수정
    {
        CS_SELECT_PACKET sc = (CS_SELECT_PACKET)cs;
        byte id=0;
        if (select_player[player_num])
        {
            id = select_slot[player_num];
        }
        else {
            return false;
        }
        
        //CHARACTER INIT OPTION SET=============================================
        savedata.CharacterData[id].id = id;
        savedata.CharacterData[id].clearedround = 0;
        savedata.CharacterData[id].remainpoint = 0;
        savedata.CharacterData[id].ch_type = sc.model;
        savedata.CharacterData[id].ch_movespd = 10;
        savedata.CharacterData[id].ch_atkspd = 1;
        savedata.CharacterData[id].DefaultSkillSetData = new int[4] { 0, 0, 0, 0 };

        switch (sc.model) {
            case Character.REDHOOD:
                savedata.CharacterData[id].ch_str = 2;
                savedata.CharacterData[id].ch_vit = 5;
                savedata.CharacterData[id].ch_int = 13;
                savedata.CharacterData[id].ch_mid = 20;
                break;
            case Character.LIBRARY:
                savedata.CharacterData[id].ch_str = 15;
                savedata.CharacterData[id].ch_vit = 10;
                savedata.CharacterData[id].ch_int = 5;
                savedata.CharacterData[id].ch_mid = 10;
                break;
            case Character.ALICE:
                savedata.CharacterData[id].ch_str = 10;
                savedata.CharacterData[id].ch_vit = 2;
                savedata.CharacterData[id].ch_int = 20;
                savedata.CharacterData[id].ch_mid = 8;
                break;
            case Character.SCROOGI:
                savedata.CharacterData[id].ch_str = 20;
                savedata.CharacterData[id].ch_vit = 20;
                savedata.CharacterData[id].ch_int = 0;
                savedata.CharacterData[id].ch_mid = 0;
                break;
        }

        savedata.CharacterData[id].ch_atk = savedata.CharacterData[id].ch_str * 2;
        savedata.CharacterData[id].ch_matk = savedata.CharacterData[id].ch_int * 2;
        savedata.CharacterData[id].ch_stamina = (byte)(20+ savedata.CharacterData[id].ch_vit+ savedata.CharacterData[id].ch_mid);
        charactercount++;

        CC[id].ch_type = sc.model;
        CC[id].clearedround = 0;
        CC[id].skillset = new int[4] { 0, 0, 0, 0 };
        show_card[id] = true;
        //CC[id].show();

        //5.31 홍승준 수정
        Debug.Log("id : " + Convert.ToString(sc.id));
        Debug.Log("type : " + Convert.ToString(sc.type));
        Debug.Log("model : " + Convert.ToString(sc.model));


        return true;
    }
    //
    public Character[] LoadToScene(byte[] id, int count) {
        Character[] ch = new Character[4];
       
        //match id connect to ch;
        for (int i = 0; i < 4; ++i)
        {
            if (i >= count)
            {
                ch[i].ch_type = 125;
                continue;
            }
            byte cid = id[i];

            ch[i] = new Character();
            ch[i].state = 0;
            ch[i].target = null;
            ch[i].skill = new int[4];

            ch[i].whereisplaying = 0;
            ch[i].clearedround = savedata.CharacterData[cid].clearedround;
            ch[i].remainpoint = savedata.CharacterData[cid].remainpoint;

            ch[i].id = (byte)i;
            ch[i].ch_type = savedata.CharacterData[cid].ch_type;
            ch[i].ch_atk = savedata.CharacterData[cid].ch_atk;
            ch[i].ch_str = savedata.CharacterData[cid].ch_str;
            ch[i].ch_vit = savedata.CharacterData[cid].ch_vit;
            ch[i].ch_int = savedata.CharacterData[cid].ch_int;
            ch[i].ch_mid = savedata.CharacterData[cid].ch_mid;
            ch[i].ch_matk = savedata.CharacterData[cid].ch_matk;
            ch[i].ch_movespd = savedata.CharacterData[cid].ch_movespd;
            ch[i].ch_atkspd = savedata.CharacterData[cid].ch_atkspd;
            ch[i].ch_stamina = savedata.CharacterData[cid].ch_stamina;
            
        }

        return ch;
    }

    public bool DataLoad()
    {
        FileStream savefile = File.Open(Application.persistentDataPath + "/SavedData.dat", FileMode.OpenOrCreate);

        if (savefile == null) return false;

        if (savefile.Length == 0)
        {
            //there is no file, make file!
            //first play!
            //CHARACTER INIT OPTION SET=============================================
            for (int i = 0; i < MAX_CHARACTER_SAVE; ++i)
            {
                savedata.CharacterData[i].id = 100;
                savedata.CharacterData[i].clearedround = 0;
                savedata.CharacterData[i].remainpoint = 0;
                savedata.CharacterData[i].ch_type = 125;
                savedata.CharacterData[i].ch_atk = 0;
                savedata.CharacterData[i].ch_str = 0;
                savedata.CharacterData[i].ch_vit = 0;
                savedata.CharacterData[i].ch_int = 0;
                savedata.CharacterData[i].ch_mid = 0;
                savedata.CharacterData[i].ch_matk = 0;
                savedata.CharacterData[i].ch_movespd = 0;
                savedata.CharacterData[i].ch_atkspd = 0;
                savedata.CharacterData[i].ch_stamina = 0;
                savedata.CharacterData[i].DefaultSkillSetData = null;
            }

            try
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(savefile, savedata);
                savefile.Close();
            }
            catch (Exception exception)
            {
                Debug.Log(exception.ToString());
                return false;
            }
        }
        else
        {
            //file is opened
            //deserialize
            try
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                savedata = (SAVEDATA)binaryFormatter.Deserialize(savefile);
                savefile.Close();
            }
            catch (Exception exception)
            {
                Debug.Log(exception.ToString());
                return false;
            }
        }


        //how to show in scene ?????? => CLEARED ROUND + CH_TYPE + DEFAULTSKILLSET ~ //
        for (int i = 0; i < MAX_CHARACTER_SAVE; ++i)
        {
            CC[i].init();
            CC[i].clearedround = savedata.CharacterData[i].clearedround;
            CC[i].ch_type = savedata.CharacterData[i].ch_type;
            CC[i].skillset = savedata.CharacterData[i].DefaultSkillSetData;
            CC[i].show();
        }

        return true;
    }

    //save time = after scoring, after Chracter Scene Changed
    public bool DataSave(Character[] ch, int playercount)
    {
        FileStream savefile = File.Open(Application.persistentDataPath + "/SavedData.dat", FileMode.Create);

        //data bind
        for (int i = 0; i < playercount; ++i) {
            if (ch[i].whereisplaying == 0){
                int id = ch[i].id;
                savedata.CharacterData[id].id = ch[i].id;
                savedata.CharacterData[id].clearedround = ch[i].clearedround;
                savedata.CharacterData[id].remainpoint = ch[i].remainpoint;
                savedata.CharacterData[id].ch_type = ch[i].ch_type;
                savedata.CharacterData[id].ch_atk = ch[i].ch_atk;
                savedata.CharacterData[id].ch_str = ch[i].ch_str;
                savedata.CharacterData[id].ch_vit = ch[i].ch_vit;
                savedata.CharacterData[id].ch_int = ch[i].ch_int;
                savedata.CharacterData[id].ch_mid = ch[i].ch_mid;
                savedata.CharacterData[id].ch_matk = ch[i].ch_matk;
                savedata.CharacterData[id].ch_movespd = ch[i].ch_movespd;
                savedata.CharacterData[id].ch_atkspd = ch[i].ch_atkspd;
                savedata.CharacterData[id].ch_stamina = ch[i].ch_stamina;
                savedata.CharacterData[id].DefaultSkillSetData = ch[i].skill;
            }
        }

        //serialize
        try
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(savefile, savedata);
        }
        catch (Exception exception)
        {
            Debug.Log(exception.ToString());
            return false;
        }

        return true;
    }
    //5.18 홍승준 추가//5.31 수정
    private void Update()
    {
        int i = 0;
        for (i = 0; i < MAX_CHARACTER_SAVE; i++)
        {
            if (not_show)
            {
                break;
            }
            CC[i].show();
        }
    }
    //
}

[Serializable]
public struct CHARACTERDATA
{
    public byte id;
    public byte clearedround;
    public byte remainpoint;
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
    public int[] DefaultSkillSetData;
}

[Serializable]
public struct SAVEDATA
{
    public CHARACTERDATA[] CharacterData;
}