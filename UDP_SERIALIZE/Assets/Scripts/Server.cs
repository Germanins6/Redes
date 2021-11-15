using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;
using System.Runtime.Serialization;

using System.Net;
using System.Net.Sockets;


public class Server : MonoBehaviour
{

    //Receive the current state of a client
    public enum ClientState
    {
        CS_Error = -1,
        CS_Initialize,
        CS_Playing,
        CS_Disconnected,
        CS_Max
    }

    private const int listenPort = 6000;

    UdpClient server;
    public IPEndPoint ipep;
    public EndPoint epo;
    bool initialized = false;

    /*---Should be doing a list for each player or just 2 clients??? CHECK---*/
    public List<Player> currentPlayers;

    // Start is called before the first frame update
    void Start()
    {
        currentPlayers = new List<Player>();

        //Initialize UDP server
        try
        {
            //TODO 1: Connect UPD socket to local network
            server = new UdpClient(listenPort);
            ipep = new IPEndPoint(IPAddress.Any, listenPort);

            StartListening();
            initialized = true;

            Debug.Log("Server initialized in localhost:6000");
        }   
        catch(Exception e)
        {
            Debug.LogWarning(e.ToString());
        }
    }

    // Update is called once per frame
    void Update()
    {
        //If server didn't initialize can't update
        if (!initialized)
            return;

        //Check non-null data for each client
        foreach(Player client in currentPlayers)
        {

        }
        
        //TODO 5: If stream does have data available then call BroadcastFunction
    }

    //Receive client connection and store into player list
    private void AcceptClients(IAsyncResult ias)
    {
        UdpClient listener = ias.AsyncState as UdpClient;
        Player newPlayer = new Player(listener.EndReceive(ias, );
        currentPlayers.Add(newPlayer);

        StartListening();

        //TODO 7: BROADCAST THAT A NEW PLAYER HAS CONNECTED
    }


    //TODO 2: Make udp port receive permanent entry connections(just 2 players)
    private void StartListening()
    {
        server.BeginReceive(AcceptClients, server);
    }

    //TODO 3: Send serialized data to each player in game session
    private void Broadcast()
    {
        //TODO 6: Serialize to xml
        foreach (Player user in currentPlayers)
            Broadcast();
    }
}


[Serializable()]
public class Player
{
    UdpClient client;
    float y;
    float paddleSpeed;
    float score;
    string playerName;
    Guid playerID;

    //Constructors
    public Player(UdpClient socket)
    {
        client = socket;
        y = 0.0f;
        paddleSpeed = 1.0f;
        score = 0.0f;
        playerName = "Default";
        playerID = Guid.NewGuid();
    }

    public Player(UdpClient socket, string name, float points = 0.0f)
    {
        client = socket;
        y = 0.0f;
        paddleSpeed = 1.0f;
        score = points;
        playerName = name;
        playerID = Guid.NewGuid();
    }

    //Getters and Setters
    public void SetPlayerPosition(float posY) { y = posY; }
    public float GetPlayerPosition() { return y; }

    public Guid GetPlayerID() { return playerID; }

    //TODO: PADDLE POWER-UPS(?)

}