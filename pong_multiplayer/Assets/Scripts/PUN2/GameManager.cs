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

    private GameObject firstPaddle;
    private GameObject secondPaddle;

    public Text nickName;
    public Text enemyNickName;

    private const float speed = 7.0f;

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
        InitializeGO();
    }

    public void Update()
    {
        //Just get input information if we are ready to play
        if (playingGame)
            GetInput();

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
        Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
        }
        

        nickName.text = PhotonNetwork.LocalPlayer.NickName;
        enemyNickName.text = other.NickName;

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
    public void GetInput()
    {
        float movement = Input.GetAxisRaw("Vertical");

        if (PhotonNetwork.IsMasterClient)
            firstPaddle.transform.position = new Vector3(firstPaddle.transform.position.x, Mathf.Clamp(firstPaddle.transform.position.y + movement * speed * Time.deltaTime, -3.75f, 3.75f), 0.0f);
        else
            secondPaddle.transform.position = new Vector3(secondPaddle.transform.position.x, Mathf.Clamp(secondPaddle.transform.position.y + movement * speed * Time.deltaTime, -3.75f, 3.75f), 0.0f);
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
    public void Restart()
    {
        //Master client destroys all gameobjects
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.DestroyAll();

        //Instance once again all gO
        InitializeGO();
    }

    [PunRPC]
    public void InitializeGO()
    {

        //Create and assign paddles for each player
        if (PhotonNetwork.IsMasterClient)
        {
            firstPaddle = PhotonNetwork.Instantiate(this.paddle.name, new Vector3(-7.5f, 0.0f, 0.0f), Quaternion.identity);

            //Instantiate Ball
            PhotonNetwork.Instantiate(this.ball.name, Vector3.zero, Quaternion.identity);
        }
        else
        {
            secondPaddle = PhotonNetwork.Instantiate(this.paddle.name, new Vector3(7.5f, 0.0f, 0.0f), Quaternion.identity);
        }

        //Enable receiving inputs once again after instancing gameobjects
        playingGame = true;
    }
}