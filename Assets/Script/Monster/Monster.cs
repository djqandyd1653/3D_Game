﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//interface IMonsterAction
//{
//    void MonsterAction();
//    void ChangeState(IMonsterAction nextAction);
//}

public class Monster : MonoBehaviour
{
    // DrawWireArc에 보호수준 어떻게 할지??
    public MonsterData monsterData;

    protected Vector3 originPos;                                // (임시) 시작위치 변수 생성, 나중에 변수선언 없이 만들어보자
    public Vector3 OriginPos { get { return originPos; } }      // 나중에 위치저장 변수에 값을 전달해줄 용도

    protected GameObject target;

    //public enum State
    //{
    //    Idle,
    //    Chase,
    //    Patrol,
    //    GoBack,
    //    Attack,
    //    Hit,
    //    Die
    //}

    //public enum Grade
    //{
    //    Common,
    //    Epic,
    //    Rare,
    //    Uniqe,
    //    Legendary
    //}

    //public float hp;
    //public float maxHp;

    //public float attackPower;
    //public float attackSpeed;

    //public float moveSpeed;
    //public float runSpeed;
    //public float rotateSpeed;

    //public float armor;

    //public GameObject target;
    //public float searchRange;
    //public float patrolRange;
    //public float attackRange;

    //public Vector3 originPos;

    //public State state;
    //public Grade grade;
}