using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class PlayerManager : MonoBehaviourPunCallbacks
{

    public static GameObject LocalPlayerInstance;
    public GameObject prefab; 
    private GameObject paddle;

    private const float bound = 3.75f;
    private const float speed = 7.0f;
    private float side = 7.5f;

    private void Start()
    {

        if (PhotonNetwork.IsMasterClient)
            side *= -1;

        //normal instance dont delete paddle(?)
        paddle = PhotonNetwork.Instantiate(prefab.name, new Vector3(side, 0.0f, 0.0f), Quaternion.identity);
    }

    private void Update()
    {
        GetInput();
    }

    [PunRPC]
    public void GetInput()
    {
        float movement = Input.GetAxisRaw("Vertical");
        paddle.transform.position = new Vector3(paddle.transform.position.x, Mathf.Clamp(paddle.transform.position.y + movement * speed * Time.deltaTime, -bound, bound), 0.0f);
    }

    //Creates paddle just once as gO
    public void ResetPaddle()
    {
        paddle.transform.position = new Vector3(side, 0.0f, 0.0f);
    }
}