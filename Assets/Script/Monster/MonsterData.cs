using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Monster Status Data", menuName = "Scriptable Object Asset/Monster Status", order = 3)]
public class MonsterData : ScriptableObject
{
    public enum State
    {
        Idle,
        Chase,
        Patrol,
        GoBack,
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

    [Header("Status")]

    [SerializeField]
    private string monsterName;
    public string MonsterName { get { return monsterName; } }

    [SerializeField]
    private float hp;
    public float Hp { get { return hp; } }

    [SerializeField]
    private float attackPower;
    public float AttackPower { get { return attackPower; } }

    [SerializeField]
    private float attackSpeed;
    public float AttackSpeed { get { return attackSpeed; } }

    [SerializeField]
    private float moveSpeed;
    public float MoveSpeed { get { return moveSpeed; } }

    [SerializeField]
    private float rotateSpeed;
    public float RotateSpeed { get { return rotateSpeed; } }

    [SerializeField]
    private float armor;
    public float Armor { get { return armor; } }

    [SerializeField]
    private float respawnTime;
    public float RespawnTime { get { return respawnTime; } }

    [SerializeField]
    private Grade monsterGrade;
    public Grade MonsterGrade { get { return monsterGrade; } }

    [Header("State")]
    [SerializeField]
    private State monsterState;
    public State MonsterState { get { return monsterState; } }

    [Header("Range")]
    [SerializeField]
    private float searchRange;
    public float SearchRange { get { return searchRange; } }

    [SerializeField]
    private float patrolRange;
    public float PatrolRange { get { return patrolRange; } }

    [SerializeField]
    private float attackRange;
    public float AttackRange { get { return attackRange; } }
}
