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


    //Dictionary<EndPoint, Client>;
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

            //byte[] packageReceive = listener.EndReceive(ias, ref ipe);
            //Debug.Log(Encoding.ASCII.GetString(packageReceive));

            ThreadReceive = new Thread(ReceiveMsg);
            ThreadReceive.Start(ipe);
        }
        catch (System.Exception e)
        {
            Debug.Log("Exception: " + e.Message);
        }



        listener.BeginReceive(new AsyncCallback(AcceptClients), listener);
    }

    private void Broadcast()
    {
        //TODO Dictionary
    }

    // Update is called once per frame
    void Update()
    {
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

    Guid GenerateID()
    {
        return Guid.NewGuid();
    }


}