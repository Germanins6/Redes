using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class Launcher : MonoBehaviourPunCallbacks
{

    [SerializeField]
    private byte maxPlayersPerRoom = 2;

    [SerializeField]
    private GameObject controlPanel;
    [SerializeField]
    private GameObject roomPanel;
    [SerializeField]
    private GameObject CreateRoomPanel;
    [SerializeField]
    private GameObject RoomButton;

    [SerializeField]
    private GameObject progressLabel;
    private Text progressText;

    [SerializeField]
    private Text roomName;

    //Button connectToNewRoom;

    string gameVersion = "1";

    bool isConnecting;


    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        progressText = progressLabel.GetComponent<Text>();
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
        roomPanel.SetActive(false);
        CreateRoomPanel.SetActive(false);

    }

    public void Connect()
    {
        progressLabel.SetActive(false);
        controlPanel.SetActive(false);
        roomPanel.SetActive(true);

        if (PhotonNetwork.IsConnected)
        {

        }

        else
        {
            isConnecting = PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }

    #region MonoBehaviourPunCallbacks Callbacks
    public override void OnConnectedToMaster()
    {
        if (isConnecting)
        {
            isConnecting = false;
            Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");
        }
    }

    public void OnClickCreateRoomButton()
    {
        CreateRoomPanel.SetActive(true);
        roomPanel.SetActive(false);

    }

    public void OnClickJoinRoom()
    {
        CreateRoomPanel.SetActive(false);
        roomPanel.SetActive(false);
        PhotonNetwork.JoinRoom(roomName.text);

    }
    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(roomName.text, new RoomOptions { MaxPlayers = 0 });
        Debug.Log(roomName.text);
        Instantiate(RoomButton);
        roomPanel.SetActive(true);
        CreateRoomPanel.SetActive(false);

    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
        roomPanel.SetActive(false);
        CreateRoomPanel.SetActive(false);

        isConnecting = false;

        Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
    }

    //Joined to lobby
    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room.");


        Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);
        
        foreach(KeyValuePair<int,Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            progressText.text += " " + player.Value.NickName;
        }
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("OnCreatedRoom() called by PUN. Now this client has created a room.", this);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.Log(message);
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //If We reach 2 players into our lobby load arena for both 
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            Debug.Log("We load the arena when 2 players ready");
            PhotonNetwork.LoadLevel("Arena");
        }
        base.OnPlayerEnteredRoom(newPlayer);
    }
    #endregion
}
