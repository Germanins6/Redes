using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BallType
{
    BT_None = -1,

    BT_IncreaseBallSize,
    BT_DecreaseBallSize,
    BT_SpeedUpPaddle,
    BT_SpeedDownPaddle,
    BT_InvertRawAxis,
    BT_Max
}
public class Ball : MonoBehaviour
{

    [SerializeField] public float initialVelocity = 2f;
    [SerializeField] private float velocityMultiplier = 1.1f;

    public GameObject gamemanager;
    public Rigidbody2D ballRb;

    public AudioClip wallBlip;
    public AudioClip pointBlip;
    public AudioClip paddleBlip;
    public AudioClip launchBlip;

    public BallType Balltype;

    public Ball(BallType type)
    {
        Balltype = type;
    }

    void Start()
    {

        gamemanager = GameObject.Find("GameManager");
        ballRb = GetComponent<Rigidbody2D>();

        //Corutina para lanzar TODO
        Launch();
    }


    //Initial launch of the ball
    public void Launch()
    {
        float xVelocity = Random.Range(0, 2) == 0 ? 1 : -1;
        float yVelocity = Random.Range(0, 2) == 0 ? 1 : -1;
        ballRb.velocity = new Vector2(xVelocity, yVelocity) * initialVelocity;

        this.GetComponent<AudioSource>().PlayOneShot(launchBlip);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Paddle"))
        {
            if (Balltype == BallType.BT_None)
            {

                this.GetComponent<AudioSource>().PlayOneShot(paddleBlip);
                ballRb.velocity *= velocityMultiplier;
            }
            else
            {
                this.GetComponent<AudioSource>().PlayOneShot(paddleBlip);
                CheckPowerUp(collision.gameObject);
            }
        }
        else
        {
            this.GetComponent<AudioSource>().PlayOneShot(wallBlip);
        }
    }

    private void CheckPowerUp(GameObject gO)
    {
        if (Balltype == BallType.BT_DecreaseBallSize || Balltype == BallType.BT_IncreaseBallSize)
            gamemanager.GetComponent<GameManager>().ApplyPowerUp(Balltype, -1);
        else
        {
            if (gO.transform.position.x < 0.0f)
                gamemanager.GetComponent<GameManager>().ApplyPowerUp(Balltype, 1);
            else
                gamemanager.GetComponent<GameManager>().ApplyPowerUp(Balltype, 2);
        }

        gamemanager.GetComponent<GameManager>().DestroyPowerUp();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        this.GetComponent<AudioSource>().PlayOneShot(pointBlip);

        //If we collide with some wall stop game receiving input, destroy gameobjects for each player, add point and restart
        gamemanager.GetComponent<GameManager>().playingGame = false;

        if (collision.gameObject.CompareTag("Goal2"))
        {
            if (Balltype == BallType.BT_None)
            {
                gamemanager.GetComponent<GameManager>().UpdateScore(2);
                gamemanager.GetComponent<GameManager>().InitializeGame();
            }
            else
            {
                gamemanager.GetComponent<GameManager>().DestroyPowerUp();
            }

        }

        if (collision.gameObject.CompareTag("Goal1"))
        {
            if (Balltype == BallType.BT_None)
            {
                gamemanager.GetComponent<GameManager>().UpdateScore(1);
                gamemanager.GetComponent<GameManager>().InitializeGame();
            }
            else
            {
                gamemanager.GetComponent<GameManager>().DestroyPowerUp();
            }

        }

    }
}
