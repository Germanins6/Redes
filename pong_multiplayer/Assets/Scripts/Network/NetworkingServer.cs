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
    public GameManager gameManager;

    int recv;
    bool paddle;

    public Text text;
    public string msgToshow;

    private float yBound;
    private float speed;
    float lastMs;

    //Ball info??¿?¿?¿?¿?¿?
    public GameObject ball;


    public float initialVelocity;
    public float velocityMultiplier;
    private Vector2 Vel;
    public Rigidbody2D ballRb;

    public int paddle1Score;
    public int paddle2Score;
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

   
    private void HandShakePlayer(Client tmp_Client)
    {
        tmp_Client.id = GenerateUUID();
        tmp_Client.socket_client = remote;

        //Assign paddles to each player
        if (!paddle)
        {
            tmp_Client.PaddleInUse = 1.ToString();
            paddle = !paddle;
        }
        else
        {
            tmp_Client.PaddleInUse = 2.ToString();
        }

        clients.Add(tmp_Client.id, tmp_Client);

        tmp_Client.PackageType = 1.ToString();

        packageDataSnd = new byte[1024];
        SerializeClient(tmp_Client);
        msgToshow = tmp_Client.id.ToString();
        SendMsg(tmp_Client);
    }

    private void BroadcastWorldState(Client tmp_Client)
    {
        //Foreach pair of player stored in our clientList send worldState
        MovePaddles(tmp_Client);

        world_Replication.PackageType = 4.ToString();

        packageDataSnd = new byte[1024];
        SerializeData(world_Replication);


        foreach (KeyValuePair<string, Client> player in clients)
        {
            if (tmp_Client.id == player.Key)
                tmp_Client.socket_client = player.Value.socket_client;
        }

        SendMsg(tmp_Client);
    }

    // Update is called once per frame
    void Update()
    {
        lastMs = Time.deltaTime;
        text.text = msgToshow;
        CheckScore();
    }

    void ReceiveMsg()
    {

        while (true)
        {
            packageDataRcv = new byte[1024];

            recv = listener.ReceiveFrom(packageDataRcv, ref remote);

            try
            {
                Client tmp_Client = new Client();

                tmp_Client = DeserializeData();

                switch ((PacketType)int.Parse(tmp_Client.PackageType))
                {

                    case PacketType.PT_Hello:
                        HandShakePlayer(tmp_Client);
                        break;

                    case PacketType.PT_InputData:
                        BroadcastWorldState(tmp_Client);
                        break;

                    case PacketType.PT_Disconnect:
                        //A
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

    void SendMsg(Client c)
    {
        listener.SendTo(packageDataSnd, packageDataSnd.Length, SocketFlags.None, c.socket_client);        //Send data to client
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

        if (int.Parse(c.PaddleInUse) == 1)
        {
            float paddle_pos = float.Parse(world_Replication.Paddle1Pos);
            paddle_pos = Mathf.Clamp(float.Parse(world_Replication.Paddle1Pos) + float.Parse(c.PaddleMovement) * speed * lastMs, -yBound, yBound);
            world_Replication.Paddle1Pos = paddle_pos.ToString();
        }
        else
        {
            float paddle_pos = float.Parse(world_Replication.Paddle2Pos);
            paddle_pos = Mathf.Clamp(float.Parse(world_Replication.Paddle2Pos) + float.Parse(c.PaddleMovement) * speed * lastMs, -yBound, yBound);
            world_Replication.Paddle2Pos = paddle_pos.ToString();
        }
    }

    //Initial launch of the ball
    public void Launch()
    {
        ball.transform.position = new Vector2(0, 0);
        Vel.x = Random.Range(0, 2) == 0 ? 1 : -1;
        Vel.y = Random.Range(0, 2) == 0 ? 1 : -1;
        ballRb.velocity = new Vector2(Vel.x, Vel.y) * initialVelocity;
    }
    public void Paddle1Scored()
    {
        paddle1Score++;
        world_Replication.Client1_Score = paddle1Score.ToString();
    }

    public void Paddle2Scored()
    {
        paddle2Score++;
        world_Replication.Client2_Score = paddle2Score.ToString();
    }

    public void CheckScore() 
    {
        if (ball.transform.position.x < -8.2)
        {
            Paddle1Scored();
            Launch();
        }
        else if (ball.transform.position.x > 8.2)
        {

            Paddle2Scored();
            Launch();
        }

    }
    #region InitializeAll 

    //Initialize Ball
    void InitializeBall()
    {
        initialVelocity = 4.0f;
        velocityMultiplier = 1.1f;
        ballRb = ball.GetComponent<Rigidbody2D>();
        Launch();
    }

    //Initialize Paddles
    void InitializePaddles()
    {
        yBound = 3.75f;
        speed = 7.0f;
    }

    #endregion
}
