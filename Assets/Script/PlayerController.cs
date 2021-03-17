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
        //moveVec = transform.TransformDirection(moveVec);
        rigid.MovePosition(transform.position + moveVec.normalized * player.moveSpeed * Time.smoothDeltaTime);
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
        //moveVec = transform.TransformDirection(moveVec);
        rigid.MovePosition(transform.position + moveVec.normalized * player.runSpeed * Time.smoothDeltaTime);
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

    Quaternion originRotation;

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

        originRotation = transform.rotation;
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
        player.vertical = Input.GetAxisRaw("Vertical");
        player.horizontal = Input.GetAxisRaw("Horizontal");

        if (player.state == Player.State.Idle || player.state == Player.State.Move)
            MoveInput();

        if (player.state == Player.State.Move || player.state == Player.State.Run)
            RunInput();
           
        //if (Input.GetMouseButton(0))
        //{
        //    float a = player.vertical;
        //    ChangeComponent(GetComponent<Attack>(), Player.State.Attack);
        //}
    }

    void MoveInput()
    {
        if (player.vertical != 0 && player.horizontal != 0)
        {
            float dir = player.vertical * player.horizontal;
            
            if (originRotation == transform.rotation)
                transform.rotation = originRotation * Quaternion.Euler(new Vector3(0, dir * 45, 0)); ;
        }
        else
        {
            if (originRotation != transform.rotation)
                transform.rotation = originRotation;
        }


        if (player.vertical != 0 || player.horizontal != 0)
        {
            ChangeComponent(GetComponent<Move>(), Player.State.Move);

            anim.SetFloat("Vertical", player.vertical);
            anim.SetFloat("Horizontal", player.horizontal);

            if (player.vertical != 0)
            {
                anim.SetBool("ISMoveForward", true);
                return;
            }

            anim.SetBool("ISMoveForward", false);
        }

        if (player.vertical == 0 && player.horizontal == 0)
            ChangeComponent(GetComponent<Idle>(), Player.State.Idle);
    }

    void RunInput()
    {
        if (player.vertical > 0 && Input.GetKey(KeyCode.LeftShift))
            ChangeComponent(GetComponent<Run>(), Player.State.Run);

        if (Input.GetKeyUp(KeyCode.LeftShift))
            ChangeComponent(GetComponent<Move>(), Player.State.Move);
    }
}