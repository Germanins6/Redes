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

    void Start()
    {
        world_Replication = new WorldReplication();

        try
        {
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), listenPort);

            sender = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6000);
            remote = sender;

            listener.Connect(ipep);

            client = new Client();

            //ThreadSend = new Thread(SendMsg);
            //ThreadSend.Start();

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

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
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
                if (packageDataRcv != null)
                    Debug.LogError("PackageFull");

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
                    case PacketType.PT_ReplicationData:
                        packageDataSnd = Encoding.ASCII.GetBytes("Client " + id + " pressing key");
                        break;
                    case PacketType.PT_Disconnect:
                        break;                    
                    default:
                        Debug.LogError("HA ENTRADO JOPUTA CLIENT 6");
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);

            }

            try
            {
                if (packageDataRcv != null)
                    Debug.LogError("PackageFull");

                DeserializeData();

                switch ((PacketType)int.Parse(world_Replication.PackageType))
                {
                    case PacketType.PT_Welcome:      
                        
                        break;
                    case PacketType.PT_Acknowledge:
                        break;
                    case PacketType.PT_ReplicationData:
                        
                        gameManager.paddle1Score = int.Parse(world_Replication.Client1_Score);
                        gameManager.paddle2Score = int.Parse(world_Replication.Client2_Score);

                        paddle1_transform.position = new Vector3(paddle1_transform.position.x, float.Parse(world_Replication.Paddle1Pos), paddle1_transform.position.z);
                        paddle2_transform.position = new Vector3(paddle2_transform.position.x, float.Parse(world_Replication.Paddle2Pos), paddle2_transform.position.z);


                        break;
                    default:
                        Debug.LogError("HA ENTRADO JOPUTA CLIENT 6");
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
                //packageDataSnd = SerializeData(client);
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
    public void DeserializeData()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(WorldReplication));
        MemoryStream stream = new MemoryStream();
        stream.Write(packageDataRcv, 0, packageDataRcv.Length);
        stream.Seek(0, SeekOrigin.Begin);

        world_Replication = (WorldReplication)serializer.Deserialize(stream);
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