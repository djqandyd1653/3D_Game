using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum State
    {
        Idle,
        Move,
        Run,
        Attack01,
        Attack02,
        Hit,
        Die
    }

    [Header("Hp")]
    [SerializeField]
    protected float hp;
    public float Hp { get { return hp; } set { if (value >= 0) hp = value; } }

    [Header("State")]
    [SerializeField]
    protected State state;
    public State PlayerState { get { return state; } }

    [Header("Stamina")]
    [SerializeField]
    protected float stamina;                   // 현재 스태미나
    [SerializeField]
    protected float maxStamina;                // 최대 스태미나
    [SerializeField]
    protected float staminaFillAmount;         // 스태미나 1회 충전량
    [SerializeField]
    protected float staminaChargeCycle;        // 스태미나 충전 주기

    [Header("Attack")]
    [SerializeField]
    protected float attackPower;
    public float AttackPower { get { return attackPower; } set { attackPower = value; } }

    [Header("Move")]
    protected float vertical;                  // 전, 후 방향
    [SerializeField]
    protected float horizontal;                // 좌, 우 방향
    [SerializeField]
    protected float moveSpeed;
    [SerializeField]
    protected float runSpeed;
    [SerializeField]
    protected float rotateSpeed;

    protected Rigidbody rigid;

    [SerializeField]
    protected PlayerWeapon weapon;
    [SerializeField]
    protected HealthBar healthBar;
    public HealthBar HealthBar { get { return healthBar; } }
}
