using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPowerupData", menuName = "Data/Powerup Data")]
public class PowerupData : ScriptableObject
{
    public string _name;
    public int level = 1;
    public float duration = 0f;
    public int maxAmmo = 1;
}
