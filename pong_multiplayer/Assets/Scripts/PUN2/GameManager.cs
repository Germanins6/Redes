using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

using UnityEngine.UI;
public class GameManager : MonoBehaviourPunCallbacks
{

    public GameObject ball;
    public GameObject paddle;

    private GameObject ballGO = null;

    public Text Player1ID;
    public Text Player2ID;

    //Score UI
    public Text paddle1ScoreText;
    public Text paddle2ScoreText;

    private int paddle1Score = 0;
    private int paddle2Score = 0;

    public bool playingGame = false; 


    //If we reach this room CORUTINE game start and spawn a ball
    public void Start()
    {
        //Instantiate gameObjects in the room
        InitializeGame();

        Player1ID.text = PhotonNetwork.PlayerList[0].NickName;
        Player2ID.text = PhotonNetwork.PlayerList[1].NickName;
    }

    public void Update()
    { 
        if (ballGO != null)
            if (Input.GetKeyDown(KeyCode.Space) && PhotonNetwork.IsMasterClient)
                ballGO.GetComponent<Ball>().Launch();

        //If player presses escape button leaves application and reach lobby again
        if (Input.GetKeyDown(KeyCode.Escape))
            LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
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
            if(ballGO != null)
                PhotonNetwork.Destroy(ballGO);

            ballGO = PhotonNetwork.Instantiate(this.ball.name, Vector3.zero, Quaternion.identity);
        }

        //Enable receiving inputs once again after instancing gameobjects
        playingGame = true;
    }

    public void CreatePowerUp(BallType type)
    {

        
        switch (type)
        {
            case BallType.BT_IncreaseBallSize:
                break;
            case BallType.BT_DecreaseBallSize:
                break;
        }
    }
}