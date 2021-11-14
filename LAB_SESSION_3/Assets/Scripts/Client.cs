using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Net.Sockets;
using System.IO;
using System;

public class Client : MonoBehaviour
{
    public GameObject chatContainer;
    public GameObject msgPrefab;

    private bool clientConnected;
    private TcpClient handler;
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;

    public void Connect()
    {
        //Avoid next code if our client currently connected
        if (clientConnected)
        {
            Debug.Log("Current session already connected!");
            return;
        }

        try
        {
            handler = new TcpClient("127.0.0.1", 6000);
            stream = handler.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);
            clientConnected = true;
        }
        catch(Exception e)
        {
            Debug.LogWarning(e.ToString());
        }

        //Disable button after connecting
        Destroy(GameObject.Find("ConnectButton"));
    }

    //Receive message if data available, and send if key pressed
    private void Update()
    {
        if (clientConnected)
        {
            if (stream.DataAvailable)
            {
                string data = reader.ReadLine();
                if (data != null)
                    OnIncomingData(data);
            }
        }

       
            if (Input.GetKeyDown(KeyCode.Return))
            {
                OnSend();
            }
        
    }

    //Create message prefab into our scrollview each time a streamdata received
    private void OnIncomingData(string data)
    {

        GameObject go = Instantiate(msgPrefab, chatContainer.transform) as GameObject;
        Component[] textChilds = go.GetComponentsInChildren<Text>();

        foreach (Text child in textChilds)
        {
            if (child.name == "MessageText")
                child.text = data;

            if (child.name == "MessageHour")
                child.text = DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString();
        }
    }


    public void Send(string data)
    {
        if (!clientConnected)
            return;

        writer.WriteLine(data);
        writer.Flush();
    }

    public void OnSend()
    {

        string message = GameObject.Find("InputField").GetComponent<InputField>().text;
        Send(message);

        GameObject.Find("InputField").GetComponent<InputField>().text = ""; 
    }

    //Disconnect socket and delete stream writer and reader
    public void ShutDown()
    {

        //Check if our client still connected
        if (!clientConnected)
            return;

        //Close new statements done in Connect Process
        writer.Close();
        reader.Close();
        handler.Close();

        clientConnected = false;
    }

    private void OnApplicationQuit()
    {
        ShutDown();
    }

    private void OnDisable()
    {
        ShutDown();
    }
}
