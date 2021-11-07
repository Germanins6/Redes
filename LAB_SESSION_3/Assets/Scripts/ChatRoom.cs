using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class ChatRoom : MonoBehaviour
{
    // Start is called before the first frame update
    TCP Client;
    TCP Server;

    public Text clientText;
    public Text serverText;

    public InputField clientChat;
    public Button sendButton;

    void Start()
    {
        Client = new TCP(TCP.tcpType.CLIENT);
        Server = new TCP(TCP.tcpType.SERVER);
    }

    // Update is called once per frame
    void Update()
    {
        //Reload chat canvas
        //clientText.text = Client.ReceiveClientMsg();
        //serverText.text = Server.ReceiveServerMsg(Client);
    }

    private void OnDestroy()
    {
        Client.Shutdown();
        Server.Shutdown();
    }

    public void Send()
    {
        Client.Send(clientChat.text);
        Debug.Log("Sending Message to Server!");
    }

}
