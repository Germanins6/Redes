using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

public class Networking : MonoBehaviour
{
    public const int listenPort = 6000;
    public IPAddress address = IPAddress.Parse("127.0.0.1");

    public Socket listener;
    public Thread ThreadRecv;
    public Thread ThreadSnd;

    public IPEndPoint ipep, sender;
    public EndPoint remote;

    public int recv;
    public byte[] data;
  
    // Start is called before the first frame update
    void Start()
    {
        try
        {
            listener = new Socket(AddressFamily.InterNetwork,
                            SocketType.Dgram, ProtocolType.Udp);

            ipep = new IPEndPoint(address, listenPort);
            sender = new IPEndPoint(address, listenPort);
            remote = (EndPoint)(sender);

        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
        }
    }

    void onPacketReceived(byte[] inputData, Socket fromAddress)
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void onConnectionReset(Socket fromAddres)
    {

    }

    void sendPacket(byte[] outputData, Socket toAddress)
    {

    }

    void onDisconnect()
    {

    }

    void reportError()
    {

    }

}
