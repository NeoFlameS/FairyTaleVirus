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

    public void DeleteCharacter(byte ch_id)
    {
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
        }
    }

    public bool MakeCharacter(byte ch_type, byte id)
    {
        //CHARACTER INIT OPTION SET=============================================
        savedata.CharacterData[id].id = id;
        savedata.CharacterData[id].clearedround = 0;
        savedata.CharacterData[id].remainpoint = 0;
        savedata.CharacterData[id].ch_type = ch_type;
        savedata.CharacterData[id].ch_atk = 0;
        savedata.CharacterData[id].ch_str = 0;
        savedata.CharacterData[id].ch_vit = 0;
        savedata.CharacterData[id].ch_int = 0;
        savedata.CharacterData[id].ch_mid = 0;
        savedata.CharacterData[id].ch_matk = 0;
        savedata.CharacterData[id].ch_movespd = 0;
        savedata.CharacterData[id].ch_atkspd = 0;
        savedata.CharacterData[id].ch_stamina = 0;
        savedata.CharacterData[id].DefaultSkillSetData = new int[4] { 0, 0, 0, 0 };
        charactercount++;

        CC[id].ch_type = ch_type;
        CC[id].clearedround = 0;
        CC[id].skillset = new int[4] { 0, 0, 0, 0 };
        CC[id].show();

        return true;
    }

    public Character[] LoadToScene(byte[] id, int count) {

        Debug.Log(id[0]);
        Debug.Log(count);
        Character[] ch = new Character[4];
       
        //match id connect to ch;
        for (int i = 0; i < count; ++i)
        {
            byte cid = id[i];

            ch[i] = new Character();
            ch[i].state = 0;
            ch[i].target = null;
            ch[i].skill = new int[4];

            ch[i].whereisplaying = 0;
            ch[i].clearedround = savedata.CharacterData[cid].clearedround;
            ch[i].remainpoint = savedata.CharacterData[cid].remainpoint;

            ch[i].id = cid;
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
    public bool DataSave(Character[] ch, int[][] skillset, int playercount)
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
                savedata.CharacterData[id].DefaultSkillSetData = skillset[i];
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
}

[Serializable]
public struct CHARACTERDATA
{
    public byte id;
    public byte clearedround;
    public byte remainpoint;
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
    public int[] DefaultSkillSetData;
}

[Serializable]
public struct SAVEDATA
{
    public CHARACTERDATA[] CharacterData;
}