using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewProbabilityData", menuName = "Data/Probability Data")]
public class ProbabilityData : ScriptableObject
{
    public AnimationCurve probabilityCurve;
}
