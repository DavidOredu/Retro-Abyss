using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(fileName = "SettingsData", menuName = "Data/Settings Data")]
public class SettingsData : ScriptableObject
{
    public UniversalRenderPipelineAsset pipelineSetting;
    public bool motionBlur;
    public bool filmGrain;
    public bool bloom;
    public bool music;
    public bool sound;
    public bool vibration;
}
