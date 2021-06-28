using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamerMove : MonoBehaviour
{
    public GameObject player;
    void Start()
    {
        
    }

    void Update()
    {
        //var pos = transform.position;
        //pos.x = player.transform.position.x - 0.4f;
        //pos.y = player.transform.position.y + 22;
        //pos.z = player.transform.position.z - 50;
        //transform.position = pos;

        //transform.position = player.transform.right * (player.transform.position.x - 0.4f) +
        //                     player.transform.up * (player.transform.position.y + 22) +
        //                     player.transform.forward * (player.transform.position.z - 50);

        //transform.forward = player.transform.forward;

        //transform.RotateAround(player.transform.position, Vector3.up, player.transform.rotation.eulerAngles.y);
    }
}
