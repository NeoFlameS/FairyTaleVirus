using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;


//PC연결도 5번째 소켓으로 같이 수행
public class MobileNetwork : MonoBehaviour {
	NetworkController NC;
	CursorControl CC;
    Socket sck;
    Socket[] s_arr;//5.15 홍승준 추가
    List<IAsyncClient> clients = new List<IAsyncClient>();
    MainGameSystem GS;

    List<Socket> client_sock = new List<Socket>();
    volatile bool[] connected;

    bool isgamescene = false;

    //5.19 홍승준 추가
    SelectManager sm;
    ManaSystem Ms;

    public void OnLevelWasLoaded()
    {
        if (7 != SceneManager.GetActiveScene().buildIndex) return;
        isgamescene = true;
        GS = GameObject.Find("GAME SYSTEM").GetComponent<MainGameSystem>();
    }
    
    // Use this for initialization
    void Start ()
    {
        s_arr = new Socket[4];
        connected = new bool[4] { false, false, false, false };
        NC = this.GetComponent<NetworkController>();
        DontDestroyOnLoad (gameObject);
        if (null == CC) CC = GameObject.Find("Cursor Manager(Clone)").GetComponent<CursorControl>();
        
        sck = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		sck.Bind (new IPEndPoint(IPAddress.Any ,8080));
        sck.Listen(100);

        client_sock.Clear();

        sck.BeginAccept(new AsyncCallback(AcceptCallback), sck);
    }

    public void AcceptCallback(IAsyncResult ar)
    {        
        Socket listener = (Socket)ar.AsyncState;
        Socket handler = listener.EndAccept(ar);
        // 받는 방식의 변화 재접속...
        int id = 0;
        for (int i = 0; i < 4; ++i) {
            if (false == connected[i]) {
                id = i;
                break;
            } 
        }
        if (client_sock.Count >= NetworkController.MAX_CONNECT)
        {
            handler.Disconnect(false);
        }
        client_sock.Add(handler);

        if (false == isgamescene) CC.connected(id);
        else GS.reconnected(id);

        NC.net_send_signal(NetworkController.SC_CONNECT, handler);
        SC_CONNECT_PACKET packet = new SC_CONNECT_PACKET();
        packet.id = (char)id;
        switch (id)
        {
            case 0:
                packet.color = 'b';
                break;
            case 1:
                packet.color = 'y';
                break;
            case 2:
                packet.color = 'r';
                break;
            case 3:
                packet.color = 'g';
                break;
            default:
                //error
                break;
        }
        Debug.Log("Player Color : "+packet.color);
        NC.net_send(packet, handler, NetworkController.SC_CONNECT);
        
        connected[id] = true;

        IAsyncClient iaclient = new IAsyncClient();
        iaclient.s = handler;
        iaclient.id = id;
        iaclient.recvbyte = 0;
        clients.Add(iaclient);
        handler.BeginReceive(iaclient.recvbuf, 0, NetworkController.MAXBUFFERSIZE, 0, new AsyncCallback(DataReceive), iaclient);
        sck.BeginAccept(new AsyncCallback(AcceptCallback), listener);

        
        s_arr[id] = handler;//5.15 홍승준 추가
    }

    //5.15 홍승준 추가
    public void SignalSend(int id, byte type) {
        NC.net_send_signal(type,s_arr[id]);
    }
    //5.15 끝

