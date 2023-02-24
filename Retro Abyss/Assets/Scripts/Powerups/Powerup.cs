using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Powerup
{
    [SerializeField]
    public string name;

    [SerializeField]
    public float value;

    [SerializeField]
    public float duration;

    [SerializeField]
    public bool isInstant;

    [SerializeField]
    public bool isSelectable;

    [SerializeField]
    public bool isTimeBased = true;

    [SerializeField]
    public bool variableAmmo;

    [SerializeField]
    public int powerupAmmo = 1;

    [SerializeField]
    public Sprite powerupIcon;

    [SerializeField]
    public PowerupData powerupData;

    [SerializeField]
    public UnityEvent startAction;

    [SerializeField]
    public UnityEvent activeAction;

    [SerializeField]
    public UnityEvent endAction;

    [SerializeField]
    public UnityEvent selectedStartAction;

    [SerializeField]
    public UnityEvent selectedActiveAction;

    [SerializeField]
    public UnityEvent selectedEndAction;

    public void Initialize(Powerup powerup)
    {
        name = powerup.name;
        value = powerup.powerupData.level;
        duration = powerup.powerupData.duration;
        isInstant = powerup.isInstant;
        isSelectable = powerup.isSelectable;
        isTimeBased = powerup.isTimeBased;
        powerupIcon = powerup.powerupIcon;
        powerupData = powerup.powerupData;
        variableAmmo = powerup.variableAmmo;
        if(variableAmmo)
            powerupAmmo = UnityEngine.Random.Range(1, powerup.powerupData.maxAmmo + 1);
        else
            powerupAmmo = powerup.powerupData.maxAmmo;
    }
    public void Start()
    {
        startAction?.Invoke();
    }
    public void SelectedStart()
    {
        selectedStartAction?.Invoke();
    }

    public void Active()
    {
        activeAction?.Invoke();
    }
    public void SelectedActive()
    {
        selectedActiveAction?.Invoke();
    }

    public void End()
    {
        endAction?.Invoke();
    }
    public void SelectedEnd()
    {
        selectedEndAction?.Invoke();
    }
}
