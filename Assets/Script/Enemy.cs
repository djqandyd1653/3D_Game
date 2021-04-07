using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum State
    {
        Idle,
        Trace,
        Patrol,
        Attack,
        Hit
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

    public State state;
    public Grade grade;
}


