using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Player, IDamageable
{
    public PlayerAction playerAction;
    public Animator anim;
    public HealthBar healthBar;

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
            // 움직임이 있으면 Move로 상태변환
            if (player.vertical != 0 || player.horizontal != 0)
            {
                player.ChangeComponent(GetComponent<Move>(), State.Move, 0.25f);
                return;
            }

            // 마우스 왼쪽버튼 누르면 Attack으로 상태변환
            if(Input.GetMouseButtonDown(0))
            {
                player.ChangeComponent(GetComponent<Attack>(), State.Attack01);
            }

            // 스테미나 충전
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
        public void Action()
        {
            // 마우스 왼쪽버튼 누르면 Attack으로 상태변환
            if (Input.GetMouseButtonDown(0))
            {
                player.ChangeComponent(GetComponent<Attack>(), State.Attack01);
            }

            // 왼쪽 쉬프트키가 눌려있으면 Run으로 상태변환
            if (Input.GetKey(KeyCode.LeftShift) && player.vertical > 0)
            {
                player.ChangeComponent(GetComponent<Run>(), State.Run, 0.25f);
            }

            // 이동속도가 0이면 Idle로 상태변환
            if (player.vertical == 0 && player.horizontal == 0)
            {
                player.ChangeComponent(GetComponent<Idle>(), State.Idle, 0.25f);
            }

            player.PlayerMove(player.moveSpeed);
        }
    }

    private class Run : StateComponent, PlayerAction
    {
        public void Action()
        {
            // 마우스 왼쪽버튼 누르면 Attack으로 상태변환
            if (Input.GetMouseButtonDown(0))
            {
                player.ChangeComponent(GetComponent<Attack>(), State.Attack01);
            }

            // 왼쪽 쉬프트키가 눌려있다가 떨어지면 Idle로 상태변환
            if (!Input.GetKey(KeyCode.LeftShift) || player.vertical <= 0)
            {
                player.ChangeComponent(GetComponent<Idle>(), State.Idle, 0.25f);
            }

            player.PlayerMove(player.runSpeed);
        }
    }

    private class Attack : StateComponent, PlayerAction
    {
        public void Action()
        {
            var currAnim = player.anim.GetCurrentAnimatorStateInfo(0);
            player.weapon.AttackAnimTime = currAnim.normalizedTime;

            if (player.state == State.Attack01)
            {
                // 0.5f ~ 0.7f에 2차공격 시도했으면 2차공격으로 상태변환
                if (currAnim.normalizedTime >= 0.5f && currAnim.normalizedTime <= 0.7f && 
                    currAnim.IsName("Attack01") && Input.GetMouseButtonDown(0))
                {
                    //player.weapon.SetColliderActive(true);
                    player.ChangeState(State.Attack02, 0.25f);
                }
            }

            if ((currAnim.IsName("Attack01") || currAnim.IsName("Attack02")) && currAnim.normalizedTime >= 1f)
            {
                //player.weapon.SetColliderActive(true);
                player.ChangeComponent(GetComponent<Idle>(), State.Idle);
            }
        }
    }

    private class Hit : StateComponent, PlayerAction
    {
        public void Action()
        {
            var currAnim = player.anim.GetCurrentAnimatorStateInfo(0);
            if (currAnim.IsName("Hit") && currAnim.normalizedTime >= 1f)
            {
                player.ChangeComponent(GetComponent<Idle>(), State.Idle);
            }
        }
    }

    private class Die : StateComponent, PlayerAction
    {
        public void Action()
        {
            if (player.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f &&
                player.anim.GetCurrentAnimatorStateInfo(0).IsName("Die"))
            {
                player.ChangeComponent(GetComponent<Idle>(), State.Idle);
                Debug.Log("게임 종료");
            }
        }
    }

    private void Start()
    {
        gameObject.AddComponent<Idle>();
        gameObject.AddComponent<Move>();
        gameObject.AddComponent<Run>();
        gameObject.AddComponent<Attack>();
        gameObject.AddComponent<Hit>();
        gameObject.AddComponent<Die>();

        playerAction = GetComponent<Idle>();
        state = State.Idle;
        rigid = GetComponent<Rigidbody>();

        healthBar.SetMaxHpBar(hp);

    }

    private void Update()
    {
        playerAction.Action();
        InputKey();
        Rotate();
    }

    private void ChangeComponent(PlayerAction newAction, State state, float transitionDuraingTime = 0)
    {
        ChangeState(state, transitionDuraingTime);
        playerAction = newAction;
    }

    private void ChangeState(State _state, float transitionDuraingTime = 0)
    {
        state = _state; // 매개변수를 nextStae로

        string strNextState = null;

        if (state == State.Move)
        {
            strNextState = (vertical == 0) ? "Walk Side" : "Walk Forward";
        }
        else
        {
            strNextState = state.ToString();
        }

        weapon.StrState = strNextState;

        if(transitionDuraingTime == 0f)
        {
            anim.Play(strNextState);
            return;
        }

        anim.CrossFade(strNextState, transitionDuraingTime);
    }

    // 플레이어 움직임
    private void PlayerMove(float speed)
    {
        Vector3 moveVec = transform.forward * vertical + transform.right * horizontal;
        rigid.MovePosition(transform.position + moveVec.normalized * speed * Time.smoothDeltaTime);
    }

    void InputKey()
    {
        vertical = Input.GetAxisRaw("Vertical");
        horizontal = Input.GetAxisRaw("Horizontal");
        anim.SetFloat("Vertical", vertical);
        anim.SetFloat("Horizontal", horizontal);
    }

    void MoveRotation()
    {
        // 플레이어가 대각선 이동 중일때
        if (vertical != 0 && horizontal != 0)
        {
            float dir = vertical * horizontal;

            // 플레이어 회전방향과 플레이어 바디 회전방향이 같으면 45도 회전
            if(transform.GetChild(0).transform.rotation == transform.rotation)
            {
                transform.GetChild(0).transform.rotation *= Quaternion.Euler(new Vector3(0, dir * 45, 0));
            }
        }   
        else
        {
            if (transform.GetChild(0).transform.rotation != transform.rotation)
            {
                transform.GetChild(0).transform.rotation = transform.rotation;
            }
        }
    }

    // 회전
    private void Rotate()
    {
        MoveRotation();

        float mouseX = Input.GetAxis("Mouse X");
        transform.Rotate(Vector3.up * rotateSpeed * mouseX);
    }

    // 현재 재생중인 애니메이션의 정규화된 시간을 반환
    public float CurrAnimationNormalizedTime()
    {
        return anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    public void ToDamage(float damage)
    {
        hp -= damage;
        healthBar.SetHpBar(hp);

        if (hp <= 0)
        {
            ChangeComponent(GetComponent<Die>(), State.Die);
            return;
        }

        if(state == State.Hit)
        {
            anim.Play("Hit", -1, 0);
        }

        ChangeComponent(GetComponent<Hit>(), State.Hit);
    }

    //private void TakeDamage(float damage)
    //{
    //    hp -= damage;
    //    healthBar.SetHpBar(hp);

    //    if (hp <= 0)
    //    {
    //        ChangeComponent(GetComponent<Die>(), State.Die);
    //        return;
    //    }

    //    ChangeComponent(GetComponent<Hit>(), State.Hit);
    //}

    // 적 무기와 충돌
    //private void OnTriggerEnter(Collider other)
    //{
    //    if(other.CompareTag("Monster Weapon"))
    //    {
    //        var monster = other.transform.parent.transform.parent;
    //        float animationTime = monster.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime;

    //        if (animationTime < 0.33f || animationTime > 0.37f)
    //        {
    //            return;
    //        }

    //        float damage = monster.GetComponent<Monster>().monsterData.AttackPower;

    //        TakeDamage(damage);
    //    }
    //}
}