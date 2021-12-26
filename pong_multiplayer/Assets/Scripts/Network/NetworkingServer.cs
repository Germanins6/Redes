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

    public Text text;
    public string msgToshow;

    //Padles info COMO COJO LA INFO DE OTRA ESCENA??¿?¿?¿?¿?¿?
    public Transform paddle1_transform, paddle2_transform;


    private float yBound;
    private float speed;

    //Ball info??¿?¿?¿?¿?¿?
    public GameObject ball;

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
        InitializeWorld();

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


    //Receive client connection and store into player list
    /*private void AcceptClients(IAsyncResult ias)
    {

        Socket server = ias.AsyncState as Socket;

        try
        {
            Socket client = server.EndAccept(ias);

            //UdpClient client = ias.AsyncState as UdpClient;
            IPEndPoint client_endpoint = client.RemoteEndPoint as IPEndPoint;

            Debug.Log("BBBBBBBBBBBBBBBBB");

            if(client_endpoint == null)
            {
                Debug.Log("AAAAAAAAAAAAAAAAAAAAA");
            }
            //Server receives hello packet from client to join and retrieves id and store info
            //Debug.Log(client.Client.RemoteEndPoint.AddressFamily.ToString());

            string id = GenerateUUID();

            Client newPlayer = new Client(client, id);

            clients.Add(id, newPlayer);


            ThreadReceive = new Thread(ReceiveMsg);
            ThreadReceive.Start(client_endpoint);
        }
        catch (System.Exception e)
        {
            Debug.Log("Exception: " + e.Message);
        }

        listener.BeginAccept(new AsyncCallback(AcceptClients), listener);
    }*/
   
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

        //Call Broadcast();

        //int i = 1;
        //foreach (KeyValuePair<IPEndPoint, Client> player in clients)
        //{
        //    MovePaddles(player.Value, i);
        //    i++;
        //}


        //ReceiveMsg();

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
                        
                        Debug.LogError(tmp_Client.socket_client);
                       
                        clients.Add(id, tmp_Client);

                        tmp_Client.PackageType = 1.ToString();
                        SerializeClient(tmp_Client);
                        msgToshow = tmp_Client.id.ToString();
                        SendMsg(tmp_Client);

                    }
                        break;
                    case PacketType.PT_Acknowledge:
                        break;
                    case PacketType.PT_InputData:
                        msgToshow = tmp_Client.PaddleMovement;
                        Debug.Log(tmp_Client.PaddleMovement);
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


    public void MovePaddles(Client c, int i)
    {
        if (i == 1)
        {
            world_Replication.Paddle1Pos = paddle1_transform.position.y;
            world_Replication.Paddle1Pos = Mathf.Clamp(world_Replication.Paddle1Pos + int.Parse(c.PaddleMovement) * speed * Time.deltaTime, -yBound, yBound);
            paddle1_transform.position = new Vector3(paddle1_transform.position.x, world_Replication.Paddle1Pos, paddle1_transform.position.z);
        }
        else
        {
            world_Replication.Paddle2Pos = paddle2_transform.position.y;
            world_Replication.Paddle2Pos = Mathf.Clamp(world_Replication.Paddle2Pos + int.Parse(c.PaddleMovement) * speed * Time.deltaTime, -yBound, yBound);
            paddle2_transform.position = new Vector3(paddle2_transform.position.x, world_Replication.Paddle2Pos, paddle2_transform.position.z);
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

    //Initialize World
    void InitializeWorld()
    {

        world_Replication = new WorldReplication();

        world_Replication.Paddle1Pos = paddle1_transform.position.y;
        world_Replication.Paddle2Pos = paddle1_transform.position.y;
        world_Replication.BallVel = Vel;

    }

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
