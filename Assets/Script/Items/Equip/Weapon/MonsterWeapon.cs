using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterWeapon : MonoBehaviour
{
    public WeaponData weaponData;
    //public Collider weaponCollider;
    private bool attackAble;
    public bool AttackAble { set { attackAble = value; } }

    //public void SetColliderActive(bool isActive)
    //{
    //    weaponCollider.enabled = isActive;
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(attackAble)
            {
                var target = other.GetComponent<IDamageable>();

                //weaponCollider.enabled = false;
                float damage = weaponData.WeaponDamage;
                GameEvent.Instance.OnEventToDamage(damage, target);
            }
        }
    }
}
