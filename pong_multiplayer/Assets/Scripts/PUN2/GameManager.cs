using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

using UnityEngine.UI;





public class GameManager : MonoBehaviourPunCallbacks
{

    public GameObject power_upGO = null;
    public GameObject power_up;

    public GameObject paddle;
    public GameObject ball;

    public GameObject ballGO = null;

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


        StartCoroutine(GeneratePowerUp());
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

    IEnumerator ResetPowerUp(GameObject gO)
    {
        System.DateTime time1 = System.DateTime.UtcNow;

        while (true)
        {
            System.DateTime time2 = System.DateTime.UtcNow;
            System.TimeSpan diff = time2 - time1;
            if (diff.Seconds > 10)
            {
                switch (gO.GetComponent<Ball>().Balltype)
                {
                    case BallType.BT_IncreaseBallSize:
                        gO.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                        gO.GetComponent<Ball>().ballRb.transform.localScale = new Vector3(1, 1, 1);
                        break;
                    case BallType.BT_DecreaseBallSize:
                        gO.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                        gO.GetComponent<Ball>().ballRb.transform.localScale = new Vector3(1, 1, 1);
                        break;
                    //case BallType.BT_SpeedUpPaddle:
                    //    break;
                    //case BallType.BT_SpeedDownPaddle:
                    //    break;
                    //case BallType.BT_InvertRawAxis:
                    //    break;
                    default:
                        break;
                }
                StopCoroutine(ResetPowerUp(gO));
                time1 = System.DateTime.UtcNow;

            }
            yield return null;
        }
    }

    IEnumerator GeneratePowerUp()
    {
        System.DateTime time1 = System.DateTime.UtcNow;
        //Debug.Log("Starting Coroutine!");

        //if time passed is greater than 10 seconds, update vertices
        while (true)
        {
            System.DateTime time2 = System.DateTime.UtcNow;
            System.TimeSpan diff = time2 - time1;

            if (diff.Seconds > 5)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    if (power_upGO == null)
                    {
                        Debug.Log("Generating PowerUp");
                        power_upGO = PhotonNetwork.Instantiate(this.power_up.name, new Vector3(0.0f, 2.0f, 0.0f), Quaternion.identity);
                        //Generate random number between 0 and 1
                        int randomNumber = UnityEngine.Random.Range(0, 1);
                        power_upGO.GetComponent<Ball>().Balltype = (BallType)randomNumber;
                        time1 = System.DateTime.UtcNow;

                    }
                }

            }

            yield return null;
        }
    }

    public void DestroyPowerUp()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Destroying PowerUp");

            if (power_upGO != null)
                PhotonNetwork.Destroy(power_upGO);

            power_upGO = null;

        }

    }

    public void ApplyPowerUp(BallType type)
    {
        switch (type)
        {
            case BallType.BT_IncreaseBallSize:
                Debug.Log("Increase Ball Size");
                ballGO.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                ballGO.GetComponent<Ball>().ballRb.transform.localScale = new Vector3(2, 2, 2);
                StartCoroutine(ResetPowerUp(ballGO));
                break;
            case BallType.BT_DecreaseBallSize:
                Debug.Log("Decrease Ball Size");
                ballGO.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
                ballGO.GetComponent<Ball>().ballRb.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                StartCoroutine(ResetPowerUp(ballGO));
                break;
            //case BallType.BT_SpeedUpPaddle:
            //    break;
            //case BallType.BT_SpeedDownPaddle:
            //    break;
            //case BallType.BT_InvertRawAxis:
            //    break;
            default:
                break;
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

        if (playerID == 2)
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
            if (ballGO != null)
                PhotonNetwork.Destroy(ballGO);

            /*if (power_upGO != null)
                PhotonNetwork.Destroy(power_upGO);*/

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