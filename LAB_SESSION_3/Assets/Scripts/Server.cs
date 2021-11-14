using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;
using System.Net.Sockets;
using System.Net;
using System.IO;

public class Server : MonoBehaviour
{
    private List<Player> clients;
    private List<Player> disconnectedClients;
    private List<string> commands;


    private int port = 6000;
    public TcpListener listener;
    private bool initialized;

    public enum COMMAND
    {
        ERROR = 0,
        HELP,
        LIST,
        WHISPER,
        NAME,
        MAX_COMMANDS
    }

    private void Start()
    {
        clients = new List<Player>();
        disconnectedClients = new List<Player>();
        commands = new List<string>();


        //Initialize server
        try
        {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();

            StartListening();
            initialized = true;

            Debug.Log("Server started on port: " + port.ToString());
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.ToString());
        }

        commands.Add("/help");
        commands.Add("/list");    
        commands.Add("/whisper");
        commands.Add("/changeName");
    }

    private void Update()
    {
        if (!initialized)
            return;

        //Check non-null data for each client connected
        foreach (Player c in clients)
        {
            //Verify is we have still players connected
            if (!IsConnected(c.client))
            {
                c.client.Close();
                disconnectedClients.Add(c);
                continue;
            }
            else
            {
                NetworkStream s = c.client.GetStream();
                if (s.DataAvailable)
                {
                    StreamReader reader = new StreamReader(s, true);
                    string data = reader.ReadLine();

                    if (data != null)
                    {
                        OnIncomingData(c, data);
                    }
                }
            }
        }

        //Detect and broadcast disconnecting message for each client in the list
        for (int i = 0; i < disconnectedClients.Count - 1; i++)
        {
            BroadCast(disconnectedClients[i].clientName + "<color=red> disconnected</color>", clients); 

            clients.Remove(disconnectedClients[i]);
            disconnectedClients.RemoveAt(i);
        }
    }

    public void StartListening()
    {
        listener.BeginAcceptTcpClient(AcceptClient, listener);
    }

    //Receive incoming connection and register user into our connected list
    public void AcceptClient(IAsyncResult ias)
    {
        TcpListener listener = ias.AsyncState as TcpListener;
        Player client = new Player(listener.EndAcceptTcpClient(ias));
        clients.Add(client);
        
        StartListening();

        BroadCast(clients[clients.Count - 1].clientName + " <color=green>has connected</color>", clients);
        BroadCastClient("Welcome! Use <i>/help</i> command for more information and use <i>/changeName</i> to set your username", client);
    }

    //Check if any client its still connected sending a poll to the client socket
    public bool IsConnected(TcpClient c)
    {
        try
        {
            if (c != null && c.Client != null && c.Client.Connected)
            {
                if (c.Client.Poll(0, SelectMode.SelectRead))
                    return !(c.Client.Receive(new byte[1], SocketFlags.Peek) == 0);

                return true;
            }
            else
                return false;

        }
        catch
        {
            return false;
        }
    }

    //When data received operate, by default broadcast to all, if we find any command then alternate between options
    public void OnIncomingData(Player client, string data)
    {
        //Gets data and stores just the command /xxxx;
        string commandOrder = SplitCommand(data)[0];

        //String to operate depending case
        string dataStored = string.Empty;

        //Make sure ur doing a command petition and dont broadcast this info to other clients
        if (data.StartsWith("/"))
        {

            switch (CommandString(commandOrder))
            {
                case COMMAND.HELP:

                    //Stores all commands into an string and just broadcast to client asking for
                    foreach (string command in commands)
                        dataStored += " " + command;
                    BroadCastClient(dataStored, client);
                    break;

                case COMMAND.LIST:

                    foreach (Player c in clients)
                        dataStored += " " + c.clientName;
                    BroadCastClient(dataStored, client);
                    break;

                case COMMAND.WHISPER:
                    //Create temporal list between source and destination and broadcast that message through the created list
                    List<Player> whisperList = new List<Player>();
                    whisperList.Add(client);
                    whisperList.Add(SearchUserConnected(data,clients));
                    BroadCast(data, whisperList);
                    break;

                case COMMAND.NAME:
                    //Get name separated by space in command prompt and checks if any connected user already using Ex. /changeName H3Dio
                    dataStored = SplitCommand(data)[1];

                    if (CheckUserNames(dataStored))
                    {
                        client.SetName(SplitCommand(data)[1]);
                        BroadCastClient("Your name has been updated to: " + client.clientName, client);
                    }
                    else
                    {
                        BroadCastClient("Your name it's already being <color=red>used by other user</color> in server. Try again", client);
                    }
                    break;
            }            
            return;
        }

        BroadCast("<i>" + client.clientName + "</i> sent: " + data, clients);
    }

    //Server shares message with current users connected
    public void BroadCast(string data, List<Player> clients)
    {
        foreach(Player c in clients)
        {
            try
            {
                StreamWriter write = new StreamWriter(c.client.GetStream());
                write.WriteLine(data);
                write.Flush();
            }
            catch(Exception e)
            {
                Debug.LogWarning(e.ToString());
            }
        }
    }

    //Server shares message only to client asking for something
    public void BroadCastClient(string data, Player client)
    {
        try
        {
            StreamWriter write = new StreamWriter(client.client.GetStream());
            write.WriteLine(data);
            write.Flush();
        } 
        catch(Exception e)
        {
            Debug.LogWarning(e.ToString());
        }
    }

    //Returns user searched by name
    public Player SearchUserConnected(string data, List<Player> clients)
    {
        Player destination = null;

        //Separate string data split "/whisper Client2" to /whisper + Client2;
        foreach(Player dest in clients)
        {
            //If you find a coincidence between names send a message just for the connected client
            if (SplitCommand(data)[1] == dest.clientName)
            {
                Debug.Log(SplitCommand(data)[1]);
                return dest;
            }
        }
 
        return destination;
    }

    //Returns proper command depending our data input if we start data with "/" char
    public COMMAND CommandString(string data)
    {
        COMMAND order = COMMAND.ERROR;

        switch (data)
        {
            case "/help":
                order = COMMAND.HELP;
                break;

            case "/list":
                order = COMMAND.LIST;
                break;

            case "/whisper":
                order = COMMAND.WHISPER;
                break;

            case "/changeName":
                order = COMMAND.NAME;
                break;
        }
        return order;
    }

    //Return false if picked name already exist in our server
    public bool CheckUserNames(string name)
    {
        foreach (Player client in clients)
            if (name == client.clientName)
                return false;

        return true;
    }

    //Break string command into 2 or more string separated between spaces into an array to access Colours, Names, Clients...
    public string[] SplitCommand(string data)
    {
        return data.Split(' ');
    }

}

//Own class to store our client name
public class Player
{
    public TcpClient client;
    public Guid playerUUID;
    public string clientName;
    
    public Player(TcpClient socket)
    {
        playerUUID = Guid.NewGuid();
        clientName = "Guest-" + playerUUID.ToString();
        client = socket;
    }

    public Guid GetUUID() { return playerUUID; }
    public void SetName(string name) { clientName = name; }
}