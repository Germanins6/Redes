using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class ChatRoom : MonoBehaviour
{
    // Start is called before the first frame update
    //TCP Client;
    //TCP Server;

    public Text chatLog;
    public InputField clientChat;
    public Button sendButton;

    List<string> chatHistory;
    private string display = "";

    void Start()
    {
        //Server = new TCP(TCP.tcpType.SERVER);
        //Client = new TCP(TCP.tcpType.CLIENT);

        chatHistory = new List<string>();

        chatHistory.Add("Hi");
        chatHistory.Add("HelloMF");
        chatHistory.Add("dew");
        chatHistory.Add("fins demá");

    }

    // Update is called once per frame
    void Update()
    {
        //Reload chat canvas
        //clientText.text = Client.ReceiveClientMsg();
        //serverText.text = Server.ReceiveServerMsg(Client);

        bool doOnce = true;

        if (doOnce)
        {
            foreach(string msg in chatHistory)
            {
                display = display.ToString() + msg.ToString() + "\n";
            }
            chatLog.text = display;
            doOnce = !doOnce;
        }
    }

    private void OnDestroy()
    {
        //Server.Shutdown();
        //Client.Shutdown();
    }

    public void Send()
    {
        //Client.Send(clientChat.text);
        //Debug.Log("Sending Message to Server!");
    }

}
