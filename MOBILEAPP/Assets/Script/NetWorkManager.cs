using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using UnityEngine.SceneManagement;
using System;


public class NetWorkManager : MonoBehaviour {
    //UI 변수
    public InputField Ipfd;
    public InputField namefd;
    public Button OK;
    //소켓 연결 정보
    int state;
    string name;
    string ip;

    public SC_CONNECT_PACKET My_Info;
    
    //소켓 관련 변수
    NetworkController NetFunc;
    Socket sck;
    byte[] data;
    byte[] Buffer = new byte[NetworkController.MAXBUFFERSIZE];
    int byteRead;

    CMessageResolver PacketReader;
    // Use this for initialization
    void Start () {
        NetFunc = this.GetComponent<NetworkController>();
        state = 0;
        name = "No Name";
        ip = "Wrong IP";
        My_Info.id = 'e';
        My_Info.color = 'n';
        PacketReader = new CMessageResolver();
        DontDestroyOnLoad(gameObject);

        //Packet 크기 테스트용 지울것
        /*
        CS_CONNECT_PACKET csc;
        csc.id = '1';
        csc.nickname = "NeoFlamell";
        csc.namelength = (byte)csc.nickname.Length;
        SC_CONNECT_PACKET scc;
        scc.id = '1';
        scc.color = 'r';
        CS_MOVE_PACKET csm;
        csm.x = (float)1.1;
        csm.y = (float)1.1;
        csm.id = '1';
        CS_BUTTON_PACKET csb;
        csb.id = '1';
        csb.btn_number = '3';

        SC_SCENE_CHANGE_PACKET sscp = new SC_SCENE_CHANGE_PACKET();
        MONSTERINFO mi = new MONSTERINFO();
        SC_CHARACTERINFO_PACKET scip= new SC_CHARACTERINFO_PACKET();
        SC_CHARACTERINFOSET_PACKET sccp;
        SC_SKILLSET_PACKET ssp = new SC_SKILLSET_PACKET();
        sccp.characterinfo = new SC_CHARACTERINFO_PACKET[4];

        Debug.Log("CS_CONNET_PACKET SIZE : "+NetFunc.ObjToByte(csc).Length+" String "+csc.namelength);
        Debug.Log("SC_CONNECT_PACKET : " + NetFunc.ObjToByte(scc).Length);
        Debug.Log("CS_MOVE_PACKET : " + NetFunc.ObjToByte(csm).Length);
        Debug.Log("CS_BUTTON_PACKET : " + NetFunc.ObjToByte(csb).Length);

        Debug.Log("SC_SCENE_CHANGE_PACKET : " + NetFunc.ObjToByte(sscp).Length);
        Debug.Log("MONSTERINFO : " + NetFunc.ObjToByte(mi).Length);
        Debug.Log("SC_CHARACTERINFO_PACKET : " + NetFunc.ObjToByte(scip).Length);
        Debug.Log("SC_CHARACTERINFOSET_PACKET : "+NetFunc.ObjToByte(sccp).Length);
        Debug.Log("SC_SKILLSET_PACKET : " + NetFunc.ObjToByte(ssp).Length);*/


    }

    // Update is called once per frame
    void FixedUpdate () {
        int res;
        if (state == 1)
        {
            res =ConnectToPC();
            state = 0;
            if (res == 1) {
                SceneManager.LoadScene("Connected");
            }
        }

	}

    void SetNetwork() {
        state = 1;
        //name = namefd.text;//예외 처리 필요

        if (namefd.text.Length > 10)
        {
            name = namefd.text.Substring(0, 10);
            Debug.Log("문자 길이 : " + name.Length + " name : " + name);
        }
        else if (namefd.text.Length < 10)
        {
            name = namefd.text;
            while (name.Length<10) {
                name = name.Insert(name.Length, "*");
            }
            
        }
        else {
            name = namefd.text;
        }


        if (Ipfd.text.Length < 7 || Ipfd.text.Length > 16)
        {
            Debug.Log("IP input ERROR");
            state = 0;
            return;
        }
        else {
            ip = Ipfd.text;
        }
    }

    public void PressButton()
    {
        SetNetwork();
    }

    private int ConnectToPC()
    {
        //소켓 설정
        sck = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(ip), 8080);

        //Connect
        try
        {
            sck.Connect(localEndPoint);
        }
        catch
        {
            Debug.Log("Unable to connect to remote end point!\r\n");
            return 0;
        }
        Debug.Log("Perfect!!");
        //Buffer = new byte[NetworkController.MAXBUFFERSIZE];//sck.socketbuffersize 에서 변경

