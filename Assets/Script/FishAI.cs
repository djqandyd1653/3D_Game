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
    private Rigidbody rigid;

    public FishState state;
    public float stateChangeTime;
    public float moveSpeed;
    public float remainTime;
    public float rotateSpeed;
    public float rotatePower;
    public int num;

    void Start()
    {
        rigid = gameObject.GetComponent<Rigidbody>();
        state = FishState.Forward;
        remainTime = stateChangeTime;
    }

    // Update is called once per frame
    void Update()
    {
        rigid.Sleep();

        var pos = transform.position;
        pos.y = 0.1f;
        transform.position = pos;

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
        rotatePower = 1;

        anim.SetBool("Left", false);
        anim.SetBool("Right", false);
        anim.SetBool("Forward", false);

        num = Random.Range(0, 3);
        //Debug.Log(num);

        switch(num)
        {
            case 0:
                anim.SetBool("Left", true);
                state = FishState.Left;
                break;
            case 1:
                anim.SetBool("Right", true);
                state = FishState.Right;
                break;
            case 2:
                anim.SetBool("Forward", true);
                state = FishState.Forward;
                break;
        }
    }

    void Move()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

        if(state == FishState.Left)
        {
            transform.Rotate(new Vector3(0, -rotatePower * Time.deltaTime, 0));
        }

        if (state == FishState.Right)
        {
            transform.Rotate(new Vector3(0, rotatePower * Time.deltaTime, 0));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        MoveChange();
        rotatePower = 30;
        remainTime = stateChangeTime;
        //transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }
}
