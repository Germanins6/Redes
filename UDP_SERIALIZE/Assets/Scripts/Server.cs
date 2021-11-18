using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

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
    SerializePlayer serializer;

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
        catch (Exception e)
        {
            Debug.LogWarning(e.ToString());
        }


        serializer = new SerializePlayer();

        Player germa = new Player(null, "germá", 100.0f);
        serializer.Serialize("heyatesting", germa);
        Player newgerma = new Player(null);
        serializer.Deserialize("heyatesting", ref newgerma);
        Debug.Log(newgerma.Score);


    }

    // Update is called once per frame
    void Update()
    {
        //If server didn't initialize can't update
        if (!initialized)
            return;

        //Check non-null data for each client
        foreach (Player client in currentPlayers)
        {

        }

        //TODO 5: If stream does have data available then call BroadcastFunction
    }

    //Receive client connection and store into player list
    private void AcceptClients(IAsyncResult ias)
    {
        UdpClient listener = ias.AsyncState as UdpClient;
        Player newPlayer = new Player(null); //listener.EndReceive(ias, ref ipep);
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
        {

            //Broadcast(); Why recursive
        }
    }

}


public class SerializePlayer
{

    IFormatter formatter;

    //Constructor
    public SerializePlayer()
    {
        formatter = new BinaryFormatter();
    }

    public void Serialize(string filename, Player playerData)
    {
        //Receive player and creates instance with original player data
        Player playerInstance = new Player(null);
        playerInstance = playerData; //Separated fields(?)

        //Write data from instance into document
        FileStream stream = new FileStream(filename, FileMode.Create);

        //Prevent error if serialization not succesful
        try
        {
            formatter.Serialize(stream, playerInstance);
        }
        catch (SerializationException e)
        {
            //Debug.Log(e.ToString());
            throw;
        }
        finally
        {
            stream.Close();
        }
    }

    //Should be returning a player?
    public void Deserialize(string fileName, ref Player playerData)
    {
        FileStream stream = new FileStream(fileName, FileMode.Open);

        try
        {
            playerData = formatter.Deserialize(stream) as Player;
        }
        catch (SerializationException e)
        {
            //Debug.LogWarning(e.ToString());
            throw;
        }
        finally
        {
            stream.Close();
        }
    }
}

[Serializable]
public class Player : ISerializable
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

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        //if (info == null)
        //    throw;

        //Serialize Player Data
        info.AddValue("_socket", client);
        info.AddValue("_position", y);
        info.AddValue("_speed", paddleSpeed);
        info.AddValue("_score", score);
        info.AddValue("_name", playerName);
        info.AddValue("_id", playerID);
    }

    public Player(SerializationInfo info, StreamingContext context)
    {
        client = (UdpClient)info.GetValue("_socket", typeof(UdpClient));
        y = (float)info.GetValue("_position", typeof(float));
        paddleSpeed = (float)info.GetValue("_speed", typeof(float));
        score = (float)info.GetValue("_score", typeof(float));
        playerName = (string)info.GetValue("_name", typeof(string));
        playerID = (Guid)info.GetValue("_id", typeof(Guid));
    }

    //Getters and Setters
    public float Position
    {
        get { return y; }
        set { y = value; }
    }

    public float Score
    {
        get { return score; }
        set { score = value; }
    }

    public Guid ID
    {
        get { return playerID; }
        set { playerID = value; }
    }

    //TODO: PADDLE POWER-UPS(?)

}