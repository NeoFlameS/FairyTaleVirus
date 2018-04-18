using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
using System.IO;

public class GameManager : MonoBehaviour {

    public const int MAX_CHARACTER_SAVE = 9;

    SAVEDATA savedata;
    int charactercount;

    public GameManager() {
        savedata = new SAVEDATA();
        savedata.CharacterData = new CHARACTERDATA[9];
    }

    public void DeleteCharacter(byte ch_id)
    {
        for (int i = 0; i < MAX_CHARACTER_SAVE; ++i) {
            if (savedata.CharacterData[i].id == ch_id) {
                savedata.CharacterData[i].id = 100;
                savedata.CharacterData[i].clearedround = 0;
                savedata.CharacterData[i].remainpoint = 0;
                savedata.CharacterData[i].ch_type = 0;
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
                break;
            }
        }
    }

    public bool MakeCharacter(byte ch_type)
    {
        byte id = 0;
        bool[] can = new bool[10];

        //temp id find
        for (int i = 0; i < MAX_CHARACTER_SAVE; ++i)
        {
            can[i] = true;
        }
        for (int i = 0; i < MAX_CHARACTER_SAVE; ++i) {
            can[savedata.CharacterData[i].id] = false;
        }
        for (byte i = 0; i < MAX_CHARACTER_SAVE; ++i)
        {
            if (true == can[i]) {
                id = i;
                break;
            };
        }

        //CHARACTER INIT OPTION SET=============================================
        for (int i = 0; i < MAX_CHARACTER_SAVE; ++i){
            if (savedata.CharacterData[i].id == 100){
                savedata.CharacterData[i].id = id;
                savedata.CharacterData[i].clearedround = 0;
                savedata.CharacterData[i].remainpoint = 0;
                savedata.CharacterData[i].ch_type = ch_type;
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
                charactercount++;
                break;
            }
        }

        return true;
    }

    public Character[] LoadToScene(byte[] id) {
        var count = id.Length;
        Character[] ch = new Character[count];

        //match id connect to ch;

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
            savefile.Close();

            //CHARACTER INIT OPTION SET=============================================
            for (int i = 0; i < MAX_CHARACTER_SAVE; ++i)
            {
                savedata.CharacterData[i].id = 100;
                savedata.CharacterData[i].clearedround = 0;
                savedata.CharacterData[i].remainpoint = 0;
                savedata.CharacterData[i].ch_type = 0;
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
            }
            catch (Exception exception)
            {
                Debug.Log(exception.ToString());
                return false;
            }
        }
        
        //file is opened
        //deserialize
        try
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            savedata = (SAVEDATA)binaryFormatter.Deserialize(savefile);
        }
        catch (Exception exception)
        {
            Debug.Log(exception.ToString());
            return false;
        }

        //how to show in scene ?????? => CLEARED ROUND + CH_TYPE + DEFAULTSKILLSET ~ //

        return true;
    }

    //세이브 타이밍 = 게임 클리어시바로, 캐릭터 씬 변경 후 바로~
    //멀티 세이브 타이밍 = 게임 클리어 후 스코어 등 전송 후
    public bool DataSave(Character[] ch, byte[][] skillset, int playercount)
    {
        FileStream savefile = File.Create(Application.persistentDataPath + "/SavedData.dat");
       
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
    
    public object ByteToObj(byte[] arr)
    {
        try
        {
            using (MemoryStream stream = new MemoryStream(arr))
            {
                IFormatter binaryFormatter = new BinaryFormatter();
                stream.Position = 0;
                return binaryFormatter.Deserialize(stream);
            }
        }
        catch (Exception exception)
        {
            Debug.Log(exception.ToString());
        }
        return null;
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
    public byte[] DefaultSkillSetData;
}

[Serializable]
public struct SAVEDATA
{
    public CHARACTERDATA[] CharacterData;
}