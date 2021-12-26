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
using Random = UnityEngine.Random;

public class NetworkingServer : Networking
{

    private WorldReplication world_Replication;
    public Dictionary<string, Client> clients;
    private GameManager gameManager;

    int recv;
    bool paddle;

    public Text text;
    public string msgToshow;

    private float yBound;
    private float speed;

    //Ball info??¿?¿?¿?¿?¿?
    //public GameObject ball;

    public float initialVelocity;
    public float velocityMultiplier;
    private Vector2 Vel;
    public Rigidbody2D ballRb;

    // Start is called before the first frame update
    void Start()
    {
        //Initialize Elements
        InitializePaddles();
        InitializeBall();
        world_Replication = new WorldReplication();

        paddle = false;

        //Initialize UDP server
        try
        {
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            ipep = new IPEndPoint(IPAddress.Any, listenPort);
            listener.Bind(ipep);

            sender = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6000);
            remote = sender;

            clients = new Dictionary<string, Client>();

            //listener.BeginAccept(new AsyncCallback(AcceptClients), listener);
            ThreadReceive = new Thread(ReceiveMsg);
            ThreadReceive.Start();

            Debug.Log("Server initialized in [" + ip.ToString() + "] : " + listenPort.ToString());
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.ToString());
        }
    }

   
    private void BroadcastWorldState()
    {
        //Foreach pair of player stored in our clientList send worldState
        foreach(KeyValuePair<string, Client> player in clients)
        {
            //Call Replication World function each 100ms(?).
            //WordReplication(player.Value.ep, ¿WorldStateData[]?);
        }
    }

    // Update is called once per frame
    void Update()
    {
        text.text = msgToshow;
    }

    void ReceiveMsg()
    {

        while (true)
        {
            packageDataRcv = new byte[1024];

            recv = listener.ReceiveFrom(packageDataRcv, ref remote);

            try
            {
                if (packageDataRcv != null)
                    Debug.LogError("PackageFull");

                Client tmp_Client = new Client();

                tmp_Client = DeserializeData();

                switch ((PacketType)int.Parse(tmp_Client.PackageType))
                {
                    case PacketType.PT_Hello:
                    {
                        string id = GenerateUUID();
                        
                        tmp_Client.id = id;                        
                        tmp_Client.socket_client = remote;

                        if (!paddle)
                        {
                            tmp_Client.PaddleInUse = 1.ToString();
                            paddle = true;
                        }
                        else
                            tmp_Client.PaddleInUse = 2.ToString();

                        Debug.LogError(tmp_Client.socket_client);
                       
                        clients.Add(id, tmp_Client);

                        tmp_Client.PackageType = 1.ToString();

                        packageDataSnd = new byte[1024];
                        SerializeClient(tmp_Client);
                        msgToshow = tmp_Client.id.ToString();
                        SendMsg(tmp_Client);

                    }
                        break;
                    case PacketType.PT_Acknowledge:
                        break;
                    case PacketType.PT_InputData:

                       
                        MovePaddles(tmp_Client);
                        Debug.LogError(world_Replication.Paddle1Pos + "   "  + world_Replication.Paddle2Pos);

                        world_Replication.PackageType = 4.ToString();

                        packageDataSnd = new byte[1024];
                        SerializeData(world_Replication);

                        
                        foreach (KeyValuePair<string, Client> player in clients)
                        {
                            if (tmp_Client.id == player.Key)
                                tmp_Client.socket_client = player.Value.socket_client;

                        }

                        SendMsg(tmp_Client);


                        //msgToshow = tmp_Client.PaddleMovement;
                        //Debug.Log(tmp_Client.PaddleMovement);
                        break;
                    case PacketType.PT_Disconnect:
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
              
            }

            

            //Debug.Log(Encoding.ASCII.GetString(packageDataRcv));
            Thread.Sleep(50);
        }
    }

    void SendMsg(Client c)
    {
        Debug.LogError("ANTES");
        if (c.socket_client == null)
            Debug.LogError("PackageEmpty");
        if (c.socket_client != null)
            Debug.LogError("PackageSendFull");

        listener.SendTo(packageDataSnd, packageDataSnd.Length, SocketFlags.None, c.socket_client);        //Send data to client
        Debug.LogError("DESPUES");
    }

    string GenerateUUID()
    {
        return Guid.NewGuid().ToString();
    }

    public void SerializeData(WorldReplication wR)
    {
        //Serialize Data
        XmlSerializer serializer = new XmlSerializer(typeof(WorldReplication));
        MemoryStream stream = new MemoryStream();
        serializer.Serialize(stream, wR);

        packageDataSnd = stream.ToArray();

    }

    public void SerializeClient(Client c)
    {
        //Serialize Data
        XmlSerializer serializer = new XmlSerializer(typeof(Client));
        MemoryStream stream = new MemoryStream();
        serializer.Serialize(stream, c);

        packageDataSnd = stream.ToArray();

    }

    //Deserialize data from XML file
    public Client DeserializeData()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Client));
        MemoryStream stream = new MemoryStream();
        stream.Write(packageDataRcv, 0, packageDataRcv.Length);
        stream.Seek(0, SeekOrigin.Begin);

        Client client_ = new Client();
        
        client_ = (Client)serializer.Deserialize(stream);

        return client_;
    }


    public void MovePaddles(Client c)
    {
        Debug.LogError("ADIOS MUY BUENAS 0");

        if (int.Parse(c.PaddleInUse) == 1)
        {
            Debug.LogError("ADIOS MUY BUENAS 1");

            float paddle_pos = float.Parse(world_Replication.Paddle1Pos);

            Debug.LogError("ADIOS MUY BUENAS 2");

            paddle_pos = Mathf.Clamp(float.Parse(world_Replication.Paddle1Pos) + float.Parse(c.PaddleMovement) * speed * (DateTime.Now.Ticks / 10000000), -yBound, yBound);

            Debug.LogError("ADIOS MUY BUENAS 3");

            world_Replication.Paddle1Pos = paddle_pos.ToString();
        }
        else
        {
            float paddle_pos = float.Parse(world_Replication.Paddle2Pos);
            paddle_pos = Mathf.Clamp(float.Parse(world_Replication.Paddle2Pos) + float.Parse(c.PaddleMovement) * speed * (DateTime.Now.Ticks/10000000), -yBound, yBound);
            
            world_Replication.Paddle2Pos = paddle_pos.ToString();
        }

    }

    //Initial launch of the ball
    public void Launch()
    {
        Vel.x = Random.Range(0, 2) == 0 ? 1 : -1;
        Vel.y = Random.Range(0, 2) == 0 ? 1 : -1;
        ballRb.velocity = new Vector2(Vel.x, Vel.y) * initialVelocity;

    }

    #region InitializeAll 

    //Initialize Ball
    void InitializeBall()
    {
        initialVelocity = 4.0f;
        velocityMultiplier = 1.1f;
    }

    //Initialize Paddles
    void InitializePaddles()
    {
        yBound = 3.75f;
        speed = 7.0f;
    }

    #endregion
}
