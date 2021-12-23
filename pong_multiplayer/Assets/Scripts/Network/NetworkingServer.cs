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

public class NetworkingServer : Networking
{

    public Text updateText;
    const int listenPort = 6000;

    Socket listener;
    Thread ThreadReceive;

    IPEndPoint sender;
    EndPoint remote;

    int recv;
    byte[] data;

    /*---Should be doing a list for each player or just 2 clients??? CHECK---*/
    public List<Player> currentPlayers;

    // Start is called before the first frame update
    void Start()
    {

        currentPlayers = new List<Player>();

        //Initialize UDP server
        try
        {
            listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Dgram, ProtocolType.Udp);

            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, listenPort);
            listener.Bind(ipep);

            sender = new IPEndPoint(IPAddress.Any, listenPort);
            remote = (EndPoint)(sender);

            ThreadReceive = new Thread(Receive);
            ThreadReceive.Start();
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.ToString());
        }
   
    }

    public void Receive()
    {
        data = new byte[1024];

        while (true)
        {
            recv = listener.ReceiveFrom(data, ref remote);
            Debug.Log(Encoding.UTF8.GetString(data, 0, recv));
            updateText.text = Encoding.UTF8.GetString(data, 0, recv);
            Thread.Sleep(500);
        }
    }

    public void Send(string msg) {

        byte[] data = new byte[1024];
        data = Encoding.UTF8.GetBytes(msg);
        listener.SendTo(data, data.Length, SocketFlags.None, remote);
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            string msg = "Server localhost 127.0.0.1 enviando mensaje";
            Send(msg);
            Debug.Log(msg);
        }

        //Check non-null data for each client
        foreach (Player client in currentPlayers)
        {
           
        }

    }

    private void OnDestroy()
    {
        ThreadReceive.Abort();
        listener.Close();
    }
}