        //시그널 수신
        char s;
        NetFunc.net_recv_signal(sck);

        //플레이어 정보 수신
        
        SC_CONNECT_PACKET SCpack = new SC_CONNECT_PACKET();
        object obj;
        obj = NetFunc.net_recv(sck, NetworkController.SC_CONNECT);
        if (obj == null) return 0;//error
        SCpack = (SC_CONNECT_PACKET)obj; 

        Debug.Log((int)SCpack.id + " " + SCpack.color);
        My_Info = SCpack;
        //시그널 전송
        NetFunc.net_send_signal(NetworkController.CS_CONNECT,sck);



        //닉네임 전송
        CS_CONNECT_PACKET CSpacket= new CS_CONNECT_PACKET();
        CSpacket.nickname = name;
        CSpacket.id = SCpack.id;

        
        Buffer = NetFunc.ObjToByte(CSpacket);
        NetFunc.net_send(CSpacket,sck,NetworkController.CS_CONNECT);

        //비동기 시작
        IAsyncClient sync_obj = new IAsyncClient();
        sync_obj.sck = sck;

        Buffer = new byte[NetworkController.MAXBUFFERSIZE];
        sck.BeginReceive(Buffer,0,NetworkController.MAXBUFFERSIZE,0,new AsyncCallback(ReciveCallBack),sync_obj);

        return 1;
    }

    public int GameDataSend(object o,byte s) {//게임 진행 중 데이터 송신 메소드

        NetFunc.net_send_signal(s,sck);
        NetFunc.net_send(o, sck, s);

        return 0;
    }
   
    public object GameDataRecv()
    {//게임 진행 중 데이터 수신 메소드
        object obj;
        char type;

        type = NetFunc.net_recv_signal(sck);
        obj = NetFunc.net_recv(sck,(byte)type);

        return obj; 
    }


    void ReciveCallBack(IAsyncResult ar) {
        IAsyncClient obj = (IAsyncClient)ar.AsyncState;
        Debug.Log("Receive Message");

        if (obj.recv_signal == 99)
        {
            PacketReader.OnReceive(obj.recvbuf, 0, 1);
        }
        else {
            switch (obj.recv_signal)
            {//받는 부분 미구현
                case NetworkController.SC_CHARACTERINFO:
                    
                    break;
                case NetworkController.SC_CHARACTERINFOSET:
                    
                    break;
                case NetworkController.SC_SKILLSET:
                    
                    break;
                case NetworkController.SC_SCENECHANGE:
                    
                    break;
                default:
                    Debug.Log("Recv Type Error : Packet Type " + (char)obj.recv_signal);
                    break;
            }
        }


        obj.sck.BeginReceive(Buffer, 0, NetworkController.MAXBUFFERSIZE, 0, new AsyncCallback(ReciveCallBack), ar);
    }

    
}


public class IAsyncClient {
    public Socket sck;
    public byte[] recvbuf = new byte[NetworkController.MAXBUFFERSIZE];
    public byte recv_signal;
    public string nickname;

    public IAsyncClient() {
        recv_signal = 99;
    }
}

class Defines
{
    // 생성자에서 한번 더 값을 변경할 수 있는 값
    public static readonly short HANDERSIZE = 2;
}

// <summary>
// [header][body] 구조를 갖는 데이터를 파싱하는 클래스.
// - header :  데이터 사이즈. Defines.HEADERSIZE에 정의된 타입만큼의 크기를 갖는다.
//                2바이트일 경우 Int16, 4바이트는 Int32로 처리하면 된다.
///               본문의 크기가 Int16.Max값을 넘지 않는다면 2바이트로 처리하는것이 좋을것 같다.
// - body : 메시지 본문.
// </summary>
class CMessageResolver
{
    //public delegate void CompleteMessageCallBack(Const<byte[]> buffer);

    // 메세지 사이즈
    int MessageSize;
    //진행중인 버퍼
    byte[] MessageBuffer = new byte[NetworkController.MAXBUFFERSIZE];

    // 현재 진행중인 버퍼의 인덱스를 가르키는 변수
    // 패킷 하나를 완성한 뒤에는 0으로 초기화 시켜주어야 한다.
    int CurrentPosition;
    //읽어와야 할 목표 위치
    int PositionToRead;
    //남은 사이즈
    int RemainBytes;

    public CMessageResolver()
    {
        this.MessageSize = 0;
        this.CurrentPosition = 0;
        this.PositionToRead = 0;
        this.RemainBytes = 0;
    }

