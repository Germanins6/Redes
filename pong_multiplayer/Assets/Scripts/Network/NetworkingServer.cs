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

public class NetworkingServer : Networking
{

    private Dictionary<EndPoint, Client> clients;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        //Initialize UDP server
        try
        {
            listener = new UdpClient(listenPort);
            ipep = new IPEndPoint(IPAddress.Any, listenPort);

            listener.BeginReceive(new AsyncCallback(AcceptClients), listener);

            Debug.Log("Server initialized in" + ip.ToString() + " : " + listenPort.ToString());
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
            Debug.Log("Starting async listenning");
            EndPoint ipe = ias.AsyncState as IPEndPoint;

            //Store data in player and generate id to welcome
            Client newPlayer = new Client(ipe, GenerateUUID());
            clients.Add(ipe, newPlayer);

            Debug.Log("Current clients:  " + clients.Count);
            
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
        foreach(KeyValuePair<EndPoint, Client> player in clients)
        {
            //Call Replication World function each 100ms(?).
            //WordReplication(player.Value.ep, ¿WorldStateData[]?);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Call Broadcast();
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


}

public class Client
{

    public Client(EndPoint e, string uuid)
    {
        ep = e;
        id = uuid;
    }

    [XmlIgnore]
    public EndPoint ep;

    [XmlIgnore]
    string id;

    [XmlElement("Paddle_Movement")]
    public string PaddleMovement;

}
public class WorldReplication
{
    //Data
    public float Paddle1Pos, Paddle2Pos;
    public Vector2 BallPos;

    public int Client1_Score, Client2_Score;

    public bool Client1_isConnected, Client2_isConnected;

}
