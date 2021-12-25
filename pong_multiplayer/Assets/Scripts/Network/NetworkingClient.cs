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
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class NetworkingClient : Networking
{
    private UdpClient client;
    public IPEndPoint ipep;
    EndPoint remote;
    private const int listenPort = 6000;
    Thread ThreadRecv;
    Thread ThreadSnd;
    int recv;
    byte[] data = new byte[1024];
    string text;

    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;

    //Each client get assigned a UUID
    Guid id = Guid.NewGuid();

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            client = new UdpClient("127.0.0.1", 6000);
            ipep = new IPEndPoint(IPAddress.Any, listenPort);
            client.Connect(ipep);

            ThreadRecv = new Thread(ReceiveMsg);
            ThreadRecv.Start();
            ThreadSnd = new Thread(SendMsg);
            ThreadSnd.Start();

           
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);
        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
            SendMsg();

    }

    void ReceiveMsg()
    {
       
        while (true)
        {
            data = client.Receive(ref ipep); //Receive data from server
                                             //
            Thread.Sleep(500);  //wait for 500ms
        }

    }

    public void SendMsg()
    {
        byte[] data_send = Encoding.ASCII.GetBytes("Client " + id.ToString() + "sending message to server"); //Sends Ping to the server

        client.Send(data_send, data_send.Length); //Send data to server
        text = Encoding.ASCII.GetString(data_send);
        Debug.Log(text);
    }


    void SendPackets()
    {
       
    }

    void ProcessPacket(ref MemoryStream stream, ref UdpClient client)
    {
     
    }
}
