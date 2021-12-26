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
using UnityEngine.UI;
using TMPro;


public class NetworkingClient : Networking
{
    private Client client;
    private WorldReplication world_Replication;

    public GameManager gameManager;
    public Transform paddle1_transform, paddle2_transform;
    public GameObject ball;

    string id = Guid.NewGuid().ToString();
    public float movement;

    public Text text;
    public string msgToshow;

    //Scores
    public TMP_Text paddle1ScoreText;
    public TMP_Text paddle2ScoreText;

    bool enableMovement;

    void Start()
    {
        world_Replication = new WorldReplication();
        enableMovement = false;

        try
        {
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), listenPort);

            sender = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6000);
            remote = sender;

            listener.Connect(ipep);

            client = new Client();

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
        text.text = msgToshow;

        if (enableMovement)
        {
            paddle1_transform.position = new Vector3(7.5f, float.Parse(world_Replication.Paddle1Pos), 0.0f);
            paddle2_transform.position = new Vector3(-7.5f, float.Parse(world_Replication.Paddle2Pos), 0.0f);
            ball.transform.position = new Vector3(float.Parse(world_Replication.BallPosX), float.Parse(world_Replication.BallPosY), 0.0f);
            enableMovement = !enableMovement;
        }



        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow))
            SendMsg(PacketType.PT_InputData);
    }

    void ReceiveMsg()
    {
        while (true)
        {
            packageDataRcv = new byte[1024];

            listener.Receive(packageDataRcv);

            try
            {
                Client tmp_Client = new Client();
                tmp_Client = DeserializeClient();

                switch ((PacketType)int.Parse(tmp_Client.PackageType))
                {                   
                    case PacketType.PT_Welcome:
                        client.id = tmp_Client.id;
                        client.socket_client = tmp_Client.socket_client;
                        client.PaddleInUse = tmp_Client.PaddleInUse;
                        msgToshow = client.id;
                        break;

                    case PacketType.PT_Acknowledge:
                        break;                    

                    case PacketType.PT_Disconnect:
                        break;                    
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);

            }

            try
            {
                world_Replication = DeserializeWorldReplication();

                switch ((PacketType)int.Parse(world_Replication.PackageType))
                {
                    case PacketType.PT_Welcome:      
                        break;

                    case PacketType.PT_Acknowledge:
                        break;

                    case PacketType.PT_ReplicationData:
                        enableMovement = !enableMovement;
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);

            }


            Thread.Sleep(50);
        }
    }

    public void SendMsg(object type)
    {
         
        switch (type)
        {
            case PacketType.PT_Hello:
                int pK = (int)type;
                client.PackageType = pK.ToString();
                packageDataSnd = SerializeData(client);
                break;
            case PacketType.PT_Welcome:
                break;
            case PacketType.PT_Acknowledge:
                break;
            case PacketType.PT_InputData:
                client.PaddleMovement = GetInput();
                client.PackageType = 3.ToString();
                packageDataSnd = SerializeData(client);
                break;
            case PacketType.PT_Max:
                break;
            default:
                break;
        }


        listener.SendTo(packageDataSnd, packageDataSnd.Length, SocketFlags.None, ipep);
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
    public WorldReplication DeserializeWorldReplication()
    {
        WorldReplication tmpWrld = new WorldReplication();

        XmlSerializer serializer = new XmlSerializer(typeof(WorldReplication));
        MemoryStream stream = new MemoryStream();

        stream.Write(packageDataRcv, 0, packageDataRcv.Length);
        stream.Seek(0, SeekOrigin.Begin);

        tmpWrld = (WorldReplication)serializer.Deserialize(stream);
        return tmpWrld;
    }

    public Client DeserializeClient()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Client));
        MemoryStream stream = new MemoryStream();
        stream.Write(packageDataRcv, 0, packageDataRcv.Length);
        stream.Seek(0, SeekOrigin.Begin);

        Client c = new Client();
        
        c = (Client)serializer.Deserialize(stream);

        return c;
    }

    string GetInput()
    {
        //Serialize
        movement = Input.GetAxisRaw("Vertical");
        string a = movement.ToString();
        return a;
    }
}