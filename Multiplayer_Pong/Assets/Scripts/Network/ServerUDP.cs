using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Text;
using UnityEngine.UI;

public class ServerUDP : MonoBehaviour
{
    Socket Socket_udp_server;
    Thread TreceiveMsg;

    IPEndPoint sender;
    EndPoint Remote;

    public Text showMsg;
    string msg_recv;

    int recv;
    byte[] data_send = new byte[1024];
    byte[] data;


    void Start()
    {
        data_send = Encoding.ASCII.GetBytes("Pong");

        Socket_udp_server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);         //Create socket

        IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6000);       //Create ipendpoint
        Socket_udp_server.Bind(ipep);       //Bind socket with ipendpoint

        sender = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6000);
        Remote = (EndPoint)(sender);

        TreceiveMsg = new Thread(ReceiveMsg);
        TreceiveMsg.Start();
    }

    void Update()
    {
        showMsg.text = msg_recv;
    }

    void ReceiveMsg()
    {
        data = new byte[1024];

        while (true)
        {
            recv = Socket_udp_server.ReceiveFrom(data, ref Remote);     //Receive data from client

            string recv_msg = "---Message Received: " + Encoding.UTF8.GetString(data, 0, recv);  //Transform data to String

            msg_recv += "\n" + (System.DateTime.Now.ToString() + recv_msg);

            //Debug.Log("Server: Message Received" + recv_msg);
            
            Thread.Sleep(500);  //wait for 500ms
            SendMsg();
        }
    }

    void SendMsg()
    {
        Socket_udp_server.SendTo(data_send, data_send.Length, SocketFlags.None, Remote);        //Send data to client

        msg_recv += "\n" + System.DateTime.Now.ToString() + "---Message Sent: " + Encoding.UTF8.GetString(data_send, 0, data_send.Length);  //Transform data to String

        //Debug.Log("Server: Message Send" + msg_recv);
    }

    private void OnDestroy()    //stops main thread and close socket
    {
        TreceiveMsg.Abort();        //Stop thread
        Socket_udp_server.Close();      //Close socket

        Debug.Log("Socket Closed");
    }
}