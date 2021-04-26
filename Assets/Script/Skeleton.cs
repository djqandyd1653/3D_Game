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
        void ChangeState();
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
            //적을 감지
            // ture -> Attack()
            // false -> 5초뒤 patrol
            if (Input.GetKey(KeyCode.Space))
            {
                monster.state = Monster.State.Patrol;
                ChangeState();
            }
        }

        public void ChangeState()
        {
            GetComponent<Idle>().enabled = false;
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
            Vector3 targetPos = monster.target.transform.position;

            if(skeleton.CalDistance(targetPos) < monster.attackRange)
            {
                ChangeState();
            }

            skeleton.Move(monster.runSpeed);
        }

        public void ChangeState()
        {
            GetComponent<Chase>().enabled = false;
            GetComponent<Attack>().enabled = true;
            skeleton.skeletonAction = GetComponent<Attack>();
            monster.state = Monster.State.Attack; 
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
            if (Mathf.Abs(skeleton.CalDistance(destPos)) < 1)
                return;

            skeleton.Move(monster.moveSpeed);
        }

        private void SearchTarget()
        {
            Vector3 targetPos = monster.target.transform.position;
            Debug.DrawLine(destPos, destPos + Vector3.up * 5, Color.red);

            if (skeleton.CalDistance(targetPos) < monster.searchRange)
            {
                ChangeState();
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
                //Debug.Log(destPos);
                yield return new WaitForSeconds(3.0f);
            }
        }

        public void ChangeState()
        {
            GetComponent<Patrol>().enabled = false;
            GetComponent<Chase>().enabled = true;
            skeleton.skeletonAction = GetComponent<Chase>();
            monster.state = Monster.State.Chase;
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

        public void ChangeState()
        {
            GetComponent<Idle>();
        }
    }

    private class Hit : StateComponent, ISkeletonAction
    {
        public void Action()
        {
            monster.hp--;
        }

        public void ChangeState()
        {
            GetComponent<Idle>();
        }
    }

    private class Die : StateComponent, ISkeletonAction
    {
        public void Action()
        {

        }

        public void ChangeState()
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
        gameObject.AddComponent<Idle>();
        gameObject.AddComponent<Chase>().enabled = false;
        gameObject.AddComponent<Patrol>().enabled = false;
        gameObject.AddComponent<Attack>().enabled = false;
        gameObject.AddComponent<Hit>().enabled = false;
        gameObject.AddComponent<Die>().enabled = false;

        skeletonAction = GetComponent<Idle>();

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
        monster.attackRange = 3.0f;

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