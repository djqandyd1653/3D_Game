using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface PlayerAction
{
    void Action();
}

public class StateComponent : MonoBehaviour
{
    protected Player player;

    protected void Start()
    {
        player = GetComponent<Player>();
    }
}

public class Idle : StateComponent, PlayerAction
{
    //Player player;
    float remainTime;   // 스태미나 충전까지 남은 시간

    new void Start()
    {
        base.Start();

        //player = GetComponent<Player>();
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

public class Move : StateComponent, PlayerAction
{
    Rigidbody rigid;
    Vector3 moveVec;

    new void Start()
    {
        base.Start();
        rigid = player.rigid;
    }

    public void Action()
    {
        moveVec = new Vector3(player.horizontal, 0, player.vertical);
        rigid.MovePosition(transform.position + moveVec * player.moveSpeed * Time.smoothDeltaTime);
    }
}

public class Run : StateComponent, PlayerAction
{
    Rigidbody rigid;
    Vector3 moveVec;

    new void Start()
    {
        base.Start();
        rigid = player.rigid;
    }

    public void Action()
    {
        moveVec = new Vector3(player.horizontal, 0, player.vertical);
        rigid.MovePosition(transform.position + moveVec * player.runSpeed * Time.smoothDeltaTime);
    }
}

public class Attack : MonoBehaviour, PlayerAction
{
    public void Action()
    {
        
    }
}

public class PlayerController : StateComponent
{
    public PlayerAction playerAction;
    public Animator anim;

    new void Start()
    {
        base.Start();

        gameObject.AddComponent<Idle>();
        gameObject.AddComponent<Move>();
        gameObject.AddComponent<Run>();
        gameObject.AddComponent<Attack>();

        playerAction = GetComponent<Idle>();
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

    void ChangeComponent(PlayerAction newAction, Player.State state)
    {
        playerAction = newAction;
        ChangeState(state);
    }

    void ChangeState(Player.State state)
    {
        foreach(Player.State arrState in (Player.State[])System.Enum.GetValues(typeof(Player.State)))
        {
            string strTempState = "Is" + arrState.ToString();
            anim.SetBool(strTempState, false);
        }

        player.state = state;

        string strCurrState = "Is" + state.ToString();
        anim.SetBool(strCurrState, true);
    }

    void InputKey()
    {
        player.vertical = Input.GetAxis("Vertical");
        player.horizontal = Input.GetAxis("Horizontal");

        if (player.vertical != 0 || player.horizontal != 0)
        {
            ChangeComponent(GetComponent<Move>(), Player.State.Move);
            //ChangeComponent(GetComponent<Move>());
            //ChangeState(Player.State.Move);

            anim.SetFloat("Vertical", player.vertical);
            anim.SetFloat("Horizontal", player.horizontal);

            if(player.vertical != 0)
            {
                anim.SetBool("ISMoveForward", true);
                return;
            }

            anim.SetBool("ISMoveForward", false);
        }

        if (!Input.GetButton("Horizontal"))
        {
            player.horizontal = 0;
        }

        if (!Input.GetButton("Vertical"))
        {
            player.vertical = 0;
        }

        if (player.vertical == 0 && player.horizontal == 0)
        {
            ChangeComponent(GetComponent<Move>(), Player.State.Idle);
            //ChangeComponent(GetComponent<Idle>());
            //ChangeState(Player.State.Idle);
        }

        if (player.vertical > 0 && Input.GetKey(KeyCode.LeftShift))
        {
            ChangeComponent(GetComponent<Move>(), Player.State.Run);
            //ChangeComponent(GetComponent<Run>());
            //ChangeState(Player.State.Run);
        }
           
        if (Input.GetMouseButton(0))
        {
            ChangeComponent(GetComponent<Move>(), Player.State.Attack);
            //ChangeComponent(GetComponent<Attack>());
            //ChangeState(Player.State.Attack);
        }
    }

    void MoveAnimCtrl()
    {
        //if(player.vertical)
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
