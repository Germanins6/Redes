using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class Networking : MonoBehaviour
{

    protected enum ClientState
    {
        CS_Error = -1,
        CS_Connected,
        CS_Playing,
        CS_Disconnected,
        CS_Max
    }

    //Flag to identify dgram type and process stream
    protected enum PacketType
    {
        PT_Welcome,
        PT_Acknowledge,
        PT_InputData,
        PT_ReplicationData,
        PT_Disconnect,
        PT_Max
    }

    //localhost:6000
    protected IPAddress ip = IPAddress.Parse("127.0.0.1");
    protected const int listenPort = 6000;

    protected UdpClient listener;
    protected IPEndPoint ipep;
    protected EndPoint remote;

    protected byte[] packageDataRcv;
    protected byte[] packageDataSnd;

    protected Thread ThreadReceive;
    protected Thread ThreadSend;

    private void Start()
    {
        packageDataSnd = new byte[1024];
        packageDataRcv = new byte[1024];
    }
}
