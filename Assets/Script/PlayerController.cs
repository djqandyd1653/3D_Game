using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface PlayerAction
{
    void Action();
}

public class Idle : MonoBehaviour, PlayerAction
{
    Player player;
    float remainTime;   // 스태미나 충전까지 남은 시간

    void Start()
    {
        player = GetComponent<Player>();
        remainTime = 0.0f;
    }

    public void Action()
    {
        ChargeStamina();
    }

    void ChargeStamina()
    {
        if (player.stamina >= player.maxStamina)
            return;

        remainTime += Time.deltaTime;

        if (remainTime >= player.staminaChargeCycle)
        {
            remainTime = 0.0f;
            player.stamina = Mathf.Clamp(player.stamina + player.staminaFillAmount, 0, player.maxStamina);
        }
    }
}

public class Move : MonoBehaviour, PlayerAction
{
    Player player;

    void Start()
    {
        player = GetComponent<Player>();
    }

    public void Action()
    {
        Rigidbody rigid = player.rigid;
        Vector3 moveVec = new Vector3(player.horizontal, 0, player.vertical);
        rigid.MovePosition(transform.position + moveVec * player.moveSpeed * Time.smoothDeltaTime);
    }
}

public class Attack : MonoBehaviour, PlayerAction
{
    public void Action()
    {
        
    }
}

public class PlayerController : MonoBehaviour
{
    Player player;
    public PlayerAction playerAction;
    public Animator anim;

    void Start()
    {
        gameObject.AddComponent<Idle>();
        gameObject.AddComponent<Move>();    
        gameObject.AddComponent<Attack>();

        playerAction = GetComponent<Idle>();
        player = GetComponent<Player>();
        player.state = Player.State.Idle;
        player.rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //MoveForward();
        //MoveSide();
        //Attack();
        playerAction.Action();
        InputKey();
    }

    void ChangeState(PlayerAction newAction)
    {
        playerAction = newAction;
    }

    void InputKey()
    {
        if ((player.vertical = Input.GetAxis("Vertical")) != 0 ||
            (player.horizontal = Input.GetAxis("Horizontal")) != 0)
            ChangeState(GetComponent<Move>());  

        if (player.vertical == 0 && player.horizontal == 0)
            ChangeState(GetComponent<Idle>());
           
        if (Input.GetMouseButton(0))
        {
            ChangeState(GetComponent<Attack>());
            anim.SetBool("IsAttack", true);
        }
    }

    //void ChangeState(PlayerState nextState)
    //{
    //    state = nextState;

    //    anim.SetBool("IsIdle", false);
    //    anim.SetBool("IsMoveForward", false);
    //    anim.SetBool("IsMoveSide", false);
    //    anim.SetBool("IsAttack", false);
    //    anim.SetBool("IsHit", false);
    //    anim.SetBool("IsDie", false);

    //    switch(state)
    //    {
    //        case PlayerState.Idle: anim.SetBool("IsIdle", true); break;
    //        case PlayerState.MoveForward: anim.SetBool("IsMoveForward", true); break;
    //        case PlayerState.MoveSide: anim.SetBool("IsMoveSide", true); break;
    //        case PlayerState.Attack: anim.SetBool("IsAttack", true); break;
    //        case PlayerState.Hit: anim.SetBool("IsHit", true); break;
    //        case PlayerState.Die: anim.SetBool("IsDie", true); break;
    //    }
    //}

    //void MoveForward()
    //{
    //    //ChangeState();

    //    moveSpeed = 0;

        
    //    float moveZ = Input.GetAxis("Vertical");
    //    anim.SetFloat("Move", moveZ);

    //    Vector3 dir = new Vector3(0, 0, moveZ).normalized;

    //    if (moveZ > 0)
    //    {
    //        moveSpeed = player.runSpeed;
    //    }
    //    else if (moveZ < 0)
    //    {
    //        moveSpeed = player.moveSpeed;
    //    }
           
    //    rigid.MovePosition(transform.position + dir * moveSpeed * Time.smoothDeltaTime);
    //}

    //void MoveSide()
    //{
    //    float moveX = Input.GetAxis("Horizontal");

    //    if (moveX > 0)
    //    {
    //        moveSpeed = player.runSpeed;
    //    }
    //    else if (moveX < 0)
    //    {
    //        moveSpeed = player.moveSpeed;
    //    }

    //    Vector3 dir = new Vector3(moveX, 0, 0).normalized;

    //    rigid.MovePosition(transform.position + dir * moveSpeed * Time.smoothDeltaTime);
    //}

    //void Attack()
    //{
    //    if (Input.GetMouseButton(0) && !anim.GetBool("IsAttack"))
    //    {
    //        //ChangeState();
    //        anim.SetBool("IsAttack", true);
    //    }
    //}

    //void Idle()
    //{
    //    if(state == PlayerState.Idle)
    //    {

    //    }
    //}
}
