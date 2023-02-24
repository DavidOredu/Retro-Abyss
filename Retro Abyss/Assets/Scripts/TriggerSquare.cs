using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SquareType
{
    Normal,
    Obstacle,
    Powerup,
    SlowDown,
    Currency,
}
public class TriggerSquare : MonoBehaviour
{
    private GameManager gameManager;
    private PlayerController player;
    private WorldController worldController;

    [SerializeField]
    private MeshRenderer meshRenderer = null;

    [SerializeField]
    private Light pointLight;

    [SerializeField]
    private GameObject destructParticle = null;
    [SerializeField]
    private GameObject hitParticle = null;
    private GameObject explosionParticle = null;

    public SquareType squareType { get; private set; }

    private Material material;

    [Header("SLOWDOWN SQUARE")]
    private BlueSquareData blueSquareData;

    [Header("POWERUP SQUARE")]
    private SpriteRenderer powerupIconRenderer;
    private PowerupController powerupController;
    private PowerupActions powerupActions;
    private PowerupBehaviour[] powerups;
    public PowerupBehaviour powerupBehaviour { get; private set; }

    [Header("CURRENCY SQUARE")]
    public YellowSquareData yellowSquareData;

    public bool isDestroyed { get; private set; }
    // Start is called before the first frame update
    void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        worldController = GameObject.FindGameObjectWithTag("WorldController").GetComponent<WorldController>();

