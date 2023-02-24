using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using Cinemachine;

public class GameManager : SingletonDontDestroy<GameManager>
{
    public static event Action OnGameStart;
    public static event Action OnGameEnd;

    public bool canStartGame;
    public bool inGame;
    public bool isGameOver;
    public bool GameIsPaused;

    public int targetFPS = 60;

    public TextMeshProUGUI currencyText;
    public TextMeshProUGUI powerupAmmoText;

    private PlayerData playerData;
    private WorldData worldData;
    private CurrencyData currencyData;
    private PowerupData[] powerupDatas;
    
    public NoiseSettings noiseProfile;
    public GameObject healthBar;
    public Transform healthSlots;
    public Button PowerupButton;
    public GameObject eventSystemPrefab;
    public BaseInputModule inputModule;

    public List<GameObject> healthBars = new List<GameObject>();

    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();

        Application.targetFrameRate = targetFPS;
         
        playerData = Resources.Load<PlayerData>("PlayerData");
        worldData = Resources.Load<WorldData>("WorldData");
        currencyData = Resources.Load<CurrencyData>("CurrencyData");
        powerupDatas = Resources.LoadAll<PowerupData>("PowerupData");

        OnGameStart += StartGame;

        OnGameEnd += DisplayGameOverPopUp;
        OnGameEnd += EndGame;

        ShopManager.PurchaseDecriptionHandler += HealthDescriptionHandler;
        ShopManager.PurchaseDecriptionHandler += GravityDescriptionHandler;
        ShopManager.PurchaseDecriptionHandler += TileDistanceDescriptionHandler;
        ShopManager.PurchaseDecriptionHandler += SteadyFallDescriptionHandler;
        ShopManager.PurchaseDecriptionHandler += SlowDownTileDescriptionHandler;
        ShopManager.PurchaseDecriptionHandler += CurrencyTileDescriptionHandler;
        ShopManager.PurchaseDecriptionHandler += ShieldDescriptionHandler;
        ShopManager.PurchaseDecriptionHandler += BombDescriptionHandler;
        ShopManager.PurchaseDecriptionHandler += InvincibilityDescriptionHandler;
        ShopManager.PurchaseDecriptionHandler += ProjectileDescriptionHandler;

