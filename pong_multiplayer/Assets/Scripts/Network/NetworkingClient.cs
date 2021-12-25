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

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            listener = new UdpClient("127.0.0.1", listenPort);
            ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), listenPort);
            listener.Connect(ipep);

            ThreadSend = new Thread(SendMsg);
            ThreadSend.Start(PacketType.PT_Hello);

            ThreadReceive = new Thread(ReceiveMsg);
            ThreadReceive.Start();
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

    public void SendMsg(object type)
    {
         
        var packet = type as PacketType?;


        switch (type)
        {
            case PacketType.PT_Hello:
                packageDataSnd = Encoding.ASCII.GetBytes("Hello");
                break;
            case PacketType.PT_Welcome:
                break;
            case PacketType.PT_Acknowledge:
                break;
            case PacketType.PT_InputData:
                break;
            case PacketType.PT_ReplicationData:
                break;
            case PacketType.PT_Disconnect:
                break;
            case PacketType.PT_Max:
                break;
            default:
                break;
        }

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