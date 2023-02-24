using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PoolList", menuName = "Data/Pool List")]
public class PoolList : ScriptableObject
{
    public List<ObjectPooler.Pool> poolsToSpawn = new List<ObjectPooler.Pool>();
}