        ShopManager.OnPurchaseHandler += HealthPurchaseHandler;
        ShopManager.OnPurchaseHandler += GravityPurchaseHandler;
        ShopManager.OnPurchaseHandler += TileDistancePurchaseHandler;
        ShopManager.OnPurchaseHandler += SteadyFallPurchaseHandler;
        ShopManager.OnPurchaseHandler += ShieldPurchaseHandler;
        ShopManager.OnPurchaseHandler += BombPurchaseHandler;
        ShopManager.OnPurchaseHandler += InvincibilityPurchaseHandler;
        ShopManager.OnPurchaseHandler += ProjectilePurchaseHandler;
        //var eventSystemGO = Instantiate(eventSystemPrefab);
        //var eventSystem = eventSystemGO.GetComponent<EventSystem>();
        //EventSystem.current = eventSystem;
        //eventSystem.currentInputModule.ActivateModule();
    }

    private void Start()
    {
        SetupHealth();
    }

    # region SHOP PURCHASE FUNCTIONS 
    private void HealthDescriptionHandler(ShopItem item)
    {
        if (item._name != "Health") { return; }

        if (!(item.level >= item.maxLevel))
            item.purchaseDescription = $"Increase Player health amount to {item.itemValueAtLevel.Evaluate(item.level + 1)}.";
        else
            item.purchaseDescription = "Maximum health level reached.";
    }
    private void HealthPurchaseHandler(ShopItem item)
    {
        if(item._name != "Health") { return; }

        playerData.health = Mathf.CeilToInt(item.itemValueAtLevel.Evaluate(item.level));

        var healthBarGO = Instantiate(healthBar, healthSlots);
        healthBars.Add(healthBarGO);
    }
    
    private void GravityDescriptionHandler(ShopItem item)
    {
        if (item._name != "Gravity") { return; }

        if (!(item.level >= item.maxLevel))
            item.purchaseDescription = $"Reduce Gravity to {item.itemValueAtLevel.Evaluate(item.level + 1)}.";
        else
            item.purchaseDescription = "Minimum Gravity reached.";
    }
    private void GravityPurchaseHandler(ShopItem item)
    {
        if (item._name != "Gravity") { return; }

        worldData.gravity = item.itemValueAtLevel.Evaluate(item.level);
    }
    private void TileDistanceDescriptionHandler(ShopItem item)
    {
        if (item._name != "Tile Distance") { return; }

        if (!(item.level >= item.maxLevel))
            item.purchaseDescription = $"Increase Tile distance by {-(item.itemValueAtLevel.Evaluate(item.level + 1) - item.itemValueAtLevel.Evaluate(item.level))} metres.";
        else
            item.purchaseDescription = "Maximum Tile Distance reached.";
    }
    private void TileDistancePurchaseHandler(ShopItem item)
    {
        if (item._name != "Tile Distance") { return; }

        worldData.squareDistance = item.itemValueAtLevel.Evaluate(item.level);
    }
    private void SteadyFallDescriptionHandler(ShopItem item)
    {
        if (item._name != "Steady Fall") { return; }

        if (!(item.level >= item.maxLevel))
            item.purchaseDescription = $"Steady Fall by {-(item.itemValueAtLevel.Evaluate(item.level + 1) - item.itemValueAtLevel.Evaluate(item.level))} metres.";
        else
            item.purchaseDescription = "Maximum 'steadiness' reached.";
    }
    private void SteadyFallPurchaseHandler(ShopItem item)
    {
        if (item._name != "Steady Fall") { return; }

        worldData.maxSpeedNoise = item.itemValueAtLevel.Evaluate(item.level);
    }
    private void SlowDownTileDescriptionHandler(ShopItem item)
    {
        if (item._name != "SlowDown") { return; }

        if (!(item.level >= item.maxLevel))
            item.purchaseDescription = "Unlock Slow Down Tile.";
        else
            item.purchaseDescription = "Slow Down Tile unlocked.";
    }
    private void CurrencyTileDescriptionHandler(ShopItem item)
    {
        if (item._name != "Currency") { return; }

        if (!(item.level >= item.maxLevel))
            item.purchaseDescription = "Unlock Currency Tile.";
        else
            item.purchaseDescription = "Currency Tile unlocked.";
    }

    private void ShieldDescriptionHandler(ShopItem item)
    {
        if (item._name != "Shield") { return; }

        if (item.level <= 0)
            item.purchaseDescription = "Unlock Shield Powerup.";
        else
        {
            if (!(item.level >= item.maxLevel))
                item.purchaseDescription = $"Increase shield strength to {item.itemValueAtLevel.Evaluate(item.level + 1)} hits.";
            else
                item.purchaseDescription = "Maximum shield capacity.";
        }
    }
    private void ShieldPurchaseHandler(ShopItem item)
    {
        if (item._name != "Shield") { return; }

        var data = GetPowerupDataWithName("Shield");
        data.level = Mathf.CeilToInt(item.itemValueAtLevel.Evaluate(item.level));
    }

    private void BombDescriptionHandler(ShopItem item)
    {
        if (item._name != "Bomb") { return; }

        if (item.level <= 0)
            item.purchaseDescription = "Unlock Bomb Powerup.";
        else
        {
            if (!(item.level >= item.maxLevel))
                item.purchaseDescription = $"Increase bomb ammo to {item.itemValueAtLevel.Evaluate(item.level + 1)}.";
            else
                item.purchaseDescription = "Maximum bomb ammo capacity.";
        }
    }
    private void BombPurchaseHandler(ShopItem item)
    {
        if (item._name != "Bomb") { return; }

        var data = GetPowerupDataWithName("Bomb");
        data.maxAmmo = Mathf.CeilToInt(item.itemValueAtLevel.Evaluate(item.level));
    }
    private void InvincibilityDescriptionHandler(ShopItem item)
    {
        if (item._name != "Invincibility") { return; }

        if (item.level <= 0)
            item.purchaseDescription = "Unlock Invincibility Powerup.";
        else
        {
            if (!(item.level >= item.maxLevel))
                item.purchaseDescription = $"Increase Invincibility duration to {item.itemValueAtLevel.Evaluate(item.level + 1)}.";
            else
                item.purchaseDescription = "Maximum Invincibility duration capacity.";
        }
    }
    private void InvincibilityPurchaseHandler(ShopItem item)
    {
        if (item._name != "Invincibility") { return; }

        var data = GetPowerupDataWithName("Invincibility");
        data.duration = Mathf.CeilToInt(item.itemValueAtLevel.Evaluate(item.level));
    }
    private void ProjectileDescriptionHandler(ShopItem item)
    {
        if (item._name != "Projectile") { return; }

        if (item.level <= 0)
            item.purchaseDescription = "Unlock Projectile Powerup.";
        else
        {
            if (!(item.level >= item.maxLevel))
                item.purchaseDescription = $"Increase Projectile ammo to {item.itemValueAtLevel.Evaluate(item.level + 1)}.";
            else
                item.purchaseDescription = "Maximum Projectile ammo capacity.";
        }
    }
    private void ProjectilePurchaseHandler(ShopItem item)
    {
        if (item._name != "Projectile") { return; }

        var data = GetPowerupDataWithName("Projectile");
        data.maxAmmo = Mathf.CeilToInt(item.itemValueAtLevel.Evaluate(item.level));
    }
    #endregion

    private PowerupData GetPowerupDataWithName(string name)
    {
        for (int i = 0; i < powerupDatas.Length; i++)
        {
            if (powerupDatas[i]._name == name)
            {
                return powerupDatas[i];
            }
        }
        return null;
    }
    // Update is called once per frame
    void Update()
    {
        UpdateCurrencyText();
    }
    
    public void StartGame()
    {
        isGameOver = false;
        canStartGame = true;
        inGame = true;
        SetActivePlayerHealthIcons();
      //  FeedbackManager.instance.ResetMoveSpeedNormalized();
    }
    private void EndGame()
    {
        isGameOver = true;
    }
    public void GoToMenu()
    {
        canStartGame = false;
        inGame = false;
        isGameOver = false;
    }
    private void SetupHealth()
    {
        for (int i = 0; i < playerData.health; i++)
        {
            var healthBarGO = Instantiate(healthBar, healthSlots);
            healthBars.Add(healthBarGO);
        }
    }
    private void SetActivePlayerHealthIcons()
    {
        for (int i = 0; i < healthBars.Count; i++)
            healthBars[i].SetActive(true);
    }
    private void DisplayGameOverPopUp()
    {
        UIManager.instance.UpdatePopUp(1);
    }
    private void UpdateCurrencyText()
    {
        currencyText.text = currencyData.currency.ToString("N0");
    }
    public void AddCurrency(int currency)
    {
        currencyData.currency += currency;

        ShopManager.instance.UpdateShopButtons();
    }
    public void UseCurrency(int currency)
    {
        currencyData.currency -= currency;

        ShopManager.instance.UpdateShopButtons();
    }
    public int GetCurrency()
    {
        return currencyData.currency;
    }
    public void ResetGameScores()
    {
        ScoreSystem.GameScore.ResetScores();
    }
    public void GameStart()
    {
        OnGameStart?.Invoke();
    }
    public void GameEnd()
    {
        OnGameEnd?.Invoke();
    }
}
