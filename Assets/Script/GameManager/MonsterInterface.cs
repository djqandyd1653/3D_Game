using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IMonsterAction
{
    void MonsterAction();
    void ChangeComponent(IMonsterAction nextComponent);
}

public class MonsterInterface : MonoBehaviour
{

}
