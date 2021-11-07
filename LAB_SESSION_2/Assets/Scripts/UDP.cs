using UnityEngine;

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class UDP
{
    public enum udpType
    {
        CLIENT = 0,
        SERVER,
    }

    public Socket socket;
    public IPEndPoint ipep;
    public IPEndPoint sender;
    public EndPoint Remote;
    public byte[] data;
    public string msgReceived;

    public UDP(udpType type)
    {

       
        ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6000);

        sender = new IPEndPoint(IPAddress.Any, 0);
        Remote = (EndPoint)(sender);

        switch (type)
        {
            case udpType.CLIENT:
                socket.Connect(ipep);
                break;

            case udpType.SERVER:
                socket.Bind(ipep);
                break;
        }
    }

    public void SendServer(string message = "pong")
    {
        data = Encoding.ASCII.GetBytes(message);
        socket.SendTo(data, data.Length, SocketFlags.None, Remote);
        Debug.Log("Message: " + message + " ! Sent by Server");
    }

    public void SendClient(string message = "ping")
    {
        data = Encoding.ASCII.GetBytes(message);
        socket.SendTo(data, data.Length, SocketFlags.None, ipep);
        Debug.Log("Message: " + message + " ! Sent by Client");
    }

    public void Receive()
    {
        data = new byte[1024];

        while (true)
        {
            int recv = socket.ReceiveFrom(data, ref Remote);
            Debug.Log("Message: received");
            Debug.Log(Encoding.ASCII.GetString(data, 0, recv));
            msgReceived = Encoding.ASCII.GetString(data, 0, recv);
        }

    }
}