using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IMonsterAction
{
    void MonsterAction();
    void ChangeState(IMonsterAction nextAction);
}

public class MonsterInterface : MonoBehaviour
{

}
