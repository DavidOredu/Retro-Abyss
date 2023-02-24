using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Upgrades,
    Powerup,
}
[CreateAssetMenu(fileName = "NewShopItem", menuName = "Data/Shop Item Data")]
public class ShopItem : ScriptableObject
{
    public string _name;
    [TextArea]
    public string description;
    public string purchaseDescription;
    public Sprite image;
    public int level;
    public int maxLevel;
    public int cost;
    public bool isUnlocked;
    public AnimationCurve priceAtLevel;
    public AnimationCurve itemValueAtLevel;
}
