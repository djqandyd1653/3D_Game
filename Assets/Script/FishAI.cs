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
    public float rotatePower;
    public float angle;
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
            MoveChange(Random.Range(0, 3));
            remainTime = stateChangeTime;
        }
            
        Move();
    }

    void MoveChange(int num)
    {
        anim.SetBool("Left", false);
        anim.SetBool("Right", false);
        anim.SetBool("Forward", false);

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
        ContactPoint point = collision.contacts[0];

        angle = Vector3.Dot(-point.normal.normalized, transform.right);
        if (angle > 0)
            MoveChange(0);
        else if (angle < 0)
            MoveChange(1);
        else
            MoveChange(Random.Range(0, 2));

        int randNum = Random.Range(3, 6);
        StartCoroutine(CollisionCoroutine(randNum));
        
        remainTime = stateChangeTime;
        //transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }

    IEnumerator CollisionCoroutine(int randNum)
    {
        rotatePower = 60;
        moveSpeed = 3;
        yield return new WaitForSeconds(randNum);

        rotatePower = 30;
        moveSpeed = 1;
    }
}
