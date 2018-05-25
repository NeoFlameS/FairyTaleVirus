using System;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class NetworkController : MonoBehaviour
{
    public const short MAXBUFFERSIZE = 500;
    public const int MAX_CONNECT = 4;

    public const byte CS_CONNECT = 0;
    public const byte CS_DISCONNECT = 1;
    public const byte CS_MOVE = 2;
    public const byte CS_BTN = 3;

    public const byte SC_CONNECT = 10;
    public const byte SC_DISCONNECT = 11;
    public const byte SC_RECONNECT = 12;
    public const byte SC_CHARACTERINFO = 13;
    public const byte SC_CHARACTERINFOSET = 14;
    public const byte SC_SKILLSET = 15;
    public const byte SC_SCENECHANGE = 16;

    public const byte PP_CONNECT = 20;
    public const byte SP_DISCONNECT = 21;
    public const byte SP_RECONNECT = 22;
    public const byte SP_MONSTERSETINFO = 23; // 라운드 시작시
    public const byte SP_MONSTERINFO = 24; // 게임 도중
    public const byte SP_PLAYERINFO = 25; // 이동
    public const byte SP_LOBYSTATE = 26;
    public const byte SP_INGAMESTATE = 27; // 라운드 
    public const byte SP_SCENEMOVE = 28;
    public const byte SP_ENDGAME = 29;
    public const byte SP_WINGAME = 30;
    public const byte PP_SKILLSET = 31;

    public const byte CS_REQCHR = 32;
    public const byte SC_SELECT = 33;
    public const byte CS_SKILL = 34;
    public const byte CS_UPGRADE = 35;
    public const byte S_NULL = 125;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public byte[] ObjToByte(object obj)
    {
        try
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(stream, obj);
                return stream.ToArray();
            }
        }
        catch (Exception exception)
        {
            Debug.Log(exception.ToString());
        }
        return null;
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

    public bool net_send_signal(byte type, Socket s)
    {
        byte[] send_signal = new byte[1] { type };
        //Debug.Log(type);
        s.Send(send_signal);
        return true;
    }

    public bool net_send(object data, Socket s,byte type)
    {
        byte[] send_data = ObjToByte(data);
        //Debug.Log("Type : " + type + " Length: " + send_data.Length);
        switch (type) {
            case CS_CONNECT:
                if (send_data.Length != 116) {
                    Debug.Log("net_send 오류 byte array의 길이가 표준치와 다릅니다. 패킷의 길이를 확인하세요. 원래 116 현재 패킷 길이 "+ send_data.Length);
                    return false;
                }
                break;
            case CS_MOVE:
                if (send_data.Length != 89)
                {
                    Debug.Log("net_send 오류 byte array의 길이가 표준치와 다릅니다. 패킷의 길이를 확인하세요.원래 89 현재 패킷 길이 " + send_data.Length);
                    return false;
                }
                break;
            case CS_BTN:
                if (send_data.Length != 89)
                {
                    Debug.Log("net_send 오류 byte array의 길이가 표준치와 다릅니다. 패킷의 길이를 확인하세요.원래 89 현재 패킷 길이 " + send_data.Length);
                    return false;
                }
                break;
            case SC_CONNECT:
                if (send_data.Length != 85)
                {
                    Debug.Log("net_send 오류 byte array의 길이가 표준치와 다릅니다. 패킷의 길이를 확인하세요.원래 85 현재 패킷 길이 " + send_data.Length);
                    return false;
                }
                break;
            case SC_CHARACTERINFO:
                if (send_data.Length != 199)
                {
                    Debug.Log("net_send 오류 byte array의 길이가 표준치와 다릅니다. 패킷의 길이를 확인하세요.원래 199 현재 패킷 길이 " + send_data.Length);
                    return false;
                }
                break;
            case SC_CHARACTERINFOSET:
                if (send_data.Length != 404)
                {
                    Debug.Log("net_send 오류 byte array의 길이가 표준치와 다릅니다. 패킷의 길이를 확인하세요.원래 404 현재 패킷 길이 " + send_data.Length);
                    return false;
                }
                break;
            case SC_SKILLSET:
                if (send_data.Length != 80)
                {
                    Debug.Log("net_send 오류 byte array의 길이가 표준치와 다릅니다. 패킷의 길이를 확인하세요.원래 80 현재 패킷 길이 " + send_data.Length);
                    return false;
                }
                break;
            case SC_SCENECHANGE:
                if (send_data.Length != 91)
                {
                    Debug.Log("net_send 오류 byte array의 길이가 표준치와 다릅니다. 패킷의 길이를 확인하세요.원래 91 현재 패킷 길이 " + send_data.Length);
                    return false;
                }
                break;
            case CS_SKILL:
                if (send_data.Length != 104) {
                    Debug.Log("net_send 오류 byte array의 길이가 표준치와 다릅니다. 패킷의 길이를 확인하세요.원래 98 현재 패킷 길이 " + send_data.Length);
                    return false;
                }
                break;
            case CS_UPGRADE:
                if (send_data.Length != 85)
                {
                    Debug.Log("net_send 오류 byte array의 길이가 표준치와 다릅니다. 패킷의 길이를 확인하세요.원래 98 현재 패킷 길이 " + send_data.Length);
                    return false;
                }
                break;
            default:
                return false;
        }
        if (send_data == null) return false;
        try
        {
            s.Send(send_data);
        }
        catch {
            Debug.Log("Send Fail");
        }
        
       // Debug.Log("type : "+type+" send complete\n"+s.RemoteEndPoint.ToString());
        return true;
    }

    public char net_recv_signal(Socket s)
    {
        byte[] Buf = new byte[1];

        int val = s.Receive(Buf);
        if (val == 0) return (char)125;
        char recv_signal = Convert.ToChar(Buf[0]);
        return recv_signal;
    }

    public object net_recv(Socket s,byte type)
    {
        byte[] Buf = new byte[MAXBUFFERSIZE];

        int val = s.Receive(Buf);
        if (val == 0) return null;

        object obj = ByteToObj(Buf);
        if (obj == null) return null;
        return obj;
    }
}

[Serializable]
public struct CS_CONNECT_PACKET
{
    public char id;
    public byte namelength;
    public string nickname;
}

[Serializable]
public struct SC_CONNECT_PACKET
{
    public char id;
    public char color;
}

[Serializable]
public struct CS_MOVE_PACKET
{
    public char id;
    public float x, y;
}

[Serializable]
public struct CS_BUTTON_PACKET
{
    public char id;
    public char btn_number;
}

[Serializable]
public struct SC_SCENE_CHANGE_PACKET
{
    public char scene_number;
}

[Serializable]
public struct MONSTERINFO
{
    public short m_hp;
    public short m_movespd;
    public char[] m_ability;
    public char m_mana;
    public short m_IRate;
    public char m_grade;
}

[Serializable]
public struct SC_CHARACTERINFO_PACKET
{
    public char id;
    public char ch_type;
    public char ch_atk;  //a
    public char ch_str;  //s
    public char ch_vit;  //v
    public char ch_int;  //i
    public char ch_mid;
    public char ch_matk;
    public short ch_movespd;
    public float ch_atkspd;
    public char ch_stamina;
}

[Serializable]
public struct SC_CHARACTERINFOSET_PACKET
{
    public SC_CHARACTERINFO_PACKET[] characterinfo;
}

[Serializable]
public struct CS_SKILLSET_PACKET
{
    public byte[] sk_id;
    public byte id;
}

[Serializable]
public struct CS_UPGRADE_PACKET {
    public byte id;
    public byte up_sg;
}