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
            MoveChange((FishState)Random.Range(0, 3));
            remainTime = stateChangeTime;
        }
            
        Move();
    }

    void MoveChange(FishState nextState)
    {
        anim.SetBool("Left", false);
        anim.SetBool("Right", false);
        anim.SetBool("Forward", false);

        state = nextState;

        switch(nextState)
        {
            case FishState.Left:
                anim.SetBool("Left", true);
                break;
            case FishState.Right:
                anim.SetBool("Right", true);
                break;
            case FishState.Forward:
                anim.SetBool("Forward", true);
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

        angle = Vector3.Dot(point.normal, transform.right);
        if (angle > 0)
            MoveChange(FishState.Right);
        else if (angle <= 0)
            MoveChange(FishState.Left);

        int randNum = Random.Range(3, 6);
        StartCoroutine(CollisionCoroutine(randNum));
        
        remainTime = stateChangeTime;
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
