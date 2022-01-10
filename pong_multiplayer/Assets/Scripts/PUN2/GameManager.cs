using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviourPunCallbacks
{

    public GameObject ball;
    public GameObject paddle;

    private GameObject ballGO = null;

    public Text Player1ID;
    public Text Player2ID;

    //Score UI
    public TMP_Text paddle1ScoreText;
    public TMP_Text paddle2ScoreText;

    private int paddle1Score = 0;
    private int paddle2Score = 0;

    public bool playingGame = false;


    //If we reach this room CORUTINE game start and spawn a ball
    public void Start()
    {
        //Instantiate gameObjects in the room
        InitializeGame();
    }

    public void Update()
    { 
        if (ballGO != null)
            if (Input.GetKeyDown(KeyCode.Space) && PhotonNetwork.IsMasterClient)
                ballGO.GetComponent<Ball>().Launch();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    
    public override void OnPlayerEnteredRoom(Player other)
    {
        Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName.ToString()); // not seen if you're the player connecting

        if (PhotonNetwork.IsMasterClient)
        {
            Player1ID.text = PhotonNetwork.LocalPlayer.NickName;
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
        }
        else
        {
            Player2ID.text = other.NickName.ToString();
        }

    }

    //If any of our 2 players leave the room call automatically load lobby.
    public override void OnPlayerLeftRoom(Player other)
    {
        Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
        }
    }

    [PunRPC]
    public void UpdateScore(int playerID)
    {
        if (playerID == 1)
        {
            paddle1Score++;
            paddle1ScoreText.text = paddle1Score.ToString();   
        }

        if(playerID == 2)
        {
            paddle2Score++;
            paddle2ScoreText.text = paddle2Score.ToString();
        }       
    }

    [PunRPC]
    public void InitializeGame()
    {
        //Delete current ball and instance new
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(ballGO);
            ballGO = PhotonNetwork.Instantiate(this.ball.name, Vector3.zero, Quaternion.identity);
        }

        //Enable receiving inputs once again after instancing gameobjects
        playingGame = true;
    }
}