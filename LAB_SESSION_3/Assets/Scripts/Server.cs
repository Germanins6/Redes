using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class Server : MonoBehaviour
{

    TCP serverTcp;

    public Text chatLog;
    public InputField inputChat;
    public Button sendButton;

    List<string> chatHistory;
    private string display = "";

    void Start()
    {
        //Initialize server and chatlog to receive incoming msgs
        serverTcp = new TCP(TCP.tcpType.SERVER, "127.0.0.1", 6000);
        
        chatHistory = new List<string>();
    }

    // Update is called once per frame
    void Update()
    {
        //Reload chat canvas
        chatLog.text = serverTcp.ReceiveServerMsg();
    }

    private void OnDestroy()
    {
        serverTcp.Shutdown();
    }

    public void Send()
    {
        serverTcp.Send(inputChat.text);
        Debug.Log("Sending message to Client");
    }

    public bool MsgPool()
    {
        bool doOnce = true;

        if (doOnce)
        {
            foreach (string msg in chatHistory)
            {
                display = display.ToString() + msg.ToString() + "\n";
            }
            chatLog.text = display;
            doOnce = !doOnce;
        }

        return doOnce;
    }

}
