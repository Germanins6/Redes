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
using Random = UnityEngine.Random;

public class NetworkingServer : Networking
{

    private WorldReplication world_Replication;
    private Dictionary<IPEndPoint, Client> clients;
    private GameManager gameManager;

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
            listener = new UdpClient(listenPort);
            ipep = new IPEndPoint(IPAddress.Any, listenPort);

            //Server receives hello packet from client to join and retrieves id and store info
            /*
            Client newPlayer = new Client(ipep, GenerateUUID());
            clients.Add(ipep, newPlayer);
            */

            listener.BeginReceive(new AsyncCallback(AcceptClients), listener);

            Debug.Log("Server initialized in [" + ip.ToString() + "] : " + listenPort.ToString());
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.ToString());
        }
    }


    //Receive client connection and store into player list
    private void AcceptClients(IAsyncResult ias)
    {
        UdpClient server = ias.AsyncState as UdpClient;

        try
        {
            IPEndPoint ipe = ias.AsyncState as IPEndPoint;

            ThreadReceive = new Thread(ReceiveMsg);
            ThreadReceive.Start(ipe);
        }
        catch (System.Exception e)
        {
            Debug.Log("Exception: " + e.Message);
        }



        listener.BeginReceive(new AsyncCallback(AcceptClients), listener);
    }

    private void BroadcastWorldState()
    {
        //Foreach pair of player stored in our clientList send worldState
        foreach(KeyValuePair<IPEndPoint, Client> player in clients)
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


        //Each time we receive an input package we deserialize
        if (packageDataRcv != null)
        {
            float movement = GetInputData(packageDataRcv);
            Debug.Log(movement);
        }
    }


    void ReceiveMsg(object ipep_Client_)
    {
        IPEndPoint ipep_Client = ipep_Client_ as IPEndPoint;

        while (true)
        {
            packageDataRcv = listener.Receive(ref ipep_Client);
            Debug.Log(Encoding.ASCII.GetString(packageDataRcv));
            Thread.Sleep(50);
        }
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

    //Deserialize data from XML file
    public void DeserializeData()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(WorldReplication));
        MemoryStream stream = new MemoryStream();
        stream.Write(packageDataRcv, 0, packageDataRcv.Length);
        stream.Seek(0, SeekOrigin.Begin);

        world_Replication = (WorldReplication)serializer.Deserialize(stream);
    }

    float GetInputData(object data)
    {
        //Deserialize
        float value;
        byte[] inputPackage = data as byte[];
        string movement = Encoding.ASCII.GetString(inputPackage);
        float.TryParse(movement, out value);
        return value;
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

public class Client
{

    public Client()
    {
        ep = null;
        id = string.Empty;
        PaddleMovement = string.Empty;
    }
    public Client(IPEndPoint e, string uuid)
    {
        ep = e;
        id = uuid;
    }

    [XmlIgnore]
    public IPEndPoint ep;

    [XmlIgnore]
    public string id;

    [XmlElement("Paddle_Movement")]
    public string PaddleMovement;

}
public class WorldReplication
{
    //Data
    public float Paddle1Pos, Paddle2Pos;
    public Vector2 BallVel;

    public int Client1_Score, Client2_Score;
    public bool Client1_isConnected, Client2_isConnected;
}
