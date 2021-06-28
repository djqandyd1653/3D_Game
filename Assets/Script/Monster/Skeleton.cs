using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : Monster
{
    private Animator anim;
    private IMonsterAction skeletonAction;
    private Rigidbody rigid;

    private class StateComponent : MonoBehaviour
    {
        protected Skeleton skeleton;
        protected MonsterData monsterData;

        protected void Awake()
        {
            skeleton = GetComponent<Skeleton>();
            monsterData = skeleton.monsterData;
        }
    }


    // 기본 상태

    private class Idle : StateComponent, IMonsterAction
    {
        float changePatrolTime;

        private void OnEnable()
        {
            changePatrolTime = 5f;
            skeleton.anim.SetBool("IsIdle", true);
        }

        private void OnDisable()
        {
            skeleton.anim.SetBool("IsIdle", false);
        }

        public void MonsterAction()
        {
            //적을 감지
            changePatrolTime -= Time.deltaTime;
            Vector3 targetPos = skeleton.target.transform.position;

            if (skeleton.CalDistance(targetPos) < monsterData.SearchRange || changePatrolTime < 0f)
            {
                ChangeState(GetComponent<Patrol>());
                //monsterData.MonsterState = MonsterData.State.Patrol;  // (임시) 상태 변수가 꼭 필요한가?, monsterData에 들어있어야 하나?
                return;
            }
        }

        public void ChangeState(IMonsterAction nextState)
        {
            GetComponent<Idle>().enabled = false;
            var _nextState = nextState as StateComponent;

            if (_nextState == null)
            {
                Debug.LogError("Component에서 ISkeletonAction으로 형변환 실패 (Idle)");
                return;
            }

            _nextState.enabled = true;
            skeleton.skeletonAction = nextState;
        }
    }


    // 추격, 발견한 적을 쫒아감

    private class Chase : StateComponent, IMonsterAction
    {
        private float remainTime;
        private void OnEnable()
        {
            remainTime = 2.5f;
            skeleton.anim.SetBool("IsChase", true);
        }

        private void OnDisable()
        {
            skeleton.anim.SetBool("IsChase", false);
        }

        public void MonsterAction()
        {
            remainTime -= Time.deltaTime;
            ChaseMove();
        }

        private void ChaseMove()
        {
            Vector3 targetPos = skeleton.target.transform.position;

            // Chase -> Attack
            if (Mathf.Abs(skeleton.CalDistance(targetPos)) < monsterData.AttackRange)
            {
                ChangeState(GetComponent<Attack>());
                //monsterData.MonsterState = MonsterData.State.Attack; // (임시) 상태 변수가 꼭 필요한가?, monsterData에 들어있어야 하나?
            }

            // Chase -> Go Back
            if (remainTime < 0f && Mathf.Abs(skeleton.CalDistance(targetPos)) > monsterData.SearchRange)
            {
                ChangeState(GetComponent<GoBack>());
                //monsterData.MonsterState = MonsterData.State.GoBack; // (임시) 상태 변수가 꼭 필요한가?, monsterData에 들어있어야 하나?
            }

            skeleton.Move(targetPos, monsterData.MoveSpeed * 2, monsterData.RotateSpeed * 2); 
        }

        public void ChangeState(IMonsterAction nextState)
        {
            GetComponent<Chase>().enabled = false;
            var _nextState = nextState as StateComponent;

            if (_nextState == null)
            {
                Debug.LogError("Component에서 ISkeletonAction으로 형변환 실패 (Chase)");
            }

            _nextState.enabled = true;
            skeleton.skeletonAction = nextState;
        }
    }


    // 정찰, 일정 범위를 계속 순찰함

    private class Patrol : StateComponent, IMonsterAction
    {
        Vector3 destPos;
        private IEnumerator coroutine;

        private void OnEnable()
        {
            coroutine = ChangeDestPos();
            skeleton.anim.SetBool("IsPatrol", true);
            StartCoroutine(coroutine);
        }

        private void OnDisable()
        {
            skeleton.anim.SetBool("IsPatrol", false);
            StopCoroutine(coroutine);
        }

        public void MonsterAction()
        {
            SearchTarget();
            PatrolMove();
        }

        private void PatrolMove()
        {
            if (Mathf.Abs(skeleton.CalDistance(destPos)) < 1)
            {
                ChangeState(GetComponent<Idle>());
                //monsterData.MonsterState = MonsterData.State.Idle; // (임시) 상태 변수가 꼭 필요한가?, monsterData에 들어있어야 하나?
            }

            skeleton.Move(destPos, monsterData.MoveSpeed, monsterData.RotateSpeed);
        }

        private void SearchTarget()
        {
            Vector3 targetPos = skeleton.target.transform.position;
            Debug.DrawLine(destPos, destPos + Vector3.up * 5, Color.red);

            if (skeleton.CalDistance(targetPos) < monsterData.SearchRange)
            {
                ChangeState(GetComponent<Chase>());
                //monsterData.MonsterState = MonsterData.State.Chase; // (임시) 상태 변수가 꼭 필요한가?, monsterData에 들어있어야 하나?
            }
        }

        IEnumerator ChangeDestPos()
        {
            while (true)
            {
                int angle = Random.Range(0, 360);
                destPos = new Vector3(transform.position.x + Mathf.Cos(angle) * monsterData.PatrolRange,
                                      transform.position.y,
                                      transform.position.z + Mathf.Sin(angle) * monsterData.PatrolRange);
                yield return new WaitForSeconds(5.0f);
            }
        }

        public void ChangeState(IMonsterAction nextState)
        {
            GetComponent<Patrol>().enabled = false;
            var _nextState = nextState as StateComponent;

            if (_nextState == null)
            {
                Debug.LogError("Component에서 ISkeletonAction으로 형변환 실패 (Patrol)");
            }

            _nextState.enabled = true;
            skeleton.skeletonAction = nextState;
        }
    }


    // 범위를 벗어나면 원래 위치로 되돌아감

    private class GoBack : StateComponent, IMonsterAction
    {
        private void OnEnable()
        {
            skeleton.anim.SetBool("IsGoBack", true);
        }

        private void OnDisable()
        {
            skeleton.anim.SetBool("IsGoBack", false);
        }

        public void MonsterAction()
        {
            skeleton.Move(skeleton.originPos, monsterData.MoveSpeed * 2, monsterData.RotateSpeed * 4);

            if (Mathf.Abs(skeleton.CalDistance(skeleton.originPos)) < 1)
            {
                ChangeState(GetComponent<Idle>());
                //monsterData.MonsterState = MonsterData.State.Idle; // (임시) 상태 변수가 꼭 필요한가?, monsterData에 들어있어야 하나?
            }
        }

        public void ChangeState(IMonsterAction nextState)
        {
            GetComponent<GoBack>().enabled = false;
            var _nextState = nextState as StateComponent;

            if (_nextState == null)
            {
                Debug.LogError("Component에서 ISkeletonAction으로 형변환 실패 (Patrol)");
            }

            _nextState.enabled = true;
            skeleton.skeletonAction = nextState;
        }
    }


    // 공격

    private class Attack : StateComponent, IMonsterAction
    {
        private void OnEnable()
        {
            skeleton.anim.SetBool("IsAttack", true);
        }

        private void OnDisable()
        {
            skeleton.anim.SetBool("IsAttack", false);
        }

        public void MonsterAction()
        {
            if (skeleton.anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f)
            {
                ChangeState(GetComponent<Chase>());
                //monsterData.MonsterState = MonsterData.State.Chase; // (임시) 상태 변수가 꼭 필요한가?, monsterData에 들어있어야 하나?
                Debug.Log("Attack에서 ChangeState");
            }
        }

        public void ChangeState(IMonsterAction nextState)
        {
            GetComponent<Attack>().enabled = false;
            var _nextState = nextState as StateComponent;

            if (_nextState == null)
            {
                Debug.LogError("Component에서 ISkeletonAction으로 형변환 실패 (Attack)");
            }

            _nextState.enabled = true;
            skeleton.skeletonAction = nextState;
        }
    }


    // 데미지를 받았을 때

    private class Hit : StateComponent, IMonsterAction
    {
        public void MonsterAction()
        {
            //monsterData.Hp--; //(임시) hp변경하지 않고 hp바UI를 변경
        }

        public void ChangeState(IMonsterAction nextState)
        {
            GetComponent<Idle>();
        }
    }


    // 죽었을 때

    private class Die : StateComponent, IMonsterAction
    {
        public void MonsterAction()
        {

        }

        public void ChangeState(IMonsterAction nextState)
        {
            GetComponent<Idle>();
        }
    }

    void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        
    }

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");        // (임시) 타겟을 지정받는 방법 생각해보자
        originPos = transform.position;                             // (임시) 시작위치 저장 시스템 구축 후 데이터 전달받기

        skeletonAction = gameObject.AddComponent<Idle>();
        gameObject.AddComponent<Chase>().enabled = false;
        gameObject.AddComponent<Patrol>().enabled = false;
        gameObject.AddComponent<GoBack>().enabled = false;
        gameObject.AddComponent<Attack>().enabled = false;
        gameObject.AddComponent<Hit>().enabled = false;
        gameObject.AddComponent<Die>().enabled = false;
    }

    void Update()
    {
        skeletonAction.MonsterAction();
    }

    // 움직임

    private void Move(Vector3 targetPos, float moveSpeed, float rotateSpeed)
    {
        Vector3 destDir = targetPos - transform.position;

        Quaternion dirToTarget = Quaternion.LookRotation(destDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, dirToTarget, rotateSpeed);

        rigid.MovePosition(transform.position + transform.forward * moveSpeed * Time.deltaTime);
    }

    // 거리 계산
    
    private float CalDistance(Vector3 targetPos)
    {
        if (Mathf.Abs(targetPos.x - transform.position.x) > 50 ||
            Mathf.Abs(targetPos.y - transform.position.y) > 50 ||
            Mathf.Abs(targetPos.z - transform.position.z) > 50)
        {
            return int.MaxValue;
        }

        float distanceFromTarget = Vector3.Distance(targetPos, transform.position);
        return distanceFromTarget;
    }
}