using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Item Data", menuName = "Scriptable Item Asset/Create Weapon Item Data")]
public class WeaponData : EquipData
{
    public enum WeaponType
    {
        Sword,
        Staff,
        Bow,
    }

    [SerializeField]
    private int weaponDamage;
    public int WeaponDamage { get { return weaponDamage; } }

    [SerializeField]
    private WeaponType weaponType;
    public WeaponType weaponItemType { get { return weaponType; } }

}
