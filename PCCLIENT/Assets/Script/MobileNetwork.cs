using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Net;
using System.Net.Sockets;

using System.Threading;
//PC연결도 5번째 소켓으로 같이 수행
public class MobileNetwork : MonoBehaviour {
	NetworkController NC;
	CursorControl CC;
    volatile Socket sck;
    List<IAsyncClient> clients = new List<IAsyncClient>();


    List<Socket> client_sock = new List<Socket>();
    volatile bool[] connected = new bool[4] {true, true, true, true };
  
    byte[][] recvbuf = new byte[4][];

    
    // Use this for initialization
    void Start ()
    {
        NC = this.GetComponent<NetworkController>();
        recvbuf[1] = new byte[NetworkController.MAXBUFFERSIZE];
        recvbuf[2] = new byte[NetworkController.MAXBUFFERSIZE];
        recvbuf[3] = new byte[NetworkController.MAXBUFFERSIZE];
        DontDestroyOnLoad (gameObject);
		CC = GameObject.Find("Cursor Manager").GetComponent<CursorControl>();
		sck = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		sck.Bind (new IPEndPoint (IPAddress.Any, 8080));
        sck.Listen(5);
        sck.BeginAccept(AcceptCallback,null);
    }

    void AcceptCallback(IAsyncResult ar)
    {
        Socket client = sck.EndAccept(ar);

        int id = client_sock.Count;
        if (id > NetworkController.MAX_CONNECT)
        {
            client.Disconnect(false);
        }
        client_sock.Add(client);


        CC.connected(id);
        NC.net_send_signal(NetworkController.SC_CONNECT, client);
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
        NC.net_send(packet, client);
        connected[id] = false;

        IAsyncClient iaclient = new IAsyncClient();
        iaclient.s = client;
        clients.Add(iaclient);
        client.BeginReceive(recvbuf[id], 0, NetworkController.MAXBUFFERSIZE, 0, DataReceive, iaclient);
        sck.BeginAccept(AcceptCallback, null);
    }

    void DataReceive(IAsyncResult ar)
    {
        IAsyncClient obj = (IAsyncClient)ar.AsyncState;

        if (obj.recvbuf.Length == 1)
        {
            obj.recv_signal = Convert.ToChar(obj.recvbuf[0]);
        }

        else if(obj.recvbuf.Length >= 2) {
            if (obj.recv_signal == NetworkController.CS_DISCONNECT)
            {
                client_sock.Remove(obj.s);
                obj.s.Disconnect(false);
                return;
            }
            else if (obj.recv_signal == NetworkController.CS_CONNECT)
            {
                object recvobj = NC.ByteToObj(obj.recvbuf);
                if (obj == null) {
                    obj.s.BeginReceive(obj.recvbuf, 0, NetworkController.MAXBUFFERSIZE, 0, DataReceive, obj);
                    return;
                }
                CS_CONNECT_PACKET res = (CS_CONNECT_PACKET)recvobj;

                obj.nickname = res.nickname;
            }

            else if (obj.recv_signal == NetworkController.CS_MOVE)
            {
                object recvobj = NC.ByteToObj(obj.recvbuf);
                if (obj == null)
                {
                    obj.s.BeginReceive(obj.recvbuf, 0, NetworkController.MAXBUFFERSIZE, 0, DataReceive, obj);
                    return;
                }
                CS_MOVE_PACKET res = (CS_MOVE_PACKET)recvobj;

                CC.move(res.movevector, res.id);

            }
            else if (obj.recv_signal == NetworkController.CS_BTN)
            {
                object recvobj = NC.ByteToObj(obj.recvbuf);
                if (obj == null)
                {
                    obj.s.BeginReceive(obj.recvbuf, 0, NetworkController.MAXBUFFERSIZE, 0, DataReceive, obj);
                    return;
                }
                CS_BUTTON_PACKET res = (CS_BUTTON_PACKET)recvobj;

                CC.click(res.id);
            }
        }
    }
}

public class IAsyncClient {
    public Socket s;
    public byte[] recvbuf = new byte[NetworkController.MAXBUFFERSIZE];
    public char recv_signal;
    public string nickname;

    public IAsyncClient() {
    }
   
}