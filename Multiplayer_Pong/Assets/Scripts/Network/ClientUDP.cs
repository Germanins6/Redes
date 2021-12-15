using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Text;
using UnityEngine.UI;

public class ClientUDP : MonoBehaviour
{
    //UI
    public Button send;
    public InputField writeMsg;
    public Text windowMsg;
    string msgTosend;

    Thread TwaitMsg;
    Socket Socket_udp_client;
    IPEndPoint ipep, sender;
    EndPoint Remote;

    byte[] data = new byte[1024];

    int recv;
    int msg_counter;    // message counter 

    // Start is called before the first frame update
    void Start()
    {
        Socket_udp_client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);         //Create Socket

        ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6000);      //Create Ipendpoint

        sender = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6000);
        Remote = sender;

        Socket_udp_client.Connect(ipep);

        send.onClick.AddListener(SendMsg); //When click on Send Button call SendMsg function

        TwaitMsg = new Thread(ReceiveMsg);
        TwaitMsg.Start();

        msg_counter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        windowMsg.text = msgTosend;
    }

    void ReceiveMsg() {        

        data = new byte[1024];
        
        while (true)
        {
            if (msg_counter >= 5) //when we have exchange 5 messages, user can't send anything
            {
                msgTosend += "\n" + (System.DateTime.Now.ToString() + "---Disconnect from server");

                send.onClick.RemoveListener(SendMsg);
                Debug.LogWarning("Disconnect from server"); 
                TwaitMsg.Abort();       //Stop thread
            }
            else
            {
                recv = Socket_udp_client.ReceiveFrom(data, ref Remote); //Receive data from server 

                string recv_msg = "---Message Received:" + Encoding.UTF8.GetString(data, 0, recv);  //Transform data to String

                // update ui text
                msgTosend += "\n" + (System.DateTime.Now.ToString() + recv_msg);

                //Debug.Log("Message Received: " + recv_msg);

                msg_counter++;
            }
            Thread.Sleep(500);  //wait for 500ms
        } 

    }

    void SendMsg() 
    {
        byte[] data_send = Encoding.UTF8.GetBytes(writeMsg.text); //Transform string to bytes

        //only if message written is "ping" client will send the message
        if(writeMsg.text == "ping" || writeMsg.text == "Ping" || writeMsg.text == "PING")
        {
            Socket_udp_client.SendTo(data_send, data_send.Length, SocketFlags.None, ipep);          //Send data to server

            // update ui text
            msgTosend += "\n" + System.DateTime.Now.ToString() + "---Message Sent: " + Encoding.UTF8.GetString(data_send, 0, data_send.Length);
            //Debug.Log("Message Sent:" + msgTosend);
                
            writeMsg.text = ""; //Clear input
        }
        else
        {
            msgTosend += "\n" + "[WARNING]---You have to send ping";
        }
        
    }

    private void OnDestroy()    //stops main thread and close socket
    {
        TwaitMsg.Abort();       //Stop thread
        Socket_udp_client.Close();      //Close socket

        Debug.Log("Socket Closed");
    }
}
