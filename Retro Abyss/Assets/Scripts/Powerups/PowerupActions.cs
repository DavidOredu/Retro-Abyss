using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PowerupActions : MonoBehaviour
{
    private PlayerController player;
    private PowerupController powerupController;
    private WorldController worldController;
    private GameObject shield;
    private Material shieldMat;

    [SerializeField]
    private AnimationCurve shieldCrackCurve;

    [SerializeField]
    private float crackSpeed = .1f;
    [SerializeField]
    private float repairSpeed = .2f;
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        powerupController = GetComponent<PowerupController>();
        worldController = FindObjectOfType<WorldController>();
        shield = Camera.main.transform.Find("Shield").gameObject;
        shieldMat = Resources.Load<Material>("ShieldMat");

        shield.SetActive(false);
    }

    #region Shield Powerup
    public void ShieldStartAction()
    {
        Powerup powerup = null;
        foreach (var p in powerupController.powerupIdentifier.Values)
        {
            if (p.name == "Shield")
            {
                powerup = p;
                break;
            }
        }

        if(powerup != null)
        {
            powerup.value = powerup.powerupData.level;
        }

        shield.gameObject.SetActive(true);
        player.isInvulnerable = true;
        shieldMat.SetFloat("_CrackAmount", 0f);
        AudioManager.instance.PlaySound("ShieldHum");
    }
    public void ShieldActiveAction()
    {
        Powerup powerup = null;
        foreach (var p in powerupController.powerupIdentifier.Values)
        {
            if (p.name == "Shield")
            {
                powerup = p;
                break;
            }
        }
        if (player.justTookDamage)
        {
            powerup.value--;
            float shieldStrengthNormalized = powerup.value / powerup.powerupData.level;
            Debug.Log(shieldStrengthNormalized);
            if (shieldStrengthNormalized <= 0)
                AudioManager.instance.PlayOneShotSound("ShieldDestroyed");
            else if (shieldStrengthNormalized <= .4f)
                AudioManager.instance.PlayOneShotSound("ShieldCrackLowStrength");
            else
                AudioManager.instance.PlayOneShotSound("ShieldCrack");

            player.justTookDamage = false;
        }

        float amount = Remap(powerup.value, 0, powerup.powerupData.level, 2f, 1f);

        float currentCrackValue = shieldMat.GetFloat("_CrackAmount");
        float finalCrackValue = shieldCrackCurve.Evaluate(amount);

        if (currentCrackValue > finalCrackValue)
            shieldMat.SetFloat("_CrackAmount", Mathf.Lerp(currentCrackValue, finalCrackValue, repairSpeed));
        else if (currentCrackValue < finalCrackValue)
            shieldMat.SetFloat("_CrackAmount", Mathf.Lerp(currentCrackValue, finalCrackValue, crackSpeed));

    }
    public void ShieldEndAction()
    {
        var shieldDestruct = Resources.Load<GameObject>("ShieldDestructParticle");
        Instantiate(shieldDestruct, shield.transform.position, shieldDestruct.transform.rotation);
        player.isInvulnerable = false;
        shield.gameObject.SetActive(false);
        AudioManager.instance.StopSound("ShieldHum");
    }
    #endregion

    #region Health Powerup
    public void HealthStartAction()
    {
        player.IncreaseHealth(1);
    }

    public void HealthEndAction()
    {

    }
    #endregion

    #region Sound Cannon Powerup
    public void SoundCannonStartAction()
    {
        foreach (TriggerSquare triggerSquare in worldController.triggerSquares)
        {
            if (triggerSquare.squareType == SquareType.Obstacle)
            {
                AudioManager.instance.PlayOneShotSound("GlassShatterProjectile", triggerSquare.transform, true);
                triggerSquare.DestroyTriggerSquare();
            }
        }
    }
    public void SoundCannonEndAction()
    {

    }
    #endregion

    #region Invincibility Powerup
    public void InvincibleStartAction()
    {
        // make player invulnerable
        // increase world speed
        // display feedbacks
        player.isInvulnerable = true;
        worldController.IncreaseMoveSpeed(5f);
        worldController.canAccelerate = false;
        player.isInvincible = true;
    }
    public void InvincibleEndAction()
    {
        player.isInvulnerable = false;
        worldController.ReduceMoveSpeed(.75f);
        worldController.canAccelerate = true;
        player.isInvincible = false;
        FeedbackManager.instance.feedbacksDict[ConstNames.INVINCIBILITY_END_FEEDBACK].PlayFeedbacks();
        // return to normal
    }
    #endregion

    #region Projectile Powerup
    public void ProjectileStartAction()
    {
        
    }
    public void ProjectileSelectedStartAction()
    {
        player.projectileActive = true;
    }
    public void ProjectileActiveAction()
    {
        if (player.hasShot)
        {
            var powerup = player.powerupButton.powerupBehaviour.powerup;
            powerup.powerupAmmo--;

            if(powerup.powerupAmmo <= 0)
            {
                powerup.value = 0;
            }

            player.powerupButton.DeselectPowerup();
            player.hasShot = false;
        }
    }
    public void ProjectileSelectedActiveAction()
    {

    }
    public void ProjectileEndAction()
    {
        player.projectileActive = false;
    }
    public void ProjectileSelectedEndAction()
    {
        player.projectileActive = false;
    }
    #endregion

    #region Coins
    public void CoinStartAction()
    {

    }

    public void CoinEndAction()
    {

    }
    #endregion

    #region Other Functions

    private float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    #endregion
}
