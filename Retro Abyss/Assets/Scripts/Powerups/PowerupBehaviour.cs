using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupBehaviour : MonoBehaviour
{
    public PowerupController powerupController;

    [SerializeField]
    public Powerup powerup;

    public void ActivatePowerup()
    {
        powerupController.ActivatePowerup(powerup);
    }

    public void SetPowerupEvents(PowerupActions powerupActions)
    {
        powerup.startAction.RemoveAllListeners();
        powerup.activeAction.RemoveAllListeners();
        powerup.endAction.RemoveAllListeners();

        switch (powerup.name)
        {
            case "Shield":
                powerup.startAction.AddListener(powerupActions.ShieldStartAction);
                powerup.activeAction.AddListener(powerupActions.ShieldActiveAction);
                powerup.endAction.AddListener(powerupActions.ShieldEndAction);
                break;
            case "SoundCannon":
                powerup.startAction.AddListener(powerupActions.SoundCannonStartAction);
                powerup.endAction.AddListener(powerupActions.SoundCannonEndAction);
                break;
            case "Health":
                powerup.startAction.AddListener(powerupActions.HealthStartAction);
                powerup.endAction.AddListener(powerupActions.HealthEndAction);
                break;
            case "Invincibility":
                powerup.startAction.AddListener(powerupActions.InvincibleStartAction);
                powerup.endAction.AddListener(powerupActions.InvincibleEndAction);
                break;
            case "Projectile":
                powerup.startAction.AddListener(powerupActions.ProjectileStartAction);
                powerup.activeAction.AddListener(powerupActions.ProjectileActiveAction);
                powerup.endAction.AddListener(powerupActions.ProjectileEndAction);
                powerup.selectedStartAction.AddListener(powerupActions.ProjectileSelectedStartAction);
                powerup.selectedActiveAction.AddListener(powerupActions.ProjectileSelectedActiveAction);
                powerup.selectedEndAction.AddListener(powerupActions.ProjectileSelectedEndAction);
                break;
        }
    }
}
