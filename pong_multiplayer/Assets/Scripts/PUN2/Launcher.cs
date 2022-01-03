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
    private GameObject progressLabel;
    private Text progressText;

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
    }

    public void Connect()
    {
        progressLabel.SetActive(true);
        controlPanel.SetActive(false);

        if (PhotonNetwork.IsConnected)
            PhotonNetwork.JoinRandomRoom();
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
            PhotonNetwork.JoinRandomRoom();
            isConnecting = false;
            Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");
        }
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
        isConnecting = false;

        Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        //Instead using this: -->  PhotonNetwork.CreateRoom(null, new RoomOptions());
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });

    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room.");

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            Debug.Log("We load the arena when 2 players ready");
            PhotonNetwork.LoadLevel("Arena");
        }

        Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);
        progressText.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString();
    }
    #endregion
}
