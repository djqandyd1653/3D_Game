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
    public float hp;

    [Header("State")]
    public State state;
    public State PlayerState { get { return state; } }

    [Header("Stamina")]
    public float stamina;                   // 현재 스태미나
    public float maxStamina;                // 최대 스태미나
    public float staminaFillAmount;         // 스태미나 1회 충전량
    public float staminaChargeCycle;        // 스태미나 충전 주기

    [Header("Attack")]
    public float attackPower;
    public float AttackPower { get { return attackPower; } }

    [Header("Move")]
    public float vertical;                  // 전, 후 방향
    public float horizontal;                // 좌, 우 방향
    public float moveSpeed;
    public float runSpeed;
    public float rotateSpeed;

    public Rigidbody rigid;
}
