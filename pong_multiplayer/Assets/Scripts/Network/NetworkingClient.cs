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
    Socket udp_client;
    IPEndPoint ipep, sender;
    EndPoint Remote;
    byte[] data = new byte[1024];
    Thread TwaitMsg;
    Thread TsendInp;
    int recv;
    Guid id = Guid.NewGuid();


    //timers
    float currentTime;
    float timeOut;
    float lastPing;

    //Input
    private float yBound = 3.75f;
    [SerializeField] private float speed = 7f;

    // Start is called before the first frame update
    void Start()
    {
        udp_client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6000);
        sender = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6000);
        Remote = sender;

        udp_client.Connect(ipep);

        TwaitMsg = new Thread(ReceiveMsg);
        TwaitMsg.Start();
        TsendInp = new Thread(SendMsg);
        TsendInp.Start();

    }

    // Update is called once per frame
    void Update()
    {
        //TODO
        if (Input.GetKeyDown(KeyCode.L))
            SendMsg();
    }

    void ReceiveMsg()
    {
        data = new byte[1024];
        while (true)
        {
            recv = udp_client.ReceiveFrom(data, ref Remote); //Receive data from server           
            Thread.Sleep(500);  //wait for 500ms
        }

    }

    public void SendMsg()
    {
        byte[] data_send = Encoding.UTF8.GetBytes("Cliennt " + id.ToString() + "sendinng message to server"); //Sends Ping to the server
        udp_client.SendTo(data_send, data_send.Length, SocketFlags.None, ipep); //Send data to server
    }
    //void SendInput()
    //{
    //    float movement;
    //    movement = Input.GetAxisRaw("Vertical");
    //    Vector2 paddlePosition = transform.position;
    //    paddlePosition.y = Mathf.Clamp(paddlePosition.y + movement * speed * Time.deltaTime, -yBound, yBound);
    //    transform.position = paddlePosition;
    //}

}
