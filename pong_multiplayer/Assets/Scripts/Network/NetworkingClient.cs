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

    //Receive the current state of a client
    public enum ClientState
    {
        CS_Error = -1,
        CS_Initialize,
        CS_Playing,
        CS_Disconnected,
        CS_Max
    }
  
    //Each client get assigned a UUID
    Guid id = Guid.NewGuid();
    ClientState playerState;

    #region PongStuff
    //timers
    float currentTime;
    float timeOut;
    float lastPing;

    //Input
    private float yBound = 3.75f;
    [SerializeField] private float speed = 7f;
    #endregion PongStuff

    // Client Start connection
    void Start()
    {
        try {
            listener.Connect(ipep);

            ThreadRecv = new Thread(ReceiveMsg);
            ThreadRecv.Start();
            ThreadSnd = new Thread(SendMsg);
            ThreadSnd.Start();
        }
        catch(Exception e)
        {
            Debug.LogWarning(e);
        }
        

        playerState = ClientState.CS_Initialize;
    }

    // Update is called once per frame
    void Update()
    {
        //If you press mouse right click send message to server with your UUID
        if (Input.GetMouseButtonDown(1))
            SendMsg();
    }

    void ReceiveMsg()
    {
        data = new byte[1024];
        while (true)
        {
            recv = listener.ReceiveFrom(data, ref remote); //Receive data from server           
            Thread.Sleep(500);  //wait for 500ms
        }

    }

    public void SendMsg()
    {
        byte[] data_send = Encoding.UTF8.GetBytes("Client " + id.ToString() + "sending message to server"); //Sends Ping to the server
        listener.SendTo(data_send, data_send.Length, SocketFlags.None, ipep); //Send data to server
    }

    public ClientState PlayerState() { return playerState; }

#region InputSend
    /*
    void SendInput()
    {
        float movement;
        movement = Input.GetAxisRaw("Vertical");
        Vector2 paddlePosition = transform.position;
        paddlePosition.y = Mathf.Clamp(paddlePosition.y + movement * speed * Time.deltaTime, -yBound, yBound);
        transform.position = paddlePosition;
    }
    */
#endregion InputSend

}
