using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMonsterAction
{
    void MonsterAction();
    void ChangeComponent(IMonsterAction nextComponent);
}

public interface IDamageable
{
    void ToDamage(float damage);
}

public class MonsterInterface : MonoBehaviour
{

}
