using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;


using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using UnityEngine.UI;

public class ClientTcp : MonoBehaviour
{
    public Socket client;
    public IPEndPoint ipep;
    string msgReceived;
    public Text message;
    public Text countdown;
    public Thread clientThread;

    int countMessages;
    bool send_receiveMessages;

    // Start is called before the first frame update
    void Start()
    {
        countMessages = 0;
        send_receiveMessages = true;

        //Initialize the socket with the current address
        client = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream,
                    ProtocolType.Tcp);
        ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6000);
        client.Connect(ipep);
        //Start threading to receive messages
        clientThread = new Thread(new ThreadStart(Receive));
        clientThread.Start();
        Thread.Sleep(1500);
        Send("Ping");
        //As the client, it's the first one to send a message to the server and then begins the loop
    }

    void Receive()
    {
       
        while (send_receiveMessages)
        { 
            //The client receive the message as many as the countdown dictates, waits 500 ms and then sends the pong (Loop)
            byte[] buffer = new byte[1024];
            int length = client.Receive(buffer);
            msgReceived = Encoding.UTF8.GetString(buffer, 0, length);
            Thread.Sleep(500);
            Send("Pong");
            msgReceived = "Waiting for server message";
            countMessages++;

            Debug.Log("Current Messages: " + countMessages);

            if (countMessages >= 5)
            {
                send_receiveMessages = false;
                msgReceived = "Simulation finished";
                //Ends the loop
            }

        }
    
    }

    void Send(string msgReceived)
    {
        client.Send(Encoding.UTF8.GetBytes(msgReceived));

    }
    // Update is called once per frame
    void Update()
    {
        message.text = msgReceived;

        if (send_receiveMessages)
            countdown.text = "Current loop: " + countMessages.ToString();
        else
        {
            //When the loop ends, the thread waits 1500ms and then calls the function OnDestroy, where it closes the socket and the application 
            countdown.text = "Connection Finished";
            Thread.Sleep(1500);
            OnDestroy();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    void OnDestroy()
    {
        client.Close();
        clientThread.Abort();
        Application.Quit();
    }
}

