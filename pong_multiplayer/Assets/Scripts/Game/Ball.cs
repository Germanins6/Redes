using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private float initialVelocity = 4f;
    [SerializeField] private float velocityMultiplier = 1.1f;

    public GameObject gamemanager;
    private Rigidbody2D ballRb;

    void Start()
    {
        gamemanager = GameObject.Find("GameManager");
        ballRb = GetComponent<Rigidbody2D>();
        Launch();
    }


    //Initial launch of the ball
    private void Launch()
    {
        float xVelocity = Random.Range(0, 2) == 0 ? 1 : -1;
        float yVelocity = Random.Range(0, 2) == 0 ? 1 : -1;
        ballRb.velocity = new Vector2(xVelocity, yVelocity) * initialVelocity;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Paddle"))
        {
            ballRb.velocity *= velocityMultiplier;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Goal1"))
        {
            gamemanager.GetComponent<GameManager2>().Paddle2Scored();
            gamemanager.GetComponent<GameManager2>().Restart();

        }
        else
        {
            gamemanager.GetComponent<GameManager2>().Paddle1Scored();
            gamemanager.GetComponent<GameManager2>().Restart();

        }
    }


}
