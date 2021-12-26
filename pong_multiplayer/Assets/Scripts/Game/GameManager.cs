using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TMP_Text paddle1ScoreText;
    [SerializeField] private TMP_Text paddle2ScoreText;

    [SerializeField] private Transform paddle1transform;
    [SerializeField] private Transform paddle2transform;
    [SerializeField] private Transform balltransform;

    private int paddle1Score;
    private int paddle2Score;

    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }
            return instance;

        }

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
        paddle1transform.position = new Vector2(paddle1transform.position.x, 0);
        paddle2transform.position = new Vector2(paddle2transform.position.x, 0);
        balltransform.position = new Vector2(0, 0);
    }
}
