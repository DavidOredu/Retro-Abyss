using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WorldController : MonoBehaviour
{
    private Rigidbody rb;
    private WorldData worldData;

    [Header("MOTION")]
    [SerializeField]
    private float gameMoveSpeed = 50f;
    [SerializeField]
    private float idleMoveSpeed = 100f;
    [SerializeField]
    private float acceleration = 1f;
    [SerializeField]
    private float endDeceleration = 10f;

    [Header("SPAWNING")]
    [SerializeField]
    private float borderSpawnOffset;
    [SerializeField]
    private float squareSpawnOffset;

    [SerializeField]
    private Border[] borders = new Border[4];
    [SerializeField]
    public TriggerSquare[] triggerSquares = new TriggerSquare[4];

    private SquareType[] squareTypes = new SquareType[] { SquareType.Normal, SquareType.Obstacle, SquareType.Powerup, SquareType.SlowDown, SquareType.Currency };

    private GameObject player;

    public float _moveSpeed { get; private set; }

    private bool hasSetSquare;

    public bool canAccelerate = true;
    public bool inGameControl = false;

    private Probability<SquareType> squareTypeProbability;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        worldData = Resources.Load<WorldData>("WorldData");
        player = GameObject.FindGameObjectWithTag("Player");
        var probability = Resources.Load<ProbabilityData>("SquareTypeProbabilityData");
        squareTypeProbability = new Probability<SquareType>(probability.probabilityCurve, squareTypes.ToList());
        _moveSpeed = idleMoveSpeed;

        acceleration = worldData.gravity;
        squareSpawnOffset = worldData.squareDistance;
        FeedbackManager.instance.maxAmplitudeGain = worldData.maxSpeedNoise;

        ToggleTriggerSquares(false);

        if(GameManager.instance.canStartGame)
            StartGameFall();
    }
    private void Update()
    {
        SpawnNewBorder();

        if (inGameControl)
        {
            SpawnNewTriggerSquare();
            DetermineSquareType();
        }

        if (GameManager.instance.isGameOver && GameManager.instance.inGame)
        {
            DestroyAllTriggerSquares();
            inGameControl = false;
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        Fall();
    }
    private void StartGameFall()
    {
        _moveSpeed = gameMoveSpeed;
        inGameControl = true;
        ToggleTriggerSquares(true);
        GameManager.instance.canStartGame = false;
    }
    private void Fall()
    {
        rb.velocity = new Vector3(0, _moveSpeed * Time.deltaTime, 0);

        if (!GameManager.instance.isGameOver)
        {
            if (inGameControl && canAccelerate)
                _moveSpeed += acceleration;           
            ScoreSystem.GameScore.scores[ConstNames.DISTANCE_COVERED] = Mathf.CeilToInt(transform.position.y);
        }
        else
        {
            _moveSpeed -= endDeceleration;
            _moveSpeed = Mathf.Max(idleMoveSpeed, _moveSpeed);
        }
    }
    public void IncreaseMoveSpeed(float increasePercentage)
    {
        _moveSpeed += _moveSpeed * increasePercentage;
     //   FeedbackManager.instance.moveSpeedNormalized = FeedbackManager.instance.moveSpeedNormalized * increasePercentage;
    }
    public void ReduceMoveSpeed(float reductionPercentage)
    {
        _moveSpeed -= _moveSpeed * reductionPercentage;
     //   FeedbackManager.instance.moveSpeedNormalized = FeedbackManager.instance.moveSpeedNormalized * reductionPercentage;
    }
    private void SpawnNewBorder()
    {
        if (borders[0].transform.position.y > player.transform.position.y)
        {
            for (int i = 0; i < borders.Length; i++)
            {
                var bPosition = borders[i].transform.position;
                var bRotation = borders[i].transform.rotation;
                var newBorder = ObjectPooler.instance.SpawnFromPool(ObjectPooler.PoolTag.Border, new Vector3(bPosition.x, bPosition.y + borderSpawnOffset, bPosition.z), bRotation, transform);

                borders[i] = newBorder.GetComponent<Border>();
            }
        }
    }
    private void SpawnNewTriggerSquare()
    {
        if (triggerSquares[0].transform.position.y > player.transform.position.y)
        {
            for (int i = 0; i < triggerSquares.Length; i++)
            {
                var bPosition = triggerSquares[i].transform.position;
                var bRotation = triggerSquares[i].transform.rotation;
                var newSquare = ObjectPooler.instance.SpawnFromPool(ObjectPooler.PoolTag.TriggerSquare, new Vector3(bPosition.x, bPosition.y + squareSpawnOffset, bPosition.z), bRotation, transform);

                if (triggerSquares[i].gameObject.activeSelf)
                {
                    triggerSquares[i].powerupBehaviour.powerup.powerupAmmo = 0;
                    triggerSquares[i].gameObject.SetActive(false);
                }

                TriggerSquare triggerSquareScript = newSquare.GetComponent<TriggerSquare>();

                // while
                while (triggerSquareScript.powerupBehaviour.powerup.powerupAmmo > 0 || triggerSquareScript.powerupBehaviour.powerupController.powerupIdentifier.ContainsValue(triggerSquareScript.powerupBehaviour.powerup))
                {
                    triggerSquareScript.gameObject.SetActive(false);
                    newSquare = ObjectPooler.instance.SpawnFromPool(ObjectPooler.PoolTag.TriggerSquare, new Vector3(bPosition.x, bPosition.y + squareSpawnOffset, bPosition.z), bRotation, transform);
                    triggerSquareScript = newSquare.GetComponent<TriggerSquare>();
                }
                triggerSquares[i] = triggerSquareScript;
            }
            hasSetSquare = false;
        }
    }
    private void DetermineSquareType()
    {
        if (hasSetSquare) { return; }

        var firstSquare = triggerSquares[Random.Range(0, triggerSquares.Length)];
        firstSquare.SetSquareType(SquareType.Normal);

        for (int i = 0; i < triggerSquares.Length; i++)
        {
            if (triggerSquares[i] == firstSquare) { continue; }

            bool hasGottenValidSquare = false;

            while (!hasGottenValidSquare)
            {
                var squareType = squareTypeProbability.ProbabilityGenerator();

                if (ShopManager.instance.CheckShopItemWithNameExists(squareType.ToString()))
                {
                    if (ShopManager.instance.GetShopItemWithName(squareType.ToString()).isUnlocked)
                    {
                        triggerSquares[i].SetSquareType(squareType);
                        hasGottenValidSquare = true;
                    }
                    else
                        hasGottenValidSquare = false;
                }
                else
                {
                    triggerSquares[i].SetSquareType(squareType);
                    hasGottenValidSquare = true;
                }
            }
        }
        hasSetSquare = true;
    }
    public bool HasTriggerSquare(TriggerSquare triggerSquare)
    {
        for (int i = 0; i < triggerSquares.Length; i++)
        {
            if(triggerSquares[i] == triggerSquare) { return true; }
        }
        return false;
    }
    public bool HasTriggerSquareType(SquareType squareType)
    {
        for (int i = 0; i < triggerSquares.Length; i++)
        {
            if(triggerSquares[i].squareType == squareType) { return true; }
        }
        return false;
    }
    public bool AllSquaresDestroyed()
    {
        for (int i = 0; i < triggerSquares.Length; i++)
        {
            if (!triggerSquares[i].isDestroyed) { return false; }
        }
        return true;
    }
    private void ToggleTriggerSquares(bool activate)
    {
        for (int i = 0; i < triggerSquares.Length; i++)
        {
            triggerSquares[i].gameObject.SetActive(activate);
        }
    }
    private void DestroyAllTriggerSquares()
    {
        for (int i = 0; i < triggerSquares.Length; i++)
        {
            if(!triggerSquares[i].isDestroyed)
                triggerSquares[i].DestroyTriggerSquare();
        }
    }
}
