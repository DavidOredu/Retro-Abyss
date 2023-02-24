using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewScoreData", menuName = "Data/Score Data")]
public class ScoreData : ScriptableObject
{
    public int maxNormalTiles;
    public int maxObstacleTiles;
    public int maxPowerupTiles;
    public int maxSlowDownTiles;
    public int maxCurrencyTiles;
    public int maxTiles;
    public int maxDistanceCovered;
}
