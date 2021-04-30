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
        void ChangeState(Behaviour component);
    }

    protected class StateComponent : MonoBehaviour
    {
        protected Skeleton skeleton;
        protected Monster monster;

        virtual protected void Awake()
        {
            skeleton = GetComponent<Skeleton>();
            monster = skeleton.monster;
        }
    }

    private class Idle : StateComponent, ISkeletonAction
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

        public void Action()
        {
            //적을 감지
            changePatrolTime -= Time.deltaTime;
            Vector3 targetPos = monster.target.transform.position;
            if(skeleton.CalDistance(targetPos) < monster.attackRange)
            {
                ChangeState(GetComponent<Attack>());
                monster.state = Monster.State.Attack;
                return;
            }

            if(skeleton.CalDistance(targetPos) < monster.searchRange || changePatrolTime < 0f)
            {
                ChangeState(GetComponent<Patrol>());
                monster.state = Monster.State.Patrol;
                return;
            }
        }

        public void ChangeState(Behaviour component)
        {
            GetComponent<Idle>().enabled = false;
            component.enabled = true;
            skeleton.skeletonAction = component as ISkeletonAction;

            if(skeleton.skeletonAction == null)
            {
                Debug.LogError("Component에서 ISkeletonAction으로 형변환 실패 (Idle)");
            }
        }
    }

    private class Chase : StateComponent, ISkeletonAction
    {
        private void OnEnable()
        {
            monster.rotateSpeed = 0.01f;
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
            Vector3 targetPos = monster.target.transform.position;

            if(skeleton.CalDistance(targetPos) < monster.attackRange)
            {
                ChangeState(GetComponent<Attack>());
                monster.state = Monster.State.Attack;
            }

            skeleton.Move(targetPos, monster.runSpeed);
        }

        public void ChangeState(Behaviour component)
        {
            GetComponent<Chase>().enabled = false;
            component.enabled = true;
            skeleton.skeletonAction = component as ISkeletonAction;

            if (skeleton.skeletonAction == null)
            {
                Debug.LogError("Component에서 ISkeletonAction으로 형변환 실패 (Chase)");
            }
        }
    }

    private class Patrol : StateComponent, ISkeletonAction
    {
        Vector3 destPos;
        private IEnumerator coroutine;

        private void OnEnable()
        {
            coroutine = ChangeDestPos();
            monster.rotateSpeed = 0.005f;
            skeleton.anim.SetBool("IsPatrol", true);
            Debug.Log("코루틴 시작");
            StartCoroutine(coroutine);
        }

        private void OnDisable()
        {
            skeleton.anim.SetBool("IsPatrol", false);
            Debug.Log("코루틴 멈춤");
            StopCoroutine(coroutine);
        }

        public void Action()
        {
            SearchTarget();
            PatrolMove();
        }

        private void PatrolMove()
        {
            if (Mathf.Abs(skeleton.CalDistance(destPos)) < 1)
            {
                ChangeState(GetComponent<Idle>());
                monster.state = Monster.State.Idle;
            }

            skeleton.Move(destPos, monster.moveSpeed);
        }

        private void SearchTarget()
        {
            Vector3 targetPos = monster.target.transform.position;
            Debug.DrawLine(destPos, destPos + Vector3.up * 5, Color.red);

            if (skeleton.CalDistance(targetPos) < monster.searchRange)
            {
                ChangeState(GetComponent<Chase>());
                monster.state = Monster.State.Chase;
            }
        }

        IEnumerator ChangeDestPos()
        {
            while (true)
            {
                int angle = Random.Range(0, 360);
                destPos = new Vector3(transform.position.x + Mathf.Cos(angle) * monster.patrolRange,
                                      transform.position.y,
                                      transform.position.z + Mathf.Sin(angle) * monster.patrolRange);
                Debug.Log("위치 변경");
                yield return new WaitForSeconds(5.0f);
            }
        }

        public void ChangeState(Behaviour component)
        {
            GetComponent<Patrol>().enabled = false;
            component.enabled = true;
            skeleton.skeletonAction = component as ISkeletonAction;

            if (skeleton.skeletonAction == null)
            {
                Debug.LogError("Component에서 ISkeletonAction으로 형변환 실패 (Patrol)");
            }
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
            if(skeleton.anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") &&
                skeleton.anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f)
            {
                ChangeState(GetComponent<Idle>());
                monster.state = Monster.State.Idle;
            }
        }

        public void ChangeState(Behaviour component)
        {
            GetComponent<Attack>().enabled = false;
            component.enabled = true;
            skeleton.skeletonAction = component as ISkeletonAction;

            if (skeleton.skeletonAction == null)
            {
                Debug.LogError("Component에서 ISkeletonAction으로 형변환 실패 (Attack)");
            }
        }
    }

    private class Hit : StateComponent, ISkeletonAction
    {
        public void Action()
        {
            monster.hp--;
        }

        public void ChangeState(Behaviour component)
        {
            GetComponent<Idle>();
        }
    }

    private class Die : StateComponent, ISkeletonAction
    {
        public void Action()
        {

        }

        public void ChangeState(Behaviour component)
        {
            GetComponent<Idle>();
        }
    }

    void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        monster = GetComponent<Monster>();
    }

    void Start()
    {
        skeletonAction = gameObject.AddComponent<Idle>();
        gameObject.AddComponent<Chase>().enabled = false;
        gameObject.AddComponent<Patrol>().enabled = false;
        gameObject.AddComponent<Attack>().enabled = false;
        gameObject.AddComponent<Hit>().enabled = false;
        gameObject.AddComponent<Die>().enabled = false;

        monster.maxHp = 100f;
        monster.hp = monster.maxHp;

        monster.attackPower = 3f;
        monster.attackSpeed = 10f;

        monster.moveSpeed = 10f;
        monster.runSpeed = 20f;

        monster.armor = 5f;

        monster.searchRange = 15.0f;
        monster.patrolRange = 10.0f;
        monster.attackRange = 5.0f;

        monster.state = Monster.State.Idle;
        monster.grade = Monster.Grade.Common;
    }

    void Update()
    {
        skeletonAction.Action();
    }

    private void Move(Vector3 targetPos, float speed)
    {
        Vector3 destDir = targetPos - transform.position;

        Quaternion dirToTarget = Quaternion.LookRotation(destDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, dirToTarget, monster.rotateSpeed);

        rigid.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
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
}