using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Xml.Serialization;


public class NetworkingClient : Networking
{

    int recv;
    string text;

    class Client
    {
        //Each client get assigned a UUID
        [XmlIgnore]
        Guid id = Guid.NewGuid();

        [XmlIgnore]
        public float Paddle_Movement;

        [XmlElement("Paddle_Movement")]
        public string PaddleMovement;

    }
    class WorldReplication
    {
        //Data
        public float Paddle1Pos, Paddle2Pos;
        public Vector2 BallPos;

        public int Client1_Score, Client2_Score;

        public bool Client1_isConnected, Client2_isConnected;

    }

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            listener = new UdpClient("127.0.0.1", listenPort);
            ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), listenPort);
            listener.Connect(ipep);

            ThreadSend = new Thread(SendMsg);
            ThreadSend.Start();

            ThreadReceive = new Thread(ReceiveMsg);
            ThreadReceive.Start();

            SendMsg();
        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
        }

    }

    // Update is called once per frame
    void Update()
    {
        //PSEUDO
        /*
         Each x ms send player state to server 
        */
    }

    void ReceiveMsg()
    {
        while (true)
        {
            packageDataRcv = listener.Receive(ref ipep);
            Thread.Sleep(50);
        }
    }

    public void SendMsg()
    {
        byte[] packageDataSnd = Encoding.ASCII.GetBytes("Client sending message to server");
        listener.Send(packageDataSnd, packageDataSnd.Length);
        Thread.Sleep(50);
    }


    void SendPackets()
    {

    }

    void ProcessPacket(ref MemoryStream stream, ref UdpClient client)
    {

    }
}