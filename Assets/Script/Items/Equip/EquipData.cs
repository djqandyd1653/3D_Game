using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipData : ItemData
{
    public enum EquipType
    {
        Weapon,
        Armor,
        Accessory
    }

    [SerializeField]
    private EquipType equipType;
    public EquipType EquipItemType { get { return equipType; } }
}