    // <summary>
    // 목표지점으로 설정된 위치까지의 바이트를 원본 버퍼로부터 복사한다.
    // 데이터가 모자랄 경우 현재 남은 바이트 까지만 복사한다.
    // </summary>
    // <param name="buffer"></param>
    //<param name="offset"></param>
    // <param name="transffered"></param>
    // <param name="size_to_read"></param>
    // <returns>다 읽었으면 true, 데이터가 모자라서 못 읽었으면 false를 리턴한다.</returns>

    bool readUntil(byte[] buffer, ref int srcPosition, int offset, int treansffered)
    {
        if (this.CurrentPosition >= offset + treansffered)
        {
            // 들어온 데이터 만큼 다 읽은 상태이므로 더이상 읽을 데이터가 없다.
            return false;
        }

        // 읽어와야 할 바이트
        // 데이터가 분리되어 올 경우 이전에 읽어놓을 값을 빼줘서 부족한 만큼 읽어올 수 있도록 계산해줌.
        int copySize = this.PositionToRead - this.CurrentPosition;

        // 남은 데이터가 더 적다면 가능한 만큼 복사한다!
        if (this.RemainBytes < copySize)
        {
            copySize = this.RemainBytes;
        }

        // 버퍼에 복사
        Array.Copy(buffer, srcPosition, this.MessageBuffer, this.CurrentPosition, copySize);

        // 원본 버퍼 포지션 이동
        srcPosition += copySize;
        // 타켓 버퍼 포지션 이동
        this.CurrentPosition += copySize;
        // 남은 바이트 수
        this.RemainBytes -= copySize;

        // 목표 지점에 도달 못했다면 false
        if (this.CurrentPosition < this.PositionToRead)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 소켓 버퍼로부터 데이터를 수신할 때 마다 호출된다.
    /// 데이터가 남아 있을 때 까지 계속 패킷을 만들어 callback을 호출 해 준다.
    /// 하나의 패킷을 완성하지 못했다면 버퍼에 보관해 놓은 뒤 다음 수신을 기다린다.
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="offset"></param>
    /// <param name="transffered"></param>
    public void OnReceive(byte[] buffer, int offset, int treansffered /*,CompleteMessageCallBack callback*/)
    {
        // 이번 receive로 읽어오게 될 바이트 수.
        this.RemainBytes = treansffered;

        // 원본 버퍼의 포지션값.
        // 패킷이 여러개 뭉쳐 올 경우 원본 버퍼의 포지션은 계속 앞으로 가야 하는데 그 처리를 위한 변수이다.
        int srcPosition = offset;

        // 남은 데이터가 있다면 계속 반복한다.
        while (this.RemainBytes > 0)
        {
            bool completed = false;

            // 헤더만큼 못읽은 경우 헤더를 먼저 읽는다.
            if (this.CurrentPosition < Defines.HANDERSIZE)
            {
                // 목표 지점 설정( 헤더 위치까지 도달하도록 설정 )
                this.PositionToRead = Defines.HANDERSIZE;

                completed = readUntil(buffer, ref srcPosition, offset, treansffered);
                if (!completed)
                {
                    // 아직 다 못읽었으므로 다음 receive를 기다린다.
                    return;
                }

                // 헤더 하나를 온전히 읽어왔으므로 메세지 사이즈를 구한다.
                this.MessageSize = getBodySize();
                // 다음 목표 지점( 헤더 + 메세지 사이즈 )
                this.PositionToRead = this.MessageSize + Defines.HANDERSIZE;
            }


            // 메세지를 읽는다
            completed = readUntil(buffer, ref srcPosition, offset, treansffered);

            if (completed)
            {
                // 패킷 하나를 완성
                //callback(new Const<byte[]>(this.MessageBuffer));
                clearBuffer();
            }
        }
    }

    int getBodySize()
    {
        // 헤더 타입의 바이트만큼을 읽어와 메세지 사이즈를 리턴함
        Type type = Defines.HANDERSIZE.GetType();

        if (type.Equals(typeof(Int16)))
        {
            return BitConverter.ToInt16(this.MessageBuffer, 0);
        }
        return BitConverter.ToInt32(this.MessageBuffer, 0);
    }
    void clearBuffer()
    {
        Array.Clear(this.MessageBuffer, 0, this.MessageBuffer.Length);

        this.CurrentPosition = 0;
        this.MessageSize = 0;
    }
}
/*
public struct Const<T>
{
    public T Value { get; private set; }

    public Const(T value)
        : this()
    {
        this.Value = value;
    }
}*/


