using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine.UI;
public class Server : MonoBehaviour
{
    UDP udpServer;
    Thread udpThread;
    public Text message;
    public Text instructions;
    // Start is called before the first frame update
    void Start()
    {
        //Initialize Server
        udpServer = new UDP(UDP.udpType.SERVER);


        //Start threading to receive messages
        udpThread = new Thread(() => udpServer.Receive());
        udpThread.Start();
    }

    void Update()
    {
        //Update incoming messages from Client
        message.text = udpServer.msgReceived;
        instructions.text = "Press B to send a message";
        //Press B in keyboard to send message to client
        if (Input.GetKeyDown(KeyCode.B))
            udpServer.SendServer("Pong");

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    


}