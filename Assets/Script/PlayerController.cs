using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum PlayerState
    {
        Idle,
        MoveForward,
        MoveSide,
        Attack,
        Hit,
        Die
    }

    Player player;
    Rigidbody rigid;

    public PlayerState state;
    public Animator anim;
    public float moveSpeed;

    void Start()
    {
        state = PlayerState.Idle;
        player = gameObject.GetComponent<Player>();
        rigid = gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        MoveForward();
        MoveSide();
        Attack();
    }

    void ChangeState(PlayerState nextState)
    {
        state = nextState;

        anim.SetBool("IsIdle", false);
        anim.SetBool("IsMoveForward", false);
        anim.SetBool("IsMoveSide", false);
        anim.SetBool("IsAttack", false);
        anim.SetBool("IsHit", false);
        anim.SetBool("IsDie", false);

        switch(state)
        {
            case PlayerState.Idle: anim.SetBool("IsIdle", true); break;
            case PlayerState.MoveForward: anim.SetBool("IsMoveForward", true); break;
            case PlayerState.MoveSide: anim.SetBool("IsMoveSide", true); break;
            case PlayerState.Attack: anim.SetBool("IsAttack", true); break;
            case PlayerState.Hit: anim.SetBool("IsHit", true); break;
            case PlayerState.Die: anim.SetBool("IsDie", true); break;
        }
    }

    void MoveForward()
    {
        ChangeState();

        moveSpeed = 0;

        
        float moveZ = Input.GetAxis("Vertical");
        anim.SetFloat("Move", moveZ);

        Vector3 dir = new Vector3(0, 0, moveZ).normalized;

        if (moveZ > 0)
        {
            moveSpeed = player.runSpeed;
        }
        else if (moveZ < 0)
        {
            moveSpeed = player.moveSpeed;
        }
           
        rigid.MovePosition(transform.position + dir * moveSpeed * Time.smoothDeltaTime);
    }

    void MoveSide()
    {
        float moveX = Input.GetAxis("Horizontal");

        if (moveX > 0)
        {
            moveSpeed = player.runSpeed;
        }
        else if (moveX < 0)
        {
            moveSpeed = player.moveSpeed;
        }

        Vector3 dir = new Vector3(moveX, 0, 0).normalized;

        rigid.MovePosition(transform.position + dir * moveSpeed * Time.smoothDeltaTime);
    }

    void Attack()
    {
        if (Input.GetMouseButton(0) && !anim.GetBool("IsAttack"))
        {
            ChangeState();
            anim.SetBool("IsAttack", true);
        }
    }

    void Idle()
    {
        if(state == PlayerState.Idle)
        {

        }
    }
}
