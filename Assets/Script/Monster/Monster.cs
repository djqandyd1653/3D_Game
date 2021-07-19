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
    // DrawWireArc에 보호수준 어떻게 할지??
    public MonsterData monsterData;

    protected Vector3 originPos;    // (임시) 나중에 변수선언 없이 만들기, 나중에 위치저장 변수에 값을 전달
    [SerializeField]
    protected float hp;             // (임시) 임시 체력, 나중에 hp바 UI와 연동하여 계산하기
    [SerializeField]
    protected HealthBar healthBar;
    [SerializeField]
    protected MonsterData.State state;


    protected GameObject target;
}
