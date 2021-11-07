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

public class ServerTcp : MonoBehaviour 
{

    public Socket socket;
    public Socket client;
    public Socket server;
    public IPEndPoint ipep;
    private byte[] data;
    private string msgReceived;
    public Text message;
    public Text countdown;
    public Thread th;

    int countMessages;
    bool send_receiveMessages;
    // Start is called before the first frame update
    void Start()
    {
        countMessages = 0;
        send_receiveMessages = true;

        data = new byte[1024];
        server = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream,
                    ProtocolType.Tcp);
        ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6000);
        server.Bind(ipep);
        server.Listen(5);
       server.BeginAccept(new AsyncCallback(AcceptCl), server);
    }

    void AcceptCl(IAsyncResult ias)
    {
        Socket server = ias.AsyncState as Socket;
        try
        {
            Socket client = server.EndAccept(ias);
            th = new Thread(Receive);
            th.Start(client);
        }
        catch (Exception)
        {
             throw;
        }
      

    }

     void Receive(object obj)
    {
         socket = obj as Socket;
        while (send_receiveMessages)
        {
            byte[] buffer = new byte[1024];
            int length = 0;
            
            try
            {
                length = socket.Receive(buffer);
            }
            catch (Exception)
            {
                Debug.Log("Connection failed.. trying again...");
                break;
            }

            msgReceived = Encoding.UTF8.GetString(buffer, 0, length);
            Console.WriteLine(msgReceived);
            Thread.Sleep(500);
            Send("ping");
            msgReceived = "Waiting for client message";
            countMessages++;

            if (countMessages >= 5)
            {
                send_receiveMessages = false;
                msgReceived = "Simulation finished";

            }

        }
    }

     void Send(string msgReceived)
    {
        string msg = "Ping";
        byte[] data = Encoding.UTF8.GetBytes(msg);
        socket.Send(Encoding.UTF8.GetBytes(msgReceived));
    
    }
    // Update is called once per frame
    void Update()
    {
        message.text = msgReceived;

        if (send_receiveMessages)
            countdown.text = "Current loop: " + countMessages.ToString();
        else
        {
            countdown.text = "Connection Finished";
            Thread.Sleep(1500);
            server.Shutdown(SocketShutdown.Both);   
        }


        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }


    void OnDestroy()
    {
        server.Close();
        th.Abort();
        Application.Quit();
    }
}
