using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net;
using System.Net.Sockets;

public class Networking : MonoBehaviour
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void onPacketReceived(byte[] inputData, Socket fromAddress)
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void onConnectionReset(Socket fromAddres)
    {

    }

    void sendPacket(byte[] outputData, Socket toAddress)
    {

    }

    void onDisconnect()
    {

    }

    void reportError()
    {

    }

}
