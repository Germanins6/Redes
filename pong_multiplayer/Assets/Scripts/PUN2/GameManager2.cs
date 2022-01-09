using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

using UnityEngine.UI;
using TMPro;

public class GameManager2 : MonoBehaviourPunCallbacks
{
    public GameObject ball;
    public GameObject paddle;

    public GameObject firstPaddle;
    public GameObject secondPaddle;

    public Text nickName;
    public Text enemyNickName;

    private float speed;

    //Score UI
    public TMP_Text paddle1ScoreText;
    public TMP_Text paddle2ScoreText;

    public int paddle1Score;
    public int paddle2Score;


    //If we reach this room CORUTINE game start and spawn a ball
    public void Start()
    {
        speed = 7.0f;
        paddle1Score = 0;
        paddle2Score = 0;

        if (PhotonNetwork.IsMasterClient)
        {
            SpawnBall();

            //Instantiate both paddles
            firstPaddle = PhotonNetwork.Instantiate(this.paddle.name, new Vector3(-7.5f, 0.0f, 0.0f), Quaternion.identity);
            secondPaddle = null;

        }
        else
        {
            //Instantiate only one paddle
            firstPaddle = null;
            secondPaddle = PhotonNetwork.Instantiate(this.paddle.name, new Vector3(7.5f, 0.0f, 0.0f), Quaternion.identity);
        }


        nickName.text = PhotonNetwork.LocalPlayer.NickName;

        //We find enemy id
        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            if (player.Value.NickName != PhotonNetwork.LocalPlayer.NickName)
                enemyNickName.text = player.Value.NickName;
        }
    }

    public void Update()
    {
        GetInput();

        if (Input.GetKeyDown(KeyCode.L))
        {

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


        enemyNickName.text = other.NickName;
        nickName.text = PhotonNetwork.LocalPlayer.NickName;

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

    public void GetInput()
    {
        float movement = Input.GetAxisRaw("Vertical");

        if (PhotonNetwork.IsMasterClient)
        {
            firstPaddle.transform.position = new Vector3(firstPaddle.transform.position.x, Mathf.Clamp(firstPaddle.transform.position.y + movement * speed * Time.deltaTime, -3.75f, 3.75f), 0.0f);
        }
        else
        {
            secondPaddle.transform.position = new Vector3(secondPaddle.transform.position.x, Mathf.Clamp(secondPaddle.transform.position.y + movement * speed * Time.deltaTime, -3.75f, 3.75f), 0.0f);
        }
    }

    public void SpawnBall()
    {
        PhotonNetwork.Instantiate(this.ball.name, Vector3.zero, Quaternion.identity);
    }

    public void Paddle1Scored()
    {
        paddle1Score++;
        paddle1ScoreText.text = paddle1Score.ToString();

    }

    public void Paddle2Scored()
    {
        paddle2Score++;
        paddle2ScoreText.text = paddle2Score.ToString();

    }

    public void Restart()
    {
        //if photon view is mine
        if (PhotonNetwork.IsMasterClient)
        {
            firstPaddle.transform.position = new Vector3(firstPaddle.transform.position.x, 0.0f, 0.0f);
            PhotonNetwork.Destroy(ball.GetComponent<PhotonView>());

        }
        else
        {
            secondPaddle.transform.position = new Vector3(secondPaddle.transform.position.x, 0.0f, 0.0f);
        }


        //modify ball position photon 

        Debug.Log(ball.transform.position.x.ToString() + " " + ball.transform.position.y.ToString());
    }

}


