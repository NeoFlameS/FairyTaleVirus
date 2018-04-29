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

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    /*public byte[] ToByteTest(char obj)
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
    }*/

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
        Debug.Log(type);
        s.Send(send_signal);
        return true;
    }

    public bool net_send(object data, Socket s,byte type)
    {
        byte[] send_data = ObjToByte(data);
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
            default:
                return false;
        }
        if (send_data == null) return false;
        s.Send(send_data);
        Debug.Log("type : "+type+" send complete");
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
    public char ch_atk;
    public char ch_str;
    public char ch_vit;
    public char ch_int;
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
public struct SC_SKILLSET_PACKET
{
    public char[] sk_id;
}