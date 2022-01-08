using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

using UnityEngine.UI;


public class GameManager2 : MonoBehaviourPunCallbacks
{
    public GameObject prefab;
    public Text nickName;
    public Text enemyNickName;


    //If we reach this room CORUTINE game start and spawn a ball
    public void Start()
    {

        if(prefab != null)
            PhotonNetwork.Instantiate(this.prefab.name, Vector3.zero, Quaternion.identity);



        nickName.text = PhotonNetwork.LocalPlayer.NickName;

        //We find enemy id
        foreach(KeyValuePair<int,Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            if (player.Value.NickName != PhotonNetwork.LocalPlayer.NickName)
                enemyNickName.text = player.Value.NickName;
        }
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }
    
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    
    void LoadArena()
    {
        if (!PhotonNetwork.IsMasterClient)
            Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");

        //PhotonNetwork.LoadLevel(0);

        Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);
        //PhotonNetwork.LoadLevel("Arena");
    }

    public override void OnPlayerEnteredRoom(Player other)
    {
        Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
            LoadArena();
        }
    }

    //If any of our 2 players leave the room call automatically load lobby.
    public override void OnPlayerLeftRoom(Player other)
    {
        Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
            LoadArena();
        }
    }
}