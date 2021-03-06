﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

//using System.Buffer;


public class NetWorkManager : MonoBehaviour {

    public Text skill_recv;

    //스킬 셀렉트 변수
    public byte My_type=126;
    //캐릭터 변수
    public SC_CHARACTERINFOSET_PACKET scinf;
    public bool recv_charinfo=false;
    //UI 변수
    public InputField Ipfd;
    public InputField namefd;
    public Button OK;
    bool select_scene;//캐릭터 셀렉트
    public bool skill_select;//스킬 셀렉트
    public bool in_game;
    //소켓 연결 정보
    int state;
    string name;
    string or_name;
    string ip;

    OptionSet last;

    public SC_CONNECT_PACKET My_Info;

    bool byte_recv = false;
    //소켓 관련 변수
    public NetworkController NetFunc;
    Socket sck;
    byte[] data;
    byte[] buffer = new byte[NetworkController.MAXBUFFERSIZE];
    int byteRead;

    Cursor cursor;
    
    void Start() {
        last = new OptionSet();

        NetFunc = this.GetComponent<NetworkController>();
        state = 0;
        name = "No Name";
        ip = "Wrong IP";
        My_Info.id = 'e';
        My_Info.color = 'n';
        
        DontDestroyOnLoad(gameObject);

        select_scene = false;
        skill_select = false;
        in_game = false;
        
        SC_TYPE_PACKET stp = new SC_TYPE_PACKET();
        stp.type = 1;

        Debug.Log("CS_CAMERA_PACKET : " + NetFunc.ObjToByte(stp).Length);
        Load_Data();
    }

