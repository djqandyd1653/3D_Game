using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface SkeletonAction
{
    void Action();
    //void ChangeState();
}

public class Idle : Enemy, SkeletonAction
{
    public void Action()
    {

    }
}

public class Skeleton : Enemy
{
    public Animator anim;

    //Vector3 targetPos;
    //float patrolSpeed = 2f;
    //float patrolRotation = 3f;

    //// Trace
    //float traceSpeed = 4;

    //// Attack
    //float attackRotation = 1.5f;

    //GameObject player;
    //Camera eye;

    //void RGB()
    //{
    //    patrolSpeed *= 3;
    //    patrolRotation *= 3;
    //    traceSpeed *= 3;
    //    attackRotation *= 3;
    //}

    void Start()
    {
        //eye = transform.GetComponentInChildren<Camera>();
        //player = GameObject.FindGameObjectWithTag("Player");
        maxHp = 100f;
        hp = maxHp;

        attackPower = 3f;
        attackSpeed = 10f;

        moveSpeed = 10f;
        rotateSpeed = 10f;

        armor = 5f;

        state = Enemy.State.Idle;
        grade = Enemy.Grade.Common;

        anim = GetComponent<Animator>();
    }

    void Update()
    {
        
    }

    void ChangeState()
    {

    }

    //void ChangeState(AI_State nextState)
    //{
    //    state = nextState;

    //    anim.SetBool("isIdle", false);
    //    anim.SetBool("isPatrol", false);
    //    anim.SetBool("isTrace", false);
    //    anim.SetBool("isAttack", false);

    //    StopAllCoroutines();

    //    switch (state)
    //    {
    //        case AI_State.Idle: StartCoroutine(Coroutine_Idle()); break;
    //        case AI_State.Patrol: StartCoroutine(Coroutine_Patrol()); break;
    //        case AI_State.Trace: StartCoroutine(Coroutine_Trace()); break;
    //        case AI_State.Attack: StartCoroutine(Coroutine_Attack()); break;
    //        case AI_State.Hide: StartCoroutine(Coroutine_Hide()); break;
    //    }
    //}

    //#region Update
    //void Update()
    //{
    //    switch (state)
    //    {
    //        case AI_State.Idle: Update_Idle(); break;
    //        case AI_State.Patrol: Update_Patrol(); break;
    //        case AI_State.Trace: Update_Trace(); break;
    //        case AI_State.Attack: Update_Attack(); break;
    //        case AI_State.Hide: Update_Hide(); break;
    //    }
    //}

    //void Update_Idle()
    //{
    //    // 매 프레임해야 되는 실행문
    //}

    //void Update_Patrol()
    //{
    //    Vector3 posOffset = targetPos - transform.position;
    //    float dist = posOffset.magnitude;

    //    // 타겟지점에 일정 거리 이상
    //    if (dist <= 0.3f)
    //    {
    //        ChangeState(AI_State.Idle);
    //        return;
    //    }

    //    // 회전
    //    var targetRotation = Quaternion.LookRotation(posOffset, Vector3.up);
    //    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, patrolRotation * Time.deltaTime);

    //    // 이동
    //    transform.position += transform.forward * patrolSpeed * Time.deltaTime;
    //}

    //void Update_Trace()
    //{
    //    Vector3 posOffset = player.transform.position - transform.position;
    //    float dist = posOffset.magnitude;

    //    // 타겟지점에 일정 거리 이상
    //    if (dist <= 1.5f)
    //    {
    //        ChangeState(AI_State.Attack);
    //        return;
    //    }
    //    else if (dist >= 12f)
    //    {
    //        ChangeState(AI_State.Idle);
    //        return;
    //    }

    //    // 회전
    //    var targetRotation = Quaternion.LookRotation(posOffset, Vector3.up);
    //    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, patrolRotation * Time.deltaTime);

    //    // 이동
    //    transform.position += transform.forward * traceSpeed * Time.deltaTime;
    //}

    //void Update_Attack()
    //{
    //    Vector3 posOffset = player.transform.position - transform.position;

    //    if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Attack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f)
    //    {
    //        ChangeState(AI_State.Trace);
    //        return;
    //    }

    //    // 회전
    //    var targetRotation = Quaternion.LookRotation(posOffset, Vector3.up);
    //    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, attackRotation * Time.deltaTime);
    //}

    //void Update_Hide()
    //{

    //}
    //#endregion

    //#region Coroutine
    //IEnumerator Coroutine_Idle()
    //{
    //    // 1. 상태가 바뀌고 최초에 한번만 하는 실행문
    //    anim.SetBool("isIdle", true);

    //    nextState = AI_State.Patrol;

    //    // 2. 일정 시간(조건)마다 동작하는 실행문
    //    while (true)
    //    {
    //        yield return new WaitForSeconds(2.0f);

    //        ChangeState(nextState);
    //    }
    //}

    //IEnumerator Coroutine_Patrol()
    //{
    //    // 1. 상태가 바뀌고 최초에 한번만 하는 실행문
    //    anim.SetBool("isPatrol", true);

    //    nextState = AI_State.Idle;

    //    // 목표지점 설정
    //    targetPos = transform.position + new Vector3(Random.Range(-3f, 3f), 0f, Random.Range(-3f, 3f));

    //    // 2. 일정 시간(조건)마다 동작하는 실행문
    //    while (true)
    //    {
    //        yield return new WaitForSeconds(5.0f);

    //        ChangeState(nextState);
    //    }
    //}

    //IEnumerator Coroutine_Trace()
    //{
    //    // 1. 상태가 바뀌고 최초에 한번만 하는 실행문
    //    anim.SetBool("isTrace", true);

    //    while (true)
    //    {
    //        yield return new WaitForSeconds(5.0f);
    //    }
    //}
    //IEnumerator Coroutine_Attack()
    //{
    //    // 1. 상태가 바뀌고 최초에 한번만 하는 실행문
    //    anim.SetBool("isAttack", true);

    //    // 2. 일정 시간(조건)마다 동작하는 실행문
    //    while (true)
    //    {
    //        yield return new WaitForSeconds(10.1f);

    //    }
    //}
    //IEnumerator Coroutine_Hide()
    //{
    //    return null;
    //}
    //#endregion
}
