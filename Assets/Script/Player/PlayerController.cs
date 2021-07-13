using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Player
{
    public PlayerAction playerAction;
    public Animator anim;

    public interface PlayerAction
    {
        void Action();
    }

    protected class StateComponent : MonoBehaviour
    {
        protected PlayerController player;

        protected void Start()
        {
            player = GetComponent<PlayerController>();
        }
    }

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

    private class Move : StateComponent, PlayerAction
    {
        Vector3 moveVec;

        new void Start()
        {
            base.Start();
        }

        public void Action()
        {
            moveVec = transform.forward * player.vertical + transform.right * player.horizontal;
            player.rigid.MovePosition(transform.position + moveVec.normalized * player.moveSpeed * Time.smoothDeltaTime);
        }
    }

    private class Run : StateComponent, PlayerAction
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

    private class Attack : StateComponent, PlayerAction
    {
        public void Action()
        {
            if (player.anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Attack01") && player.state == Player.State.Attack01)
            {
                if (player.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
                    player.ChangeComponent(GetComponent<Idle>(), Player.State.Idle);

                else if (player.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.4f &&
                    Input.GetMouseButtonDown(0))
                    player.ChangeState(Player.State.Attack02);
            }

            if (player.anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Attack02") &&
                player.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f &&
                player.state == Player.State.Attack02)
                player.ChangeComponent(GetComponent<Idle>(), Player.State.Idle);
        }
    }

    private class Hit : StateComponent, PlayerAction
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

    private class Die : StateComponent, PlayerAction
    {
        public void Action()
        {

        }
    }

    void Start()
    {
        gameObject.AddComponent<Idle>();
        gameObject.AddComponent<Move>();
        gameObject.AddComponent<Run>();
        gameObject.AddComponent<Attack>();
        gameObject.AddComponent<Hit>();

        playerAction = GetComponent<Idle>();
        state = Player.State.Idle;
        rigid = GetComponent<Rigidbody>();
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

    void ChangeState(Player.State _state)
    {
        string strCurrState = "Is" + state.ToString();
        anim.SetBool(strCurrState, false);

        state = _state;

        strCurrState = "Is" + state.ToString();
        anim.SetBool(strCurrState, true);
        anim.SetTrigger(strCurrState);
    }

    void InputKey()
    {
        vertical = Input.GetAxisRaw("Vertical");
        horizontal = Input.GetAxisRaw("Horizontal");

        if ((state == Player.State.Idle || state == Player.State.Move) &&
            (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Idle_Battle") || 
            anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.WalkForward") || 
            anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.WalkSide")))
            MoveInput();

        if (state == Player.State.Move || state == Player.State.Run)
            RunInput();

        if (state != Player.State.Hit || state != Player.State.Die)
            AttackInput();

        HitInput(); // OnCollisionEnter에 사용예정
    }

    void MoveRotation()
    {
        if (vertical != 0 && horizontal != 0)
        {
            float dir = vertical * horizontal;

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
        if (vertical != 0 || horizontal != 0)
        {
            ChangeComponent(GetComponent<Move>(), Player.State.Move);

            anim.SetFloat("Vertical", vertical);
            anim.SetFloat("Horizontal", horizontal);

            if (vertical != 0)
            {
                anim.SetBool("ISMoveForward", true);
                return;
            }

            anim.SetBool("ISMoveForward", false);
        }

        if (vertical == 0 && horizontal == 0)
            ChangeComponent(GetComponent<Idle>(), Player.State.Idle);
    }

    void RunInput()
    {
        if (vertical > 0 && Input.GetKey(KeyCode.LeftShift))
            ChangeComponent(GetComponent<Run>(), Player.State.Run);

        if (Input.GetKeyUp(KeyCode.LeftShift) || vertical <= 0)
            ChangeComponent(GetComponent<Move>(), Player.State.Move);
    }

    void AttackInput()
    {
        if (Input.GetMouseButtonDown(0) && state != Player.State.Attack01 && state != Player.State.Attack02)
            ChangeComponent(GetComponent<Attack>(), Player.State.Attack01);
    }

    void HitInput()
    {
        if(Input.GetKeyDown(KeyCode.P) && state != Player.State.Hit)
        {
            ChangeComponent(GetComponent<Hit>(), Player.State.Hit);
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.GetHit") &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
        {
            ChangeComponent(GetComponent<Idle>(), Player.State.Idle);
        }
    }

    void Rotate()
    {
        MoveRotation();

        float mouseX = Input.GetAxis("Mouse X");
        transform.Rotate(Vector3.up * rotateSpeed * mouseX);
    }

    public float CurrAnimationNormalizedTime()
    {
        return anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Monster Weapon"))
        {
            var monster = other.transform.parent.transform.parent;
            float animationTime = monster.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime;

            if (animationTime < 0.3f || animationTime > 0.4f)
            {
                return;
            }

            hp -= monster.GetComponent<Monster>().monsterData.AttackPower;
        }
    }
}