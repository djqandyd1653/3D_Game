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

public class PlayerMove : StateComponent, PlayerAction
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
        moveVec = transform.forward * player.vertical + transform.right * player.horizontal;
        rigid.MovePosition(transform.position + moveVec.normalized * player.moveSpeed * Time.smoothDeltaTime);
    }
}

public class PlayerRun : StateComponent, PlayerAction
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
        moveVec = transform.forward * player.vertical + transform.right * player.horizontal;
        rigid.MovePosition(transform.position + moveVec.normalized * player.runSpeed * Time.smoothDeltaTime);
    }
}

public class PlayerAttack : StateComponent, PlayerAction
{
    public void Action()
    {
        
    }
}

public class PlayerHit : StateComponent, PlayerAction
{
    new void Start()
    {
        base.Start();
    }

    public void Action()
    {
        player.hp--;
    }
}

public class PlayerDie : StateComponent, PlayerAction
{
    public void Action()
    {
        
    }
}

public class PlayerController : StateComponent
{
    public PlayerAction playerAction;
    public Animator anim;

    private class Idle : StateComponent, PlayerAction
    {
        float remainTime;   // 스태미나 충전까지 남은 시간

        new void Start()
        {
            base.Start();
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

    new void Start()
    {
        base.Start();

        gameObject.AddComponent<Idle>();
        gameObject.AddComponent<PlayerMove>();
        gameObject.AddComponent<PlayerRun>();
        gameObject.AddComponent<PlayerAttack>();
        gameObject.AddComponent<PlayerHit>();

        playerAction = GetComponent<PlayerIdle>();
        player.state = Player.State.Idle;
        player.rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        playerAction.Action();
        InputKey();
        Rotate();
    }

    void ChangeComponent(PlayerAction newAction, Player.State state)
    {
        playerAction = newAction;
        ChangeState(state);
    }

    void ChangeState(Player.State state)
    {
        string strCurrState = "Is" + player.state.ToString();
        anim.SetBool(strCurrState, false);

        player.state = state;

        strCurrState = "Is" + state.ToString();
        anim.SetBool(strCurrState, true);
        anim.SetTrigger(strCurrState);
    }

    void InputKey()
    {
        player.vertical = Input.GetAxisRaw("Vertical");
        player.horizontal = Input.GetAxisRaw("Horizontal");

        if (player.state == Player.State.Idle || player.state == Player.State.Move)
            MoveInput();

        if (player.state == Player.State.Move || player.state == Player.State.Run)
            RunInput();

        if (player.state != Player.State.Hit || player.state != Player.State.Die)
            AttackInput();

        HitInput(); // OnCollisionEnter에 사용예정
    }

    void MoveRotation()
    {
        if (player.vertical != 0 && player.horizontal != 0)
        {
            float dir = player.vertical * player.horizontal;

            if(transform.GetChild(0).transform.rotation == transform.rotation)
                transform.GetChild(0).transform.rotation *= Quaternion.Euler(new Vector3(0, dir * 45, 0));
        }   
        else
        {
            if (transform.GetChild(0).transform.rotation != transform.rotation)
                transform.GetChild(0).transform.rotation = transform.rotation;
        }
    }

    void MoveInput()
    {
        if (player.vertical != 0 || player.horizontal != 0)
        {
            ChangeComponent(GetComponent<PlayerMove>(), Player.State.Move);

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
            ChangeComponent(GetComponent<PlayerIdle>(), Player.State.Idle);
    }

    void RunInput()
    {
        if (player.vertical > 0 && Input.GetKey(KeyCode.LeftShift))
            ChangeComponent(GetComponent<PlayerRun>(), Player.State.Run);

        if (Input.GetKeyUp(KeyCode.LeftShift) || player.vertical <= 0)
            ChangeComponent(GetComponent<PlayerMove>(), Player.State.Move);
    }

    void AttackInput()
    {
        if (Input.GetMouseButtonDown(0) && player.state != Player.State.Attack01 && player.state != Player.State.Attack02)
            ChangeComponent(GetComponent<PlayerAttack>(), Player.State.Attack01);

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Attack01") && player.state == Player.State.Attack01)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
                ChangeComponent(GetComponent<PlayerIdle>(), Player.State.Idle);

            else if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.4f &&
                Input.GetMouseButtonDown(0))
                ChangeState(Player.State.Attack02);
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Attack02") &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            ChangeComponent(GetComponent<PlayerIdle>(), Player.State.Idle);
    }

    void HitInput()
    {
        if(Input.GetKeyDown(KeyCode.P) && player.state != Player.State.Hit)
        {
            ChangeComponent(GetComponent<PlayerHit>(), Player.State.Hit);
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.GetHit") &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
        {
            ChangeComponent(GetComponent<PlayerIdle>(), Player.State.Idle);
        }
    }

    void Rotate()
    {
        MoveRotation();

        float mouseX = Input.GetAxis("Mouse X");
        transform.Rotate(Vector3.up * player.rotateSpeed * mouseX);
    }
}