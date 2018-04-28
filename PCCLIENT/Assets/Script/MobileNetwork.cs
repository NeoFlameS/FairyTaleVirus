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

    public static ManualResetEvent allDone = new ManualResetEvent(false);

	NetworkController NC;
	CursorControl CC;
    Socket sck;
    List<IAsyncClient> clients = new List<IAsyncClient>();
    MainGameSystem GS;


    List<Socket> client_sock = new List<Socket>();
    volatile bool[] connected = new bool[4] {true, true, true, true };

    bool isgamescene = false;

    public void OnLevelWasLoaded()
    {
        if (7 != SceneManager.GetActiveScene().buildIndex) return;
        isgamescene = true;
        GS = GameObject.Find("GAME SYSTEM").GetComponent<MainGameSystem>();
    }

        byte[][] recvbuf = new byte[4][];
    // Use this for initialization
    void Start ()
    {
        NC = this.GetComponent<NetworkController>();
        recvbuf[1] = new byte[NetworkController.MAXBUFFERSIZE];
        recvbuf[2] = new byte[NetworkController.MAXBUFFERSIZE];
        recvbuf[3] = new byte[NetworkController.MAXBUFFERSIZE];
        DontDestroyOnLoad (gameObject);
        if (null == CC) CC = GameObject.Find("Cursor Manager(Clone)").GetComponent<CursorControl>();
        
        IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        IPAddress ipAddress = ipHostInfo.AddressList[0];
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 8080);
        
        sck = new Socket (ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
		sck.Bind (localEndPoint);
        sck.Listen(100);
        while (true) {
            allDone.Reset();

            sck.BeginAccept(new AsyncCallback(AcceptCallback), sck);

            allDone.WaitOne();
        }
    }

    void AcceptCallback(IAsyncResult ar)
    {
        allDone.Set();
        
        Socket listener = (Socket)ar.AsyncState;
        Socket handler = listener.EndAccept(ar);
        // 받는 방식의 변화 재접속...
        int id = client_sock.Count;
        if (id > NetworkController.MAX_CONNECT)
        {
            handler.Disconnect(false);
        }
        client_sock.Add(handler);

        if (false == isgamescene) CC.connected(id);
        else GS.reconnected(id);

        NC.net_send_signal(NetworkController.SC_CONNECT, handler);
        SC_CONNECT_PACKET packet = new SC_CONNECT_PACKET();
        Debug.Log((int)(char)id);
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
        NC.net_send(packet, handler, NetworkController.SC_CONNECT);
        connected[id] = false;

        IAsyncClient iaclient = new IAsyncClient();
        iaclient.s = handler;
        iaclient.id = id;
        iaclient.recvbyte = 0;
        clients.Add(iaclient);
        handler.BeginReceive(iaclient.recvbuf, 0, NetworkController.MAXBUFFERSIZE, 0, new AsyncCallback(DataReceive), iaclient);
        sck.BeginAccept(AcceptCallback, null);
    }
    
    //받은 데이터 체크 꼭 할것!!!!
    void DataReceive(IAsyncResult ar)
    {
        IAsyncClient obj = (IAsyncClient)ar.AsyncState;
        Socket handler = obj.s;

        int bytesRead = handler.EndReceive(ar);
        obj.recvbyte += bytesRead;

        //길이 맞는지 확인해보고
        if (obj.recvbyte > 0)
        {
            if (true == obj.signalread)
            {
                obj.recv_signal = Convert.ToChar(obj.recvbuf[0]);
                Buffer.BlockCopy(obj.recvbuf, 1, obj.recvbuf, 0, obj.recvbyte - 1);
                obj.recvbyte -= 1;
                obj.signalread = false;
            }

            if (false == obj.signalread)
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
                object recvobj = NC.ByteToObj(obj.recvbuf);
                Buffer.BlockCopy(obj.recvbuf, 116, obj.recvbuf, 0, obj.recvbyte - 116);
                obj.recvbyte -= 116;
                    if (obj == null)
                {
                    obj.s.BeginReceive(obj.recvbuf, 0, NetworkController.MAXBUFFERSIZE, 0, DataReceive, obj);
                    return;
                }
                CS_CONNECT_PACKET res = (CS_CONNECT_PACKET)recvobj;

                CC.nickname[res.id] = res.nickname;
            }

            else if (obj.recv_signal == NetworkController.CS_MOVE && obj.recvbyte >= 89)
            {
                object recvobj = NC.ByteToObj(obj.recvbuf);
                Buffer.BlockCopy(obj.recvbuf, 89, obj.recvbuf, 0, obj.recvbyte - 89);
                obj.recvbyte -= 89;
                if (obj == null)
                {
                    obj.s.BeginReceive(obj.recvbuf, 0, NetworkController.MAXBUFFERSIZE, 0, DataReceive, obj);
                    return;
                }
                CS_MOVE_PACKET res = (CS_MOVE_PACKET)recvobj;

                if (false == isgamescene) CC.move(res.x, res.y, res.id);
                else GS.move(res.x, res.y, res.id);

            }
            else if (obj.recv_signal == NetworkController.CS_BTN && obj.recvbyte >= 89)
            {
                object recvobj = NC.ByteToObj(obj.recvbuf);
                Buffer.BlockCopy(obj.recvbuf, 89, obj.recvbuf, 0, obj.recvbyte - 89);
                obj.recvbyte -= 89;
                if (obj == null)
                {
                    obj.s.BeginReceive(obj.recvbuf, 0, NetworkController.MAXBUFFERSIZE, 0, DataReceive, obj);
                    return;
                }
                CS_BUTTON_PACKET res = (CS_BUTTON_PACKET)recvobj;

                if (false == isgamescene) CC.click(res.id, res.btn_number);
                else GS.click(res.id, res.btn_number);
            }
            else {
                handler.BeginReceive(obj.recvbuf, obj.recvbyte, NetworkController.MAXBUFFERSIZE - obj.recvbyte, 0, new AsyncCallback(DataReceive), obj);
            }
        }
    }
}

public class IAsyncClient {
    public Socket s = null;
    public byte[] recvbuf = new byte[NetworkController.MAXBUFFERSIZE];
    public int recvbyte;
    public char recv_signal;
    public string nickname;
    public bool signalread = true;
    public int id;
}