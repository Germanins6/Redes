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

    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        //Initialize UDP server
        try
        {
            listener = new UdpClient(listenPort);
            ipep = new IPEndPoint(IPAddress.Any, listenPort);

         
            //StartListening();

            ThreadReceive = new Thread(StartListening);
            ThreadReceive.Start();

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
        Debug.Log("Starting async listenning");
        UdpClient listener = ias.AsyncState as UdpClient;
        IPEndPoint ipe = ias.AsyncState as IPEndPoint;

        byte[] packageReceive = listener.EndReceive(ias, ref ipe);
        Debug.Log(Encoding.ASCII.GetString(packageReceive));

        Debug.Log(ipe.Address.ToString() + ipe.Port.ToString());


        //StartListening();
    }

    //TODO 2: Make udp port receive permanent entry connections(just 2 players)
    private void StartListening()
    {
        Debug.Log("HOLA");
        listener.BeginReceive(new AsyncCallback(AcceptClients), listener);
    }

    //TODO 3: Send serialized data to each player in game session
    private void Broadcast()
    {
       //TODO Dictionary
    }

    // Update is called once per frame
    void Update()
    {
    }

    void ReceiveMsg()
    {  
        while (true)
        {
            packageDataRcv = listener.Receive(ref ipep);
            Thread.Sleep(50);
        }
    }

    Guid GenerateID()
    {
        return Guid.NewGuid();
    }

    
}
