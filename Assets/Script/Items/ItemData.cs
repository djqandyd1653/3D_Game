using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData : ScriptableObject
{
    public enum Type
    {
        Equip,
        Consum,
        Ect
    }

    public enum Tier
    {
        Legendary,
        Unique,
        Rare,
        UnCommon,
        Common
    }

    [SerializeField]
    private string itemName;
    public string ItemName { get { return itemName; } }

    [SerializeField]
    private Type itemType;
    public Type ItemType { get { return itemType; } }

    [SerializeField]
    private Tier itemTier;
    public Tier ItemTier { get { return itemTier; } }

    [SerializeField]
    private int price;
    public int Price { get { return price; } }
}
