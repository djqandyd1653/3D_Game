using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private enum PlayerState
    {
        Idle,
        Move,
        Attack,
        Hit
    }

    Player player;
    Rigidbody rigid;
    public Animator anim;
    public float speed;

    void Start()
    {
        player = gameObject.GetComponent<Player>();
        rigid = gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        Move();
        Attack();
    }

    void ChangeState()
    {
        //anim.SetBool("IsIdle", false);
        anim.SetFloat("Move", 0);
        anim.SetBool("IsAttack", false);
        //anim.SetBool("IsHit", false);
    }

    void Move()
    {
        ChangeState();

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        anim.SetFloat("Move", moveZ);

        Vector3 dir = new Vector3(moveX, 0, moveZ).normalized;

        if (moveZ > 0)
        {
            speed = player.runSpeed;
        }
        else if (moveZ < 0)
        {
            speed = player.moveSpeed;
        }
        else
        {
            speed = 0;
            //Idle();
        }
           
        rigid.MovePosition(transform.position + dir * speed * Time.smoothDeltaTime);
    }

    void Attack()
    {
        if (Input.GetMouseButton(0) && !anim.GetBool("IsAttack"))
        {
            ChangeState();
            anim.SetBool("IsAttack", true);
        }
    }

    //void Idle()
    //{
    //    ChangeState();
    //    anim.SetBool("IsIdle", true);
    //}
}
