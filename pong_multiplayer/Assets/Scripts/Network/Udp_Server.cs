using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using UnityEngine.UI;

public class Udp_Server : MonoBehaviour
{

    private const int listenPort = 6000;

    UdpClient listener;
    public IPEndPoint ipep;
    public EndPoint epo;
    bool initialized = false;
    SerializePlayer serializer;
    // Testing conexion
    public Text updateText;
    string text;
    Thread ThreadReceive;
    byte[] data = new byte[1024];

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
            listener = new UdpClient(listenPort);
            ipep = new IPEndPoint(IPAddress.Any, listenPort);

            StartListening();
            initialized = true;
            ThreadReceive = new Thread(ReceiveMsg);
            ThreadReceive.Start();
            Debug.Log("Server initialized in localhost:6000");
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.ToString());
        }

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
        listener.BeginReceive(AcceptClients, listener);
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

    // Update is called once per frame
    void Update()
    {
        //If server didn't initialize can't update
        if (!initialized)
            return;

        updateText.text = text;
        //Check non-null data for each client
        foreach (Player client in currentPlayers)
        {

        }

        //TODO 5: If stream does have data available then call BroadcastFunction
    }
    void ReceiveMsg()
    {
        
        while (true)
        {
            data = listener.Receive(ref ipep); //Receive data from server
            text = Encoding.ASCII.GetString(data);
            Debug.Log(text);
            Thread.Sleep(500);  //wait for 500ms
        }

    }
}