    //받은 데이터 체크 꼭 할것!!!!
    public void DataReceive(IAsyncResult ar)
    {
        IAsyncClient obj = (IAsyncClient)ar.AsyncState;
        Socket handler = obj.s;

        int bytesRead = handler.EndReceive(ar);
        obj.recvbyte += bytesRead;

        //길이 맞는지 확인해보고
        if (obj.recvbyte > 0 || bytesRead > 0)
        {
            Debug.Log("DataRecived");
            if (true == obj.signalread)
            {
                //5.14 홍승준 수정
                obj.recv_signal = obj.recvbuf[0];
                Buffer.BlockCopy(obj.recvbuf, 1, obj.recvbuf, 0, obj.recvbyte - 1);

                switch (obj.recv_signal)
                {
                    case NetworkController.CS_REQCHR:
                        //캐릭터 정보 요청 신호 일 때
                        Debug.Log("Requset signal");
                        SC_CHARACTERINFOSET_PACKET chset = new SC_CHARACTERINFOSET_PACKET();
                        chset.characterinfo = new SC_CHARACTERINFO_PACKET[4];

                        int i = 0;
                        for (i = 0; i < 4; i++) {
                            if (connected[i])
                            {
                                chset.characterinfo[i] = GS.PC[i].CharacterInfo();
                                chset.characterinfo[i].id = (byte)i;
                            }
                            else {
                                chset.characterinfo[i].id = 125;
                            }
                        }

                        NC.net_send_signal(NetworkController.SC_CHARACTERINFOSET, obj.s);
                        NC.net_send(chset,obj.s, NetworkController.SC_CHARACTERINFOSET);
                        
                        Debug.Log("Ch_info_set_packet : "+NC.ObjToByte(chset).Length);
                        //다시 시그널 리시브 하도록 초기화
                        obj.recvbyte -= 1;
                        obj.signalread = true;

                        break;
                    default:
                        obj.recvbyte -= 1;
                        obj.signalread = false;
                        break;
                }
            }
            //5.14 홍승준 수정 끝
            if (false == obj.signalread)
            {
                if (obj.recv_signal == NetworkController.CS_DISCONNECT)
                {
                    client_sock.Remove(obj.s);
                    obj.s.Disconnect(false);

                    if (false == isgamescene) CC.disconnected(obj.id);
                    else GS.disconnected(obj.id);
                    return;
                }

                else if (obj.recv_signal == NetworkController.CS_CONNECT && obj.recvbyte >= 116)
                {
                    Buffer.BlockCopy(obj.recvbuf, 0, obj.cbuf, 0, 116);
                    object recvobj = NC.ByteToObj(obj.cbuf);
                    Buffer.BlockCopy(obj.recvbuf, 116, obj.recvbuf, 0, obj.recvbyte - 116);
                    obj.recvbyte -= 116;
                    CS_CONNECT_PACKET res = (CS_CONNECT_PACKET)recvobj;

                    CC.nickname[res.id] = res.nickname;
                    obj.recv_signal = NetworkController.S_NULL;
                    obj.signalread = true;
                }

                else if (obj.recv_signal == NetworkController.CS_MOVE && obj.recvbyte >= 89)
                {
                    Buffer.BlockCopy(obj.recvbuf, 0, obj.cbuf, 0, 89);
                    object recvobj = NC.ByteToObj(obj.cbuf);
                    Buffer.BlockCopy(obj.recvbuf, 89, obj.recvbuf, 0, obj.recvbyte - 89);
                    obj.recvbyte -= 89;
                    CS_MOVE_PACKET res = (CS_MOVE_PACKET)recvobj;

                    if (false == isgamescene) CC.move(res.x, res.y, res.id);
                    else GS.move(res.x, res.y, res.id);
                    obj.recv_signal = NetworkController.S_NULL;
                    obj.signalread = true;

                }
                else if (obj.recv_signal == NetworkController.CS_BTN && obj.recvbyte >= 89)
                {
                    Buffer.BlockCopy(obj.recvbuf, 0, obj.cbuf, 0, 89);
                    object recvobj = NC.ByteToObj(obj.cbuf);
                    Buffer.BlockCopy(obj.recvbuf, 89, obj.recvbuf, 0, obj.recvbyte - 89);
                    obj.recvbyte -= 89;
                    CS_BUTTON_PACKET res = (CS_BUTTON_PACKET)recvobj;

                    if (false == isgamescene) CC.click(res.id, res.btn_number);
                    else GS.click(res.id, res.btn_number);
                    obj.recv_signal = NetworkController.S_NULL;
                    obj.signalread = true;
                }//5.15 홍승준 추가//5.18 홍승준 수정
                else if (obj.recv_signal == NetworkController.CS_SKILL && obj.recvbyte >= 104) {
                    Buffer.BlockCopy(obj.recvbuf, 0, obj.cbuf, 0, 104);
                    object recvobj = NC.ByteToObj(obj.cbuf);
                    Buffer.BlockCopy(obj.recvbuf, 104, obj.recvbuf, 0, obj.recvbyte - 104);
                    obj.recvbyte -= 104;

                    CS_SKILLSET_PACKET sc = (CS_SKILLSET_PACKET)recvobj;
                    sm.MakeCharacter(0, sc.id, sc);
                    obj.recv_signal = NetworkController.S_NULL;
                    obj.signalread = true;
                    /*
                    Debug.Log("id : "+ Convert.ToString(sc.sk_id));
                    Debug.Log(" 1 : "+(short)sc.sk_id[0]);
                    Debug.Log(" 2 : " + (short)sc.sk_id[1]);
                    Debug.Log(" 3 : " + (short)sc.sk_id[2]);
                    Debug.Log(" 4 : " + (short)sc.sk_id[3]);*/
                }//여기까지
                else if (obj.recv_signal == NetworkController.CS_UPGRADE && obj.recvbyte >= 85) {
                    Debug.Log("UPGRADE RECEIVE");
                    Buffer.BlockCopy(obj.recvbuf, 0, obj.cbuf, 0, 85);
                    object recvobj = NC.ByteToObj(obj.cbuf);
                    Buffer.BlockCopy(obj.recvbuf, 85, obj.recvbuf, 0, obj.recvbyte - 85);
                    obj.recvbyte -= 85;

                    Ms.Upgrade_reciev(recvobj);
                    obj.recv_signal = NetworkController.S_NULL;
                    obj.signalread = true;
                }
            }
        }

            
        handler.BeginReceive(obj.recvbuf, obj.recvbyte, NetworkController.MAXBUFFERSIZE - obj.recvbyte, 0, new AsyncCallback(DataReceive), obj);
    }

    public void Get_SelectManger() {
        sm=GameObject.Find("Select Manager(Clone)").GetComponent<SelectManager>();
    }

    public void Get_ManaSystem() {
        Ms = GameObject.Find("ManaSystem").GetComponent<ManaSystem>();
    }
}


public class IAsyncClient {
    public Socket s = null;
    public byte[] recvbuf = new byte[NetworkController.MAXBUFFERSIZE];
    public byte[] cbuf = new byte[NetworkController.MAXBUFFERSIZE];
    public int recvbyte;
    public byte recv_signal;
    public string nickname;
    public bool signalread = true;
    public int id;
}