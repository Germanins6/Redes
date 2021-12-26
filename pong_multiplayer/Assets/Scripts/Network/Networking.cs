using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

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
    public enum PacketType
    {
        PT_Hello,
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

    protected Socket listener;
    protected IPEndPoint ipep;
    protected EndPoint remote;
    protected IPEndPoint sender;

    protected byte[] packageDataRcv;
    protected byte[] packageDataSnd;

    protected Thread ThreadReceive;
    protected Thread ThreadSend;

    private void Start()
    {
        packageDataSnd = new byte[1024];
        packageDataRcv = new byte[1024];

    }

    public class Client
    {

        public Client()
        {
            socket_client = null;
            id = string.Empty;
            PaddleMovement = string.Empty;
        }
        public Client(EndPoint e, string uuid)
        {
            socket_client = e;
            id = uuid;
        }

        [XmlIgnore]
        public EndPoint socket_client;

        [XmlElement("Id")]
        public string id;

        [XmlElement("Paddle_Movement")]
        public string PaddleMovement;

        [XmlElement("PackageType")]
        public string PackageType;

    }
    public class WorldReplication
    {
        //Data
        public float Paddle1Pos, Paddle2Pos;
        public Vector2 BallVel;

        public int Client1_Score, Client2_Score;
        public bool Client1_isConnected, Client2_isConnected;
    }

}
