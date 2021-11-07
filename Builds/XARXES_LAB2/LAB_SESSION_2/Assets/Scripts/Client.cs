using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Server libraries
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine.UI;

public class Client : MonoBehaviour
{
    UDP udpClient;
    Thread udpThread;
    public Text message;
    public Text instructions;

    // Start is called before the first frame update
    void Start()
    {
        //Initialize Client
        udpClient = new UDP(UDP.udpType.CLIENT);


        //Start threading to receive messages
        udpThread = new Thread(() => udpClient.Receive());
        udpThread.Start();
    }

    void Update()
    {
        //Update incoming messages from server
        message.text = udpClient.msgReceived;
        instructions.text = "Press A to send a message";
        //Press A keyboard button to send message to server
        if (Input.GetKeyDown(KeyCode.A))
            udpClient.SendClient("Ping");
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }
}