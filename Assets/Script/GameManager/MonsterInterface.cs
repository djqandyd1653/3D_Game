using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMonsterAction
{
    void MonsterAction();                                   // 실행될 동장
    void ChangeComponent(IMonsterAction nextComponent);     // 컴포넌트 교체
}

public interface IDamageable
{
    void ToDamage(float damage);
}

public class MonsterInterface : MonoBehaviour
{

}