    // Update is called once per frame
    void FixedUpdate() {
        int res;
        if (state == 1)
        {
            res = ConnectToPC();
            state = 0;
            if (res == 1) {
                SceneManager.LoadScene("Connected");
            }
        }

        if (select_scene) {//캐릭터 셀렉트 씬
            select_scene = false;
            SceneManager.LoadScene("CharacterSelect");//캐릭터 셀렉트 씬 으로 변경할것
        }

        /*if (skill_select) {
            
            skill_select = false;
            SceneManager.LoadScene("SelectSkill");
        }*/
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
            or_name = name;
            while (name.Length < 10) {
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

        OverLaped_Data(ip, or_name);//연결 데이터 저장

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
        //char s;
        NetFunc.net_recv_signal(sck);

        //플레이어 정보 수신

        SC_CONNECT_PACKET SCpack = new SC_CONNECT_PACKET();
        object obj;
        obj = NetFunc.net_recv(sck, NetworkController.SC_CONNECT);
        if (obj == null) return 0;//error
        SCpack = (SC_CONNECT_PACKET)obj;

       // Debug.Log((int)SCpack.id + " " + SCpack.color);
        My_Info = SCpack;
        //시그널 전송
        NetFunc.net_send_signal(NetworkController.CS_CONNECT, sck);



        //닉네임 전송
        CS_CONNECT_PACKET CSpacket = new CS_CONNECT_PACKET();
        CSpacket.nickname = name;
        CSpacket.id = SCpack.id;


        buffer = NetFunc.ObjToByte(CSpacket);
        NetFunc.net_send(CSpacket, sck, NetworkController.CS_CONNECT);
        sck.NoDelay=true;
        //비동기 시작
        IAsyncClient sync_obj = new IAsyncClient();
        sync_obj.sck = sck;

        buffer = new byte[NetworkController.MAXBUFFERSIZE];
        sck.BeginReceive(sync_obj.recvbuf, 0, NetworkController.MAXBUFFERSIZE, 0, new AsyncCallback(ReciveCallBack), sync_obj);

        return 1;
    }

    public int GameDataSend(object o, byte s) {//게임 진행 중 데이터 송신 메소드

        NetFunc.net_send_signal(s, sck);
        NetFunc.net_send(o, sck, s);

        return 0;
    }

    public void SendRequestSignal(byte type) {
        NetFunc.net_send_signal(type,sck);
        Debug.Log("Request");
    }
    
    void ReciveCallBack(IAsyncResult ar) {
        IAsyncClient obj = (IAsyncClient)ar.AsyncState;
        Socket handler = obj.sck;

        Debug.Log("Receive Message");
       
        int bytesRead = handler.EndReceive(ar);
        obj.recvbyte += bytesRead;
        
        if (obj.recvbyte > 0 || bytesRead > 0) {
            if (true == obj.signalread)
            {
                byte befor_signal=obj.recv_signal;
                obj.recv_signal = obj.recvbuf[0];
                Buffer.BlockCopy(obj.recvbuf, 1, obj.recvbuf, 0, obj.recvbyte - 1);

                if (obj.recv_signal == NetworkController.SC_SELECT)
                {
                    select_scene = true;
                    obj.recvbyte -= 1;
                    obj.signalread = true;
                }
                else if (obj.recv_signal == NetworkController.SC_GAME_RESULT) {
                    in_game = false;
                    cursor.in_game = in_game;//차후 수정할것 이부분 필요없음 
                    obj.recvbyte -= 1;
                    obj.signalread = true;
                }
                else if (obj.recv_signal == NetworkController.SC_IN_GAME)
                {
                    in_game = true;
                    cursor.in_game = in_game;//차후 수정할것 이부분 필요없음 
                    obj.recvbyte -= 1;
                    obj.signalread = true;
                }
                else if (obj.recv_signal == NetworkController.CS_SKILL) {
                    skill_select = true;
                    Debug.Log("Skill_Signal_recived");
                    obj.recvbyte -= 1;
                    obj.signalread = true;
                }
                else
                {
                    obj.recvbyte -= 1;
                    obj.signalread = false;
                }
                
                Debug.Log(Convert.ToString(obj.recvbuf[0]) + " / " + Convert.ToString(obj.recv_signal));
                Debug.Log(obj.signalread);
            }
            else {
                if (obj.recv_signal == NetworkController.SC_CHARACTERINFOSET && obj.recvbyte >= 404)
                {
                    Buffer.BlockCopy(obj.recvbuf, 0, obj.cbuf, 0, 404);
                    object recvobj = NetFunc.ByteToObj(obj.cbuf);
                    Buffer.BlockCopy(obj.recvbuf, 404, obj.recvbuf, 0, obj.recvbyte - 404);
                    obj.recvbyte -= 404;


                    //캐릭터 인포 셋 수신
                    scinf = (SC_CHARACTERINFOSET_PACKET)recvobj;
                    recv_charinfo = true;

                    //Debug.Log("type : " + Convert.ToString(res.characterinfo[].ch_type) + " / vit : " + Convert.ToString(res.ch_vit));

                    obj.recv_signal = NetworkController.S_NULL;
                    obj.signalread = true;
                }
                else if (obj.recv_signal == NetworkController.CS_SKILL && obj.recvbyte >= 75)
                {
                    Buffer.BlockCopy(obj.recvbuf, 0, obj.cbuf, 0, 75);
                    object recvobj = NetFunc.ByteToObj(obj.cbuf);
                    Buffer.BlockCopy(obj.recvbuf, 75, obj.recvbuf, 0, obj.recvbyte - 75);
                    obj.recvbyte -= 75;

                    SC_TYPE_PACKET sc = (SC_TYPE_PACKET)recvobj;
                    My_type = sc.type;
                    Debug.Log("Type : " + Convert.ToString(sc.type));

                    //skill_select = true;
                    obj.recv_signal = NetworkController.S_NULL;
                    obj.signalread = true;
                }
            }
        }
        obj.sck.BeginReceive(obj.recvbuf, obj.recvbyte, NetworkController.MAXBUFFERSIZE-obj.recvbyte, 0, new AsyncCallback(ReciveCallBack), obj);
    }
    public void Get_CS() {
        cursor = GameObject.Find("Cursor").GetComponent<Cursor>();
    }

    void Load_Data()
    {
        try
        {
            var path = Application.persistentDataPath + "/LastData.xml";
            var serializer = new XmlSerializer(typeof(OptionSet));
            var stream = new FileStream(path, FileMode.Open);
            last = serializer.Deserialize(stream) as OptionSet;
            Ipfd.text = last.ip;
            namefd.text = last.name;
            stream.Close();
            return;
        }
        catch (Exception e)
        {
            //Debug.Log(e);
            Debug.Log(e.Message);
            return;
            //error;
        }

    }

    void Creat_Data() {//파일 생성
        XmlDocument xmlDoc = new XmlDocument();

        // Xml을 선언한다(xml의 버전과 인코딩 방식을 정해준다.)
        xmlDoc.AppendChild(xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "yes"));

        // 루트 노드 생성
        XmlNode root = xmlDoc.CreateNode(XmlNodeType.Element, "ConnectInfo", string.Empty);
        xmlDoc.AppendChild(root);

        // 자식 노드 생성
        XmlNode child = xmlDoc.CreateNode(XmlNodeType.Element, "Last", string.Empty);
        root.AppendChild(child);

        // 자식 노드에 들어갈 속성 생성
        XmlElement name = xmlDoc.CreateElement("Name");
        name.InnerText = "No Name";
        child.AppendChild(name);

        XmlElement Ip = xmlDoc.CreateElement("Ip");
        Ip.InnerText = "No Ip";
        child.AppendChild(Ip);

       

    }

    void OverLaped_Data(string ip, string name)//최종 데이터 덮어쓰기
    {
        

        last.name = name;
        last.ip = ip;
        try
        {
            var path = Application.persistentDataPath + "/LastData.xml";
            var serializer = new XmlSerializer(typeof(OptionSet));
            var stream = new FileStream(path, FileMode.Create);
            serializer.Serialize(stream, last);
            stream.Close();
        }
        catch (Exception e)
        {
            Debug.Log("저장 에러???");
            //error
        }
    }
}



public class IAsyncClient {
    public Socket sck=null;
    public int recvbyte;
    public byte[] recvbuf = new byte[NetworkController.MAXBUFFERSIZE];
    public byte[] cbuf = new byte[NetworkController.MAXBUFFERSIZE];
    public byte recv_signal;
    public string nickname;
    public bool signalread = true;
    public IAsyncClient() {
        recv_signal = 0;
    }
}

[XmlRoot("Option")]
public class OptionSet
{
    [XmlArrayItem("Name")]
    [XmlAttribute("name")]
    public string name;
    
    [XmlArrayItem("Ip")]
    [XmlAttribute("ip")]
    public string ip;

    public OptionSet() {
        name = "No Name";
        ip = "No Ip";
    }
}
