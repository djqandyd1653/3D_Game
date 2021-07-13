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
            if (skeleton.state == MonsterData.State.Idle)
            {
                skeleton.anim.Play("Idle");
                Debug.Log("Idle Animation Start");
                changePatrolTime = 5f;
            }
        }

        private void OnDisable()
        {

        }

        public void MonsterAction()
        {
            //적을 감지
            changePatrolTime -= Time.deltaTime;
            Vector3 targetPos = skeleton.target.transform.position;

            if (skeleton.CalDistance(targetPos) < monsterData.SearchRange || changePatrolTime < 0f)
            {
                skeleton.state = MonsterData.State.Patrol;
                ChangeComponent(GetComponent<Patrol>());
                //monsterData.MonsterState = MonsterData.State.Patrol;  // (임시) 상태 변수가 꼭 필요한가?, monsterData에 들어있어야 하나?
                Debug.Log("Idle -> Patrol");
                return;
            }
        }

        public void ChangeComponent(IMonsterAction nextComponent)
        {
            GetComponent<Idle>().enabled = false;
            var _nextComponent = nextComponent as StateComponent;

            if (_nextComponent == null)
            {
                Debug.LogError("Component에서 ISkeletonAction으로 형변환 실패 (Idle)");
                return;
            }

            _nextComponent.enabled = true;
            skeleton.skeletonAction = nextComponent;
        }
    }


    // 추격, 발견한 적을 쫒아감

    private class Chase : StateComponent, IMonsterAction
    {
        private float remainTime;
        private void OnEnable()
        {
            if(skeleton.state == MonsterData.State.Chase)
            {
                remainTime = 2.5f;
                skeleton.anim.Play("Chase");
                Debug.Log("Chase Animation Start");
            }
        }

        private void OnDisable()
        {

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
                skeleton.state = MonsterData.State.Attack;
                ChangeComponent(GetComponent<Attack>());
                //monsterData.MonsterState = MonsterData.State.Attack; // (임시) 상태 변수가 꼭 필요한가?, monsterData에 들어있어야 하나?
            }

            // Chase -> Go Back
            if (remainTime < 0f && Mathf.Abs(skeleton.CalDistance(targetPos)) > monsterData.SearchRange)
            {
                skeleton.state = MonsterData.State.GoBack;
                ChangeComponent(GetComponent<GoBack>());
                //monsterData.MonsterState = MonsterData.State.GoBack; // (임시) 상태 변수가 꼭 필요한가?, monsterData에 들어있어야 하나?
            }

            skeleton.Move(targetPos, monsterData.MoveSpeed * 2, monsterData.RotateSpeed * 2); 
        }

        public void ChangeComponent(IMonsterAction nextComponent)
        {
            GetComponent<Chase>().enabled = false;
            var _nextComponent = nextComponent as StateComponent;

            if (_nextComponent == null)
            {
                Debug.LogError("Component에서 ISkeletonAction으로 형변환 실패 (Chase)");
            }

            _nextComponent.enabled = true;
            skeleton.skeletonAction = nextComponent;
        }
    }


    // 정찰, 일정 범위를 계속 순찰함

    private class Patrol : StateComponent, IMonsterAction
    {
        Vector3 destPos;
        private IEnumerator coroutine;

        private void OnEnable()
        {
            if (skeleton.state == MonsterData.State.Patrol)
            {
                skeleton.anim.Play("Patrol");
                Debug.Log("Patrol Animation Start");
                coroutine = ChangeDestPos();
                StartCoroutine(coroutine);
            }
        }

        private void OnDisable()
        {
            if(coroutine != null)
            {
                StopCoroutine(coroutine);
            }
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
                skeleton.state = MonsterData.State.Idle;
                ChangeComponent(GetComponent<Idle>());
                //monsterData.MonsterState = MonsterData.State.Idle; // (임시) 상태 변수가 꼭 필요한가?, monsterData에 들어있어야 하나?
                Debug.Log("Patrol -> Idle");
            }

            skeleton.Move(destPos, monsterData.MoveSpeed, monsterData.RotateSpeed);
        }

        private void SearchTarget()
        {
            Vector3 targetPos = skeleton.target.transform.position;
            Debug.DrawLine(destPos, destPos + Vector3.up * 5, Color.red);

            if (skeleton.CalDistance(targetPos) < monsterData.SearchRange)
            {
                skeleton.state = MonsterData.State.Chase;
                ChangeComponent(GetComponent<Chase>());
                //monsterData.MonsterState = MonsterData.State.Chase; // (임시) 상태 변수가 꼭 필요한가?, monsterData에 들어있어야 하나?
                Debug.Log("Patrol -> Chase");
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

        public void ChangeComponent(IMonsterAction nextComponent)
        {
            GetComponent<Patrol>().enabled = false;
            var _nextComponent = nextComponent as StateComponent;

            if (_nextComponent == null)
            {
                Debug.LogError("Component에서 ISkeletonAction으로 형변환 실패 (Patrol)");
            }

            _nextComponent.enabled = true;
            skeleton.skeletonAction = nextComponent;
        }
    }


    // 범위를 벗어나면 원래 위치로 되돌아감

    private class GoBack : StateComponent, IMonsterAction
    {
        private void OnEnable()
        {
            if(skeleton.state == MonsterData.State.GoBack)
            {
                skeleton.anim.Play("GoBack");
            }
        }

        private void OnDisable()
        {

        }

        public void MonsterAction()
        {
            skeleton.Move(skeleton.originPos, monsterData.MoveSpeed * 2, monsterData.RotateSpeed * 4);

            if (Mathf.Abs(skeleton.CalDistance(skeleton.originPos)) < 1)
            {
                skeleton.state = MonsterData.State.Idle;
                ChangeComponent(GetComponent<Idle>());
                //monsterData.MonsterState = MonsterData.State.Idle; // (임시) 상태 변수가 꼭 필요한가?, monsterData에 들어있어야 하나?
            }
        }

        public void ChangeComponent(IMonsterAction nextComponent)
        {
            GetComponent<GoBack>().enabled = false;
            var _nextComponent = nextComponent as StateComponent;

            if (_nextComponent == null)
            {
                Debug.LogError("Component에서 ISkeletonAction으로 형변환 실패 (Patrol)");
            }

            _nextComponent.enabled = true;
            skeleton.skeletonAction = nextComponent;
        }
    }


    // 공격

    private class Attack : StateComponent, IMonsterAction
    {
        private void OnEnable()
        {
            if(skeleton.state == MonsterData.State.Attack)
            {
                skeleton.anim.Play("Attack");
                Debug.Log("Attack Animation Start");
            }
        }

        private void OnDisable()
        {
            
        }

        public void MonsterAction()
        {
            if (skeleton.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                skeleton.state = MonsterData.State.Chase;
                ChangeComponent(GetComponent<Chase>());
                //monsterData.MonsterState = MonsterData.State.Chase; // (임시) 상태 변수가 꼭 필요한가?, monsterData에 들어있어야 하나?
                //Debug.Log("Attack에서 ChangeState");
            }
        }

        public void ChangeComponent(IMonsterAction nextComponent)
        {
            GetComponent<Attack>().enabled = false;
            var _nextComponent = nextComponent as StateComponent;

            if (_nextComponent == null)
            {
                Debug.LogError("ISkeletonAction에서 Component로 형변환 실패 (Attack)");
            }

            _nextComponent.enabled = true;
            skeleton.skeletonAction = nextComponent;
        }
    }


    // 데미지를 받았을 때

    private class Hit : StateComponent, IMonsterAction
    {
        private void OnEnable()
        {
            if(skeleton.state == MonsterData.State.Hit)
            {
                skeleton.anim.Play("Hit");
            }
        }

        private void OnDisable()
        {

        }

        public void MonsterAction()
        {
            if(skeleton.hp <= 0)
            {
                skeleton.state = MonsterData.State.Die;
                ChangeComponent(GetComponent<Die>());
                Debug.Log("Hit -> Die");
            }

            if(!skeleton.anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Hit"))
            {
                //skeleton.anim.Play("Hit");
            }

            if(skeleton.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                skeleton.state = MonsterData.State.Idle;
                ChangeComponent(GetComponent<Idle>());
                Debug.Log("Hit -> Idle");
            }
        }

        public void ChangeComponent(IMonsterAction nextComponent)
        {
            GetComponent<Hit>().enabled = false;
            var _nextComponent = nextComponent as StateComponent;

            if(_nextComponent == null)
            {
                Debug.LogError("ISkeletonAction에서 Component로 형변환 실패 (Hit)");
            }

            _nextComponent.enabled = true;
            skeleton.skeletonAction = nextComponent;
        }
    }


    // 죽었을 때

    private class Die : StateComponent, IMonsterAction
    {
        private void OnEnable()
        {
            if(skeleton.state == MonsterData.State.Die)
            {
                skeleton.anim.Play("Die");
            }
        }

        public void MonsterAction()
        {
            if (skeleton.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                GameEvent.Instance.OnEventMonsterDead(this.gameObject, skeleton.originPos, monsterData.MonsterName, monsterData.RespawnTime);
                skeleton.state = MonsterData.State.Idle;
                ChangeComponent(GetComponent<Idle>());
                Debug.Log("Die에서 ChangeState");
            }
        }

        public void ChangeComponent(IMonsterAction nextComponent)
        {
            GetComponent<Die>().enabled = false;
            var _nextComponent = nextComponent as StateComponent;

            if (_nextComponent == null)
            {
                Debug.LogError("ISkeletonAction에서 Component로 형변환 실패 (Die)");
            }

            _nextComponent.enabled = true;
            skeleton.skeletonAction = nextComponent;
        }
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        originPos = transform.position;                             // (임시) 시작위치 저장 시스템 구축 후 데이터 전달받기
        hp = monsterData.Hp;
        state = MonsterData.State.Idle;
    }

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");        // (임시) 타겟을 지정받는 방법 생각해보자
        
        skeletonAction = gameObject.AddComponent<Idle>();
        gameObject.AddComponent<Chase>().enabled = false;
        gameObject.AddComponent<Patrol>().enabled = false;
        gameObject.AddComponent<GoBack>().enabled = false;
        gameObject.AddComponent<Attack>().enabled = false;
        gameObject.AddComponent<Hit>().enabled = false;
        gameObject.AddComponent<Die>().enabled = false;
    }

    private void Update()
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            var player = target.GetComponent<PlayerController>();
            float animationTime = player.CurrAnimationNormalizedTime();

            if (animationTime > 0.5f)
            {
                return;
            }

            if (player.PlayerState == Player.State.Attack01 || player.PlayerState == Player.State.Attack02)
            {
                hp -= player.AttackPower;

                if(state == MonsterData.State.Hit)
                {
                    anim.Play("Hit", -1, 0);
                    return;
                }

                state = MonsterData.State.Hit;
                skeletonAction.ChangeComponent(GetComponent<Hit>());
                Debug.Log("Hit!!");
            }
        }
    }
}