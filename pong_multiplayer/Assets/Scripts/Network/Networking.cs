using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Networking : MonoBehaviour
{

    enum ClientState
    {
        CS_Error = -1,
        CS_Connected,
        CS_Playing,
        CS_Disconnected,
        CS_Max
    }

    //Flag to identify dgram type and process stream
    enum PacketType
    {
        PT_Welcome,
        PT_Acknowledge,
        PT_TimeOutCheck,
        PT_ReplicationData,
        PT_Disconnect,
        PT_Max
    }

}
