using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{

    [SerializeField] private float initialVelocity = 4f;
    [SerializeField] private float velocityMultiplier = 1.1f;

    public GameObject gamemanager;
    private Rigidbody2D ballRb;

    public AudioClip wallBlip;
    public AudioClip pointBlip;
    public AudioClip paddleBlip;
    public AudioClip launchBlip;


    void Start()
    {
        gamemanager = GameObject.Find("GameManager");
        ballRb = GetComponent<Rigidbody2D>();
        //Launch();
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
            this.GetComponent<AudioSource>().PlayOneShot(paddleBlip);
            ballRb.velocity *= velocityMultiplier;
        }
        else
        {
            this.GetComponent<AudioSource>().PlayOneShot(wallBlip);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        this.GetComponent<AudioSource>().PlayOneShot(pointBlip);

        //If we collide with some wall stop game receiving input, destroy gameobjects for each player, add point and restart
        gamemanager.GetComponent<GameManager>().playingGame = false;

        if (collision.gameObject.CompareTag("Goal2"))
        {
            gamemanager.GetComponent<GameManager>().UpdateScore(2);
            gamemanager.GetComponent<GameManager>().InitializeGame();
        }
        
        if(collision.gameObject.CompareTag("Goal1"))
        {
            gamemanager.GetComponent<GameManager>().UpdateScore(1);
            gamemanager.GetComponent<GameManager>().InitializeGame();
        }

    }
}
