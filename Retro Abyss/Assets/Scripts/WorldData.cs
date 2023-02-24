using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldData", menuName = "Data/World Data")]
public class WorldData : ScriptableObject
{
    public float gravity = 5f;
    public float squareDistance;
    public float maxSpeedNoise;
}
