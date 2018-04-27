using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using UnityEngine.SceneManagement;


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
    static byte[] Buffer { get; set; }
    int byteRead;


    // Use this for initialization
    void Start () {
        NetFunc = this.GetComponent<NetworkController>();
        state = 0;
        name = "No Name";
        ip = "Wrong IP";
        My_Info.id = 'e';
        My_Info.color = 'n';
        DontDestroyOnLoad(gameObject);
        

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
        name = namefd.text;//예외 처리 필요
        if (Ipfd.text.Length < 7 || Ipfd.text.Length > 16)
        {
            Debug.Log("IP input ERROR");
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
        Buffer = new byte[sck.SendBufferSize];

        //시그널 수신
        NetFunc.net_recv_signal(sck);

        //플레이어 정보 수신
        
        SC_CONNECT_PACKET SCpack = new SC_CONNECT_PACKET();
        object obj;
        obj = NetFunc.net_recv(sck);
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
        NetFunc.net_send(CSpacket,sck);

       

        return 1;
    }

    public int GameDataSend(object o,byte s) {//게임 진행 중 데이터 송신 메소드

        NetFunc.net_send_signal(s,sck);
        NetFunc.net_send(o, sck);

        return 0;
    }
   
    public object GameDataRecv()
    {//게임 진행 중 데이터 수신 메소드
        object obj;
        char type;

        type = NetFunc.net_recv_signal(sck);
        obj = NetFunc.net_recv(sck);

        return obj; 
    }
}
