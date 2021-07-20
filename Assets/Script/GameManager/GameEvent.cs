using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameEvent : Singleton<GameEvent>
{
    public event Action<GameObject, Vector3, string, float> EventMonsterDead;
    public event Action<float, IDamageable> EventToDamage;
    public event Action<bool> EventSetActiveMainMenu;

    public void OnEventMonsterDead(GameObject monster, Vector3 originPoint, string monsterName, float respawnTime)
    {
        EventMonsterDead?.Invoke(monster, originPoint, monsterName, respawnTime);
    }

    public void OnEventToDamage(float damage, IDamageable target)
    {
        EventToDamage?.Invoke(damage, target);
        target.ToDamage(damage);
    }

    public void OnEventSetActiveMainMenu(bool isOpen)
    {
        EventSetActiveMainMenu?.Invoke(isOpen);
    }
}
