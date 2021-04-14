using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public enum State
    {
        Idle,
        Chase,
        Patrol,
        Attack,
        Hit,
        Die
    }

    public enum Grade
    {
        Common,
        Epic,
        Rare,
        Uniqe,
        Legendary
    }

    public float hp;
    public float maxHp;

    public float attackPower;
    public float attackSpeed;

    public float moveSpeed;
    public float rotateSpeed;

    public float armor;

    public GameObject target;
    public float searchRange;

    public State state;
    public Grade grade;
}
