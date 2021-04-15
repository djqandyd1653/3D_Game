using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : MonoBehaviour
{
    public Animator anim;
    [SerializeField]
    private ISkeletonAction skeletonAction;
    private Monster monster;
    private GameObject Commander;
    private Rigidbody rigid;

    private interface ISkeletonAction
    {
        void Action();
        ISkeletonAction ChangeState();
    }

    protected class StateComponent : MonoBehaviour
    {
        protected Skeleton skeleton;
        protected Monster monster;

        protected void Start()
        {
            skeleton = GetComponent<Skeleton>();
            monster = skeleton.monster;
        }
    }

    private class Idle : StateComponent, ISkeletonAction
    {
        private void OnEnable()
        {
            skeleton.anim.SetBool("IsIdle", true);
        }

        private void OnDisable()
        {
            skeleton.anim.SetBool("IsIdle", false);
        }

        public void Action()
        {
            if(Input.GetKey(KeyCode.Space))
            {
                monster.state = Monster.State.Patrol;
                skeleton.skeletonAction = ChangeState();
            }
        }

        public ISkeletonAction ChangeState()
        {
            GetComponent<Idle>().enabled = false;
            GetComponent<Patrol>().enabled = true;
            return GetComponent<Patrol>();
        }
    }

    private class Chase : StateComponent, ISkeletonAction
    {
        private void OnEnable()
        {
            skeleton.anim.SetBool("IsChase", true);
        }

        private void OnDisable()
        {
            skeleton.anim.SetBool("IsChase", false);
        }

        public void Action()
        {
            ChaseMove();
        }

        private void ChaseMove()
        {
            skeleton.Move(monster.runSpeed);
        }

        public ISkeletonAction ChangeState()
        {
            return GetComponent<Attack>();
        }
    }

    private class Patrol : StateComponent, ISkeletonAction
    {
        Vector3 destPos;

        private void OnEnable()
        {
            skeleton.anim.SetBool("IsPatrol", true);
            StartCoroutine(ChangeDestPos());
        }

        private void OnDisable()
        {
            skeleton.anim.SetBool("IsPatrol", false);
            StopCoroutine(ChangeDestPos());
        }

        public void Action()
        {
            PatrolMove();
            SearchTarget();
        }

        private void PatrolMove()
        {
            if (Mathf.Abs(CalDistance(destPos)) < 1)
                return;

            skeleton.Move(monster.moveSpeed);
        }

        private void SearchTarget()
        {
            Vector3 targetPos = monster.target.transform.position;
            Debug.DrawLine(destPos, destPos + Vector3.up * 5, Color.red);

            if (CalDistance(targetPos) < monster.searchRange)
            {
                ChangeState();
                Debug.Log("Trace로 변경");
            }
        }

        private float CalDistance(Vector3 targetPos)
        {
            if (Mathf.Abs(targetPos.x - transform.position.x) > 50 ||
                Mathf.Abs(targetPos.y - transform.position.y) > 50 ||
                Mathf.Abs(targetPos.z - transform.position.z) > 50)
            {
                return monster.searchRange + 1;
            }

            float distanceFromTarget = Mathf.Sqrt(Mathf.Pow(targetPos.x - transform.position.x, 2) +
                                                  Mathf.Pow(targetPos.y - transform.position.y, 2) +
                                                  Mathf.Pow(targetPos.z - transform.position.z, 2));

            return distanceFromTarget;
        }

        IEnumerator ChangeDestPos()
        {
            while(true)
            {
                int angle = Random.Range(0, 360);
                destPos = new Vector3(transform.position.x + Mathf.Cos(angle) * monster.patrolRange,
                                      transform.position.y,
                                      transform.position.z + Mathf.Sin(angle) * monster.patrolRange);
                //Debug.Log(destPos);
                yield return new WaitForSeconds(3.0f);
            }
        }

        public ISkeletonAction ChangeState()
        {
            return GetComponent<Chase>();
        }
    }

    private class Attack : StateComponent, ISkeletonAction
    {
        private void OnEnable()
        {
            skeleton.anim.SetBool("IsAttack", true);
        }

        private void OnDisable()
        {
            skeleton.anim.SetBool("IsAttack", false);
        }

        public void Action()
        {
            
        }

        public ISkeletonAction ChangeState()
        {
            return GetComponent<Idle>();
        }
    }

    private class Hit : StateComponent, ISkeletonAction
    {
        public void Action()
        {
            monster.hp--;
        }

        public ISkeletonAction ChangeState()
        {
            return GetComponent<Idle>();
        }
    }

    private class Die : StateComponent, ISkeletonAction
    {
        public void Action()
        {

        }

        public ISkeletonAction ChangeState()
        {
            return GetComponent<Idle>();
        }
    }

    void Start()
    {
        gameObject.AddComponent<Idle>();
        gameObject.AddComponent<Chase>();
        gameObject.AddComponent<Patrol>();
        gameObject.AddComponent<Attack>();
        gameObject.AddComponent<Hit>();
        gameObject.AddComponent<Die>();

        skeletonAction = GetComponent<Idle>();
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        monster = GetComponent<Monster>();

        monster.maxHp = 100f;
        monster.hp = monster.maxHp;

        monster.attackPower = 3f;
        monster.attackSpeed = 10f;

        monster.moveSpeed = 10f;
        monster.runSpeed = 20f;
        monster.rotateSpeed = 10f;

        monster.armor = 5f;

        monster.searchRange = 15.0f;
        monster.patrolRange = 10.0f;

        monster.state = Monster.State.Idle;
        monster.grade = Monster.Grade.Common;
    }

    void Update()
    {
        skeletonAction.Action();
    }

    private void Move(float speed)
    {
        Vector3 targetPos = monster.target.transform.position;
        Vector3 destDir = targetPos - transform.position;

        Quaternion dirToTarget = Quaternion.LookRotation(destDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, dirToTarget, 0.01f);

        rigid.MovePosition(transform.position + destDir.normalized * speed * Time.deltaTime);
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