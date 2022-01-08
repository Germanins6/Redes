using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private float initialVelocity = 4f;
    [SerializeField] private float velocityMultiplier = 1.1f;

    private Rigidbody2D ballRb;
    
    void Start()
    {
        ballRb = GetComponent<Rigidbody2D>();
        Launch();
    }


    //Initial launch of the ball
    private void Launch() {
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
            //NetworkingServer.gameManager.Paddle2Scored();
            //GameManager.Instance.Restart();
            Launch();
        }
        else 
        {
            //GameManager.Instance.Paddle1Scored();
            //GameManager.Instance.Restart();
            Launch();
        }
    }
    
   
}
