using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//interface IMonsterAction
//{
//    void MonsterAction();
//    void ChangeState(IMonsterAction nextAction);
//}

public class Monster : MonoBehaviour
{
    // DrawWireArc에 보호수준 어떻게 할지?? => 프로퍼티로 변경해보자
    public MonsterData monsterData;

    // (임시) 나중에 변수선언 없이 만들기, 나중에 위치저장 변수에 값을 전달
    protected Vector3 originPos;    
    public Vector3 OriginPos { get { return originPos; } }

    // (임시) 임시 체력, 나중에 hp바 UI와 연동하여 계산하기
    [SerializeField]
    protected float hp;                                                     
    public float Hp { get { return hp; } set { if (value >= 0) hp = value; } }

    [SerializeField]
    protected HealthBar healthBar;
    public HealthBar HealthBar { get { return healthBar; } }

    [SerializeField]
    protected MonsterData.State state;

    protected GameObject target;
}