        powerupIconRenderer = GetComponentInChildren<SpriteRenderer>();
        powerupIconRenderer.gameObject.SetActive(false);
        powerupBehaviour = GetComponent<PowerupBehaviour>();
        var powerupManager = GameObject.FindGameObjectWithTag("PowerupManager");
        powerupController = powerupManager.GetComponent<PowerupController>();
        powerupActions = powerupManager.GetComponent<PowerupActions>();
        powerupBehaviour.powerupController = powerupController;
        powerups = Resources.LoadAll<PowerupBehaviour>("Powerups");
    }

   
    public void SetSquareType(SquareType squareType)
    {
        isDestroyed = false;
        powerupIconRenderer.gameObject.SetActive(false);

        switch (squareType)
        {
            case SquareType.Normal:
                material = Resources.Load<Material>("TriggerSquare_Normal");
                destructParticle = Resources.Load<GameObject>("WhiteDestructParticle");
                hitParticle = Resources.Load<GameObject>("WhiteHitParticle");
                explosionParticle = Resources.Load<GameObject>("WhiteExplosionParticle");
                break;
            case SquareType.Obstacle:
                material = Resources.Load<Material>("TriggerSquare_Obstacle");
                destructParticle = Resources.Load<GameObject>("RedDestructParticle");
                hitParticle = Resources.Load<GameObject>("RedHitParticle");
                explosionParticle = Resources.Load<GameObject>("RedExplosionParticle");
                break;
            case SquareType.Powerup:
                material = Resources.Load<Material>("TriggerSquare_Powerup");
                destructParticle = Resources.Load<GameObject>("GreenDestructParticle");
                hitParticle = Resources.Load<GameObject>("GreenHitParticle");
                explosionParticle = Resources.Load<GameObject>("GreenExplosionParticle");

                bool hasGottenValidPowerup = false;

                // to use only unlocked or never-locked powerups
                while (!hasGottenValidPowerup)
                {
                    PowerupBehaviour newPowerupBehaviour = powerups[Random.Range(0, powerups.Length)];
                    if (ShopManager.instance.CheckShopItemWithNameExists(newPowerupBehaviour.powerup.name) && newPowerupBehaviour.powerup.name != "Health")
                    {
                        if (ShopManager.instance.GetShopItemWithName(newPowerupBehaviour.powerup.name).isUnlocked)
                        {
                            powerupBehaviour.powerup.Initialize(newPowerupBehaviour.powerup);
                            hasGottenValidPowerup = true;
                        }
                        else
                            hasGottenValidPowerup = false;
                    }
                    else
                    {
                        powerupBehaviour.powerup.Initialize(newPowerupBehaviour.powerup);
                        hasGottenValidPowerup = true;
                    }
                }

                powerupIconRenderer.sprite = powerupBehaviour.powerup.powerupIcon;
                powerupIconRenderer.gameObject.SetActive(true);
                powerupBehaviour.SetPowerupEvents(powerupActions);
                break;
            case SquareType.SlowDown:
                material = Resources.Load<Material>("TriggerSquare_SlowDown");
                destructParticle = Resources.Load<GameObject>("BlueDestructParticle");
                hitParticle = Resources.Load<GameObject>("BlueHitParticle");
                explosionParticle = Resources.Load<GameObject>("BlueExplosionParticle");
                blueSquareData = Resources.Load<BlueSquareData>("BlueSquareData");
                break;
            case SquareType.Currency:
                material = Resources.Load<Material>("TriggerSquare_Currency");
                destructParticle = Resources.Load<GameObject>("YellowDestructParticle");
                hitParticle = Resources.Load<GameObject>("YellowHitParticle");
                explosionParticle = Resources.Load<GameObject>("YellowExplosionParticle");
                yellowSquareData = Resources.Load<YellowSquareData>("YellowSquareData");
                break;
        }
        meshRenderer.material = material;
        pointLight.color = material.color;
        this.squareType = squareType;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(player == null)
                player = other.GetComponent<PlayerController>();

            if (worldController.HasTriggerSquare(player.currentTriggerSquare)) { return; }

            player.currentTriggerSquare = this;

            if(squareType == SquareType.Obstacle)
            {
                if (player.health == 1 && !player.isInvulnerable)
                    ScoreAndActivateSquare();
                player.TakeDamage();
            }

            AudioManager.instance.PlayOneShotSound("GlassShatterPlayer", transform);
            DestroyTriggerSquare();
        }
        if (other.CompareTag("Projectile"))
        {
            // break square effect 
            // disable square
            if(squareType == SquareType.Powerup)
            {
                powerupBehaviour.powerup.powerupAmmo = 0;
            }
            Debug.Log("Collided with projectile");
            AudioManager.instance.PlayOneShotSound("GlassShatterProjectile", transform);
            DestroyTriggerSquare();
        }
    }
    public void DestroyTriggerSquare()
    {
        if (isDestroyed) { return; }
        
        if(!GameManager.instance.isGameOver)
            ScoreAndActivateSquare();
        
        var destruct = Instantiate(destructParticle, transform.position, destructParticle.transform.rotation);
        Instantiate(hitParticle, transform.position, hitParticle.transform.rotation);
        Instantiate(explosionParticle, transform.position, explosionParticle.transform.rotation);
        gameObject.SetActive(false);

        AudioManager.instance.PlaySound("GlassDebris", destruct.transform, true);
        isDestroyed = true;
    }
    private void ScoreAndActivateSquare()
    {
        if (player == null)
            player = FindObjectOfType<PlayerController>();

        ScoreSystem.GameScore.scores[ConstNames.MAIN_SCORE]++;
        FeedbackManager.instance.feedbacksDict[ConstNames.SCORE_TEXT_FEEDBACK].PlayFeedbacks();

        if (squareType != SquareType.Obstacle)
            FeedbackManager.instance.feedbacksDict[ConstNames.COLLISION_FEEDBACK].PlayFeedbacks();
        switch (squareType)
        {
            case SquareType.Normal:
                ScoreSystem.GameScore.scores[ConstNames.NORMAL_TILES_HIT]++;
                break;
            case SquareType.Obstacle:
                ScoreSystem.GameScore.scores[ConstNames.OBSTACLE_TILES_HIT]++;

                if(!player.isInvulnerable)
                    FeedbackManager.instance.feedbacksDict[ConstNames.DAMAGE_FEEDBACK].PlayFeedbacks();
                break;
            case SquareType.Powerup:
                ScoreSystem.GameScore.scores[ConstNames.POWERUP_TILES_HIT]++;

                player.powerupButton.SetupButton(powerupBehaviour);
                break;
            case SquareType.SlowDown:
                ScoreSystem.GameScore.scores[ConstNames.SLOWDOWN_TILES_HIT]++;

                worldController.ReduceMoveSpeed(blueSquareData.moveSpeedReductionPercentage);
                FeedbackManager.instance.StartScreenFrost();
                break;
            case SquareType.Currency:
                ScoreSystem.GameScore.scores[ConstNames.CURRENCY_TILES_HIT]++;

                GameManager.instance.AddCurrency(yellowSquareData.currencyToAdd);
                break;
            default:
                break;
        }
    }
}
