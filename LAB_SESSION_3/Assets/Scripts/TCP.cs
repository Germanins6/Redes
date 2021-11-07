using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

public class TCP
{

    public Socket socket;
    public Socket entrySocket;
    public IPEndPoint ipep;
    byte[] data;

    public enum tcpType
    {
        SERVER = 0,
        CLIENT
    }

    public TCP(tcpType type)
    {

        data = new byte[1024];
        socket = new Socket(AddressFamily.InterNetwork, 
            SocketType.Stream, 
            ProtocolType.Tcp);
        ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6000);

        if(type == tcpType.SERVER)
        {
            socket.Bind(ipep);
            socket.Listen(3);
            socket.BeginAccept(new System.AsyncCallback(AcceptClient), socket);
        }

        if(type == tcpType.CLIENT)
            socket.Connect(ipep);
       
    }

    void AcceptClient(IAsyncResult ias)
    {
        Socket server = ias.AsyncState as Socket;
        try
        {
            Socket client = server.EndAccept(ias);
            //Thread Stuff new thread and start
        }
        catch (Exception)
        {
            throw;
        }
    }

    public void Send(string msg)
    {
        data = Encoding.UTF8.GetBytes(msg);
        socket.Send(data);
    }

    public byte[] ReceiveClient() 
    {
        byte[] bufferData = new byte[1024];
        int lenght = socket.Receive(bufferData);
        return bufferData;
    }

    public string ReceiveClientMsg()
    {
        byte[] bufferData = new byte[1024];
        int lenght = socket.Receive(bufferData);
        string msg = Encoding.UTF8.GetString(bufferData, 0, lenght);
        return msg;
    }

    public byte[] ReceiveServer(object obj)
    {
        Socket socket = obj as Socket;

        while (true)
        {
            byte[] bufferData = new byte[1024];
            int length;

            try
            {
                length = socket.Receive(bufferData);
            }
            catch(Exception)
            {
                Debug.Log("Connection Failed");
            }

            return bufferData;
        }
    }


    public string ReceiveServerMsg(object obj)
    {
        Socket socket = obj as Socket;

        while (true)
        {
            byte[] bufferData = new byte[1024];
            int length;
            string msg = "0";
            try
            {
                length = socket.Receive(bufferData);
                msg = Encoding.UTF8.GetString(bufferData, 0, length);

            }
            catch (Exception)
            {
                Debug.Log("Connection Failed");
            }

            return msg;
        }
    }

    public void Shutdown()
    {
        socket.Shutdown(SocketShutdown.Both);
        socket.Close();
    }
}
