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

    #region PowerUp

    IEnumerator GeneratePowerUp()
    {
        System.DateTime time1 = System.DateTime.UtcNow;

        while (true)
        {
            System.DateTime time2 = System.DateTime.UtcNow;
            System.TimeSpan diff = time2 - time1;

            int randomnumber2 = UnityEngine.Random.Range(10, 15);
            if (diff.Seconds > randomnumber2)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    if (power_upGO == null)
                    {
                        Debug.Log("Generating PowerUp");
                        power_upGO = PhotonNetwork.Instantiate(this.power_up.name, new Vector3(0.0f, 2.0f, 0.0f), Quaternion.identity);
                        //Generate random number between 0 and 5
                        int randomNumber = UnityEngine.Random.Range(0, 5);
                        Debug.Log("Random Number: " + randomNumber);
                        power_upGO.GetComponent<Ball>().Balltype = (BallType)randomNumber;
                        time1 = System.DateTime.UtcNow;

                    }
                }

            }

            yield return null;
        }
    }
    public void ApplyPowerUp(BallType type, int paddle_id)
    {
        switch (type)
        {
            case BallType.BT_IncreaseBallSize:
                Debug.Log("Increase Ball Size");
                ballGO.transform.localScale = new Vector3(0.45f, 0.45f, 0.45f);
                ballGO.GetComponent<Ball>().ballRb.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                StartCoroutine(ResetPowerUp(ballGO, BallType.BT_IncreaseBallSize, paddle_id));

                break;
            case BallType.BT_DecreaseBallSize:
                Debug.Log("Decrease Ball Size");
                ballGO.transform.localScale = new Vector3(0.075f, 0.075f, 0.075f);
                ballGO.GetComponent<Ball>().ballRb.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                StartCoroutine(ResetPowerUp(ballGO, BallType.BT_DecreaseBallSize, paddle_id));

                break;
            case BallType.BT_SpeedUpPaddle:
                Debug.Log("Speed Up Paddle");
                if (paddle_id == 1)
                    this.GetComponent<PlayerManager>().speed1 = 9.5f;
                else
                    this.GetComponent<PlayerManager>().speed2 = 9.5f;
                StartCoroutine(ResetPowerUp(null, BallType.BT_SpeedUpPaddle, paddle_id));
                break;
            case BallType.BT_SpeedDownPaddle:
                Debug.Log("Speed Down Paddle");
                if (paddle_id == 1)
                    this.GetComponent<PlayerManager>().speed1 = 3.5f;
                else
                    this.GetComponent<PlayerManager>().speed2 = 3.5f;
                StartCoroutine(ResetPowerUp(null, BallType.BT_SpeedDownPaddle, paddle_id));
                break;
            case BallType.BT_InvertRawAxis:
                Debug.Log("Invert Raw Axis");
                if (paddle_id == 1)
                    this.GetComponent<PlayerManager>().speed1 = -this.GetComponent<PlayerManager>().speed1;
                else
                    this.GetComponent<PlayerManager>().speed2 = -this.GetComponent<PlayerManager>().speed2;

                StartCoroutine(ResetPowerUp(null, BallType.BT_InvertRawAxis, paddle_id));
                break;
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
    IEnumerator ResetPowerUp(GameObject gO, BallType type, int player_id)
    {
        System.DateTime time1 = System.DateTime.UtcNow;

        while (true)
        {
            System.DateTime time2 = System.DateTime.UtcNow;
            System.TimeSpan diff = time2 - time1;
            if (diff.Seconds > 10)
            {
                switch (type)
                {
                    case BallType.BT_IncreaseBallSize:
                        if (gO != null)
                        {
                            gO.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                            gO.GetComponent<Ball>().ballRb.transform.localScale = new Vector3(1, 1, 1);
                        }
                        break;
                    case BallType.BT_DecreaseBallSize:
                        if (gO != null)
                        {
                            gO.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                            gO.GetComponent<Ball>().ballRb.transform.localScale = new Vector3(1, 1, 1);
                        }
                        break;
                    case BallType.BT_SpeedUpPaddle:
                        if (player_id == 1)
                            this.GetComponent<PlayerManager>().speed1 = 7.0f;
                        else
                            this.GetComponent<PlayerManager>().speed2 = 7.0f;
                        break;
                    case BallType.BT_SpeedDownPaddle:
                        if (player_id == 1)
                            this.GetComponent<PlayerManager>().speed1 = 7.0f;
                        else
                            this.GetComponent<PlayerManager>().speed2 = 7.0f;
                        break;
                    case BallType.BT_InvertRawAxis:
                        if (player_id == 1)
                            if (this.GetComponent<PlayerManager>().speed1 < 0.0f)
                                this.GetComponent<PlayerManager>().speed1 = -this.GetComponent<PlayerManager>().speed1;
                            else
                        if (this.GetComponent<PlayerManager>().speed1 < 0.0f)
                                this.GetComponent<PlayerManager>().speed2 = -this.GetComponent<PlayerManager>().speed2;
                        break;
                }
                //time1 = System.DateTime.UtcNow;

            }
            yield return null;
        }
    }

    #endregion

    #region Photon Callbacks

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

            ballGO = PhotonNetwork.Instantiate(this.ball.name, Vector3.zero, Quaternion.identity);
        }

        //Enable receiving inputs once again after instancing gameobjects
        playingGame = true;
    }

    #endregion
}