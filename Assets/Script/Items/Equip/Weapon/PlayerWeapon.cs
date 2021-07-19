using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public WeaponData weaponData;
    //public Collider weaponCollider;
    private string strState;
    public string StrState { set { strState = value; } }
    private float attackAnimTime;
    public float AttackAnimTime { set { attackAnimTime = value; } }

    //public void SetColliderActive(bool isActive)
    //{
    //    weaponCollider.enabled = isActive;
    //}

    private void AttackTest(Collider other)
    {
        var monster = other.GetComponent<IDamageable>();

        //weaponCollider.enabled = false;
        float damage = weaponData.WeaponDamage;
        GameEvent.Instance.OnEventToDamage(damage, monster);
    }

    private void AttackTest2(string _stateName, float min, float max, Collider other)
    {
        if(strState == _stateName)
        {
            if (attackAnimTime >= min && attackAnimTime < max)
            {
                AttackTest(other);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            AttackTest2("Attack01", 0.37f, 0.5f, other);
            AttackTest2("Attack02", 0.3f, 0.45f, other);
        }
    }
}
