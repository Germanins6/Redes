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
    private Client client;
    private WorldReplication world_Replication;

    public Transform paddle1_transform, paddle2_transform;
    public GameObject ball;

    string id = Guid.NewGuid().ToString();
    public float movement;

    void Start()
    {
        world_Replication = new WorldReplication();

        try
        {
            listener = new UdpClient("127.0.0.1", listenPort);
            ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), listenPort);
            listener.Connect(ipep);

            client = new Client();

            ThreadSend = new Thread(SendMsg);
            ThreadSend.Start(PacketType.PT_Hello);

            ThreadReceive = new Thread(ReceiveMsg);
            ThreadReceive.Start();

            SendMsg(PacketType.PT_Hello);
        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
            SendMsg(PacketType.PT_InputData);
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
                //packageDataSnd = SerializeData(client);
                packageDataSnd = GetInput();
                break;
            case PacketType.PT_ReplicationData:
                packageDataSnd = Encoding.ASCII.GetBytes("Client " + id + " pressing key");
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

    //Serialize data and save it to XML file
    public byte[] SerializeData(Client c)
    {
        //Serialize Data
        XmlSerializer serializer = new XmlSerializer(typeof(Client));
        MemoryStream stream = new MemoryStream();
        serializer.Serialize(stream, c);

         return stream.ToArray();
    }

    //Deserialize data from XML file
    public void DeserializeData()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(WorldReplication));
        MemoryStream stream = new MemoryStream();
        stream.Write(packageDataRcv, 0, packageDataRcv.Length);
        stream.Seek(0, SeekOrigin.Begin);

        world_Replication = (WorldReplication)serializer.Deserialize(stream);
    }

    byte[] GetInput()
    {
        //Serialize
        movement = Input.GetAxisRaw("Vertical");
        string a = movement.ToString();
        return Encoding.ASCII.GetBytes(a);
    }
}