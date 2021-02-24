using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FishState
{
    Forward,
    Right,
    Left
}

public class FishAI : MonoBehaviour
{
    public Animator anim;

    FishState state = FishState.Forward;
    public float stateChangeTime;
    public float remainTime;
    public float rotationSpeed;

    void Start()
    {
        remainTime = stateChangeTime;
    }

    // Update is called once per frame
    void Update()
    {
        remainTime -= Time.deltaTime;
        if (remainTime < 0)
        {
            MoveChange();
            remainTime = stateChangeTime;
        }
            

        Move();
    }

    void MoveChange()
    {
        anim.SetBool("Left", false);
        anim.SetBool("Right", false);
        anim.SetBool("Forward", false);

        int num = Random.Range(0, 3);
        //Debug.Log(num);

        switch(num)
        {
            case 0: anim.SetBool("Left", true); break;
            case 1: anim.SetBool("Right", true); break;
            case 2: anim.SetBool("Forward", true); break;
        }
    }

    void Move()
    {
        transform.Translate(Vector3.forward * Time.deltaTime);

        if(state == FishState.Left)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(0, -30, 0)), rotationSpeed * Time.deltaTime);
        }

        if (state == FishState.Right)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(0, 30, 0)), rotationSpeed * Time.deltaTime);
        }
    }

}
