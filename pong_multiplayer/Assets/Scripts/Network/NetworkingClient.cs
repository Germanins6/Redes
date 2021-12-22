using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using UnityEngine.UI;

public class NetworkingClient : Networking
{
    Socket udp_client;
    IPEndPoint ipep, sender;
    EndPoint Remote;
    byte[] data = new byte[1024];
    Thread TwaitMsg;
    int recv;

    //timers
    float currentTime;
    float timeOut;
    float lastPing;

    // Start is called before the first frame update
    void Start()
    {
        udp_client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        ipep = new IPEndPoint(IPAddress.Any, 6000);
        sender = new IPEndPoint(IPAddress.Any, 6000);
        Remote = sender;

        udp_client.Connect(ipep);

        TwaitMsg = new Thread(ReceiveMsg);
        TwaitMsg.Start();

    }

    // Update is called once per frame
    void Update()
    {

    }

    void ReceiveMsg()
    {
        data = new byte[1024];
        while (true)
        {
            recv = udp_client.ReceiveFrom(data, ref Remote); //Receive data from server           
            Thread.Sleep(500);  //wait for 500ms
        }

    }

    void SendMsg()
    {
        byte[] data_send = Encoding.UTF8.GetBytes("ping"); //Sends Ping to the server
        udp_client.SendTo(data_send, data_send.Length, SocketFlags.None, ipep); //Send data to server

    }
}
