using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : MonoBehaviour
{
    public Animator anim;
    [SerializeField]
    private StateComponent skeletonAction;
    private Monster monster;
    private GameObject Commander;
    private Rigidbody rigid;

    protected abstract class StateComponent : MonoBehaviour
    {
        protected Skeleton skeleton;
        protected Monster monster;

        protected void Awake()
        {
            skeleton = GetComponent<Skeleton>();
            monster = skeleton.monster;
        }

        public abstract void Action();
        public abstract void ChangeState(StateComponent component);
    }

    private class Idle : StateComponent
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

        public override void Action()
        {
            //적을 감지
            changePatrolTime -= Time.deltaTime;
            Vector3 targetPos = monster.target.transform.position;

            if(skeleton.CalDistance(targetPos) < monster.searchRange || changePatrolTime < 0f)
            {
                ChangeState(GetComponent<Patrol>());
                monster.state = Monster.State.Patrol;
                return;
            }
        }

        public override void ChangeState(StateComponent component)
        {
            GetComponent<Idle>().enabled = false;
            component.enabled = true;
            skeleton.skeletonAction = component;

            if(skeleton.skeletonAction == null)
            {
                Debug.LogError("Component에서 ISkeletonAction으로 형변환 실패 (Idle)");
            }
        }
    }

    private class Chase : StateComponent
    {
        private float remainTime;
        private void OnEnable()
        {
            remainTime = 5f;
            monster.rotateSpeed = 0.01f;
            skeleton.anim.SetBool("IsChase", true);
        }

        private void OnDisable()
        {
            skeleton.anim.SetBool("IsChase", false);
        }

        public override void Action()
        {
            remainTime -= Time.deltaTime;
            ChaseMove();
        }

        private void ChaseMove()
        {
            Vector3 targetPos = monster.target.transform.position;

            if(Mathf.Abs(skeleton.CalDistance(targetPos)) < monster.attackRange)
            {
                ChangeState(GetComponent<Attack>());
                monster.state = Monster.State.Attack;
                Debug.Log("Chase -> Attack");
            }

            if(remainTime < 0f && Mathf.Abs(skeleton.CalDistance(targetPos)) > monster.searchRange)
            {
                ChangeState(GetComponent<GoBack>());
                monster.state = Monster.State.GoBack;
            }

            skeleton.Move(targetPos, monster.runSpeed);
        }

        public override void ChangeState(StateComponent component)
        {
            GetComponent<Chase>().enabled = false;
            component.enabled = true;
            skeleton.skeletonAction = component;

            if (skeleton.skeletonAction == null)
            {
                Debug.LogError("Component에서 ISkeletonAction으로 형변환 실패 (Chase)");
            }
        }
    }

    private class Patrol : StateComponent
    {
        Vector3 destPos;
        private IEnumerator coroutine;

        private void OnEnable()
        {
            coroutine = ChangeDestPos();
            monster.rotateSpeed = 0.005f;
            skeleton.anim.SetBool("IsPatrol", true);
            StartCoroutine(coroutine);
        }

        private void OnDisable()
        {
            skeleton.anim.SetBool("IsPatrol", false);
            StopCoroutine(coroutine);
        }

        public override void Action()
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
                yield return new WaitForSeconds(5.0f);
            }
        }

        public override void ChangeState(StateComponent component)
        {
            GetComponent<Patrol>().enabled = false;
            component.enabled = true;
            skeleton.skeletonAction = component;

            if (skeleton.skeletonAction == null)
            {
                Debug.LogError("Component에서 ISkeletonAction으로 형변환 실패 (Patrol)");
            }
        }
    }

    private class GoBack : StateComponent
    {
        private void OnEnable()
        {
            monster.rotateSpeed = 0.1f;
            skeleton.anim.SetBool("IsGoBack", true);
        }

        private void OnDisable()
        {
            skeleton.anim.SetBool("IsGoBack", false);
        }

        public override void Action()
        {
            skeleton.Move(monster.originPos, monster.runSpeed);

            if (Mathf.Abs(skeleton.CalDistance(monster.originPos)) < 1)
            {
                ChangeState(GetComponent<Idle>());
                monster.state = Monster.State.Idle;
            }
        }

        public override void ChangeState(StateComponent component)
        {
            GetComponent<GoBack>().enabled = false;
            component.enabled = true;
            skeleton.skeletonAction = component;

            if (skeleton.skeletonAction == null)
            {
                Debug.LogError("Component에서 ISkeletonAction으로 형변환 실패 (Patrol)");
            }
        }
    }

    private class Attack : StateComponent
    {
        private void OnEnable()
        {
            skeleton.anim.SetBool("IsAttack", true);
        }

        private void OnDisable()
        {
            skeleton.anim.SetBool("IsAttack", false);
        }

        public override void Action()
        {
            if(
                skeleton.anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f)
            {
                ChangeState(GetComponent<Chase>());
                monster.state = Monster.State.Chase;
                Debug.Log("Attack에서 ChangeState");
            }
        }

        public override void ChangeState(StateComponent component)
        {
            GetComponent<Attack>().enabled = false;
            component.enabled = true;
            skeleton.skeletonAction = component;

            if (skeleton.skeletonAction == null)
            {
                Debug.LogError("Component에서 ISkeletonAction으로 형변환 실패 (Attack)");
            }
        }
    }

    private class Hit : StateComponent
    {
        public override void Action()
        {
            monster.hp--;
        }

        public override void ChangeState(StateComponent component)
        {
            GetComponent<Idle>();
        }
    }

    private class Die : StateComponent
    {
        public override void Action()
        {

        }

        public override void ChangeState(StateComponent component)
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
        gameObject.AddComponent<GoBack>().enabled = false;
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

        monster.originPos = transform.position;

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

        float distanceFromTarget = Vector3.Distance(targetPos, transform.position);
        return distanceFromTarget;
    }
}