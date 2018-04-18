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

    //라운드 정보 = 캐릭터 이동명령(포인터 위치) + 몬스터 위치 + 오염도 수치 + 공유자원 따로 보냄
    //char -> byte 변경

    void Start(){
		DontDestroyOnLoad (gameObject);
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
        Debug.Log(type);
		s.Send(send_signal);
		return true;
	}

	public bool net_send(object data, Socket s)
	{
		byte[] send_data = ObjToByte(data);
		if (send_data == null) return false;
		s.Send(send_data);

		return true;
	}

	public char net_recv_signal(Socket s)
	{
		byte[] Buf = new byte[MAXBUFFERSIZE];

		int val = s.Receive(Buf);
		if (val == 0) return (char)125;
		char recv_signal = Convert.ToChar(Buf[0]);
		return recv_signal;
	}

	public object net_recv(Socket s)
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
public struct CS_CONNECT_PACKET {
	public char id;
	public string nickname;
}

[Serializable]
public struct SC_CONNECT_PACKET {
	public char id;
	public char color;
}

[Serializable]
public struct CS_MOVE_PACKET {
	public char id;
	public Vector2 movevector;
}

[Serializable]
public struct CS_BUTTON_PACKET {
	public char id;
	public 	char btn_number;
}

[Serializable]
public struct SC_SCENE_CHANGE_PACKET {
	public char scene_number;
}

[Serializable]
public 	struct MONSTERINFO {
	public 	short m_hp;
	public short m_movespd;
	public char[] m_ability;
	public char m_mana;
	public short m_IRate;
	public char m_grade;
}

[Serializable]
public 	struct SC_CHARACTERINFO_PACKET {
	public byte id;
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
}

[Serializable]
public struct SC_CHARACTERINFOSET_PACKET{
	public SC_CHARACTERINFO_PACKET[] characterinfo;
}

[Serializable]
public 	struct SC_SKILLSET_PACKET {
	public byte[] sk_id;
}

[Serializable]
public struct PP_CONNECT_PACKET
{
    public byte                      userNumber;
    public byte                      id; // for reconnect
    public SC_CHARACTERINFO_PACKET[] characterInfo;
}

[Serializable]
public struct PP_SKILLSET_PACKET
{
    public byte[][] sk_id;
}

[Serializable]
public struct PS_RECONNECT_PACKET
{
    public byte id;
    public byte userNumber;
}

[Serializable]
public struct SP_RECONNECT_PACKET
{
    public bool ack;
    public byte id;
    public byte userNumber;
    public SC_CHARACTERINFO_PACKET[] characterInfo;
    public byte[][] sk_id;
}



/*  public const byte SS_MONSTERSETINFO = 23; // 라운드 시작시
    public const byte SS_MONSTERINFO = 24; // 게임 도중
    public const byte SS_PLAYERINFO = 25; // 이동
    public const byte SS_LOBYSTATE = 26;
    public const byte SS_INGAMESTATE = 27; // 라운드 
    public const byte SS_SCENEMOVE = 28;
    public const byte SS_ENDGAME = 29;
    public const byte SS_WINGAME = 30;
*/
