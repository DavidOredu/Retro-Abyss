using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum PlayerControllerType
{
    Touch,
    Keyboard,
}
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    public PlayerControllerType controllerType;
    private PowerupController powerupController;
    public PowerupButton powerupButton { get; private set; }
    private PlayerData playerData;

    [SerializeField]
    private GameObject projectilePrefab;

    [SerializeField]
    private Transform projectileSpawnPoint;

    [SerializeField]
    private float moveSpeed = 20f;

    public TriggerSquare currentTriggerSquare { get; set; } = null;

    public int health { get;private set; }

    private Vector2 moveInput;
    private GameObject shotTarget;
    private Vector3 shootDirection;
    private Quaternion rotation;

    private bool canTakeInput;
    private bool canShoot;
    public bool hasShot;
    public bool justTookDamage;
    public bool projectileActive;
    public bool isClickingPowerupButton;
    public bool isInvulnerable { get; set; }
    public bool isInvincible { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        powerupController = FindObjectOfType<PowerupController>();
        powerupButton = GameManager.instance.PowerupButton.GetComponent<PowerupButton>();
        playerData = Resources.Load<PlayerData>("PlayerData");

        health = playerData.health;
        canTakeInput = true;
    }

    // Update is called once per frame
    void Update()
    {
        GetMoveInput();

        RotateFirePoint();

        if (Input.GetButtonUp("Fire1") && canShoot)
        {
            Shoot();
            canTakeInput = true;
        }
    }
    private void FixedUpdate()
    {
        Move();
    }
    private void GetMoveInput()
    {
        if (!GameManager.instance.inGame || GameManager.instance.isGameOver || isClickingPowerupButton || !canTakeInput /* projectileActive */ ) { return; }

        if (controllerType == PlayerControllerType.Keyboard)
        {
            moveInput.x = Input.GetAxis("Horizontal");
            moveInput.y = Input.GetAxis("Vertical");
        }
        else if (controllerType == PlayerControllerType.Touch)
        {
            if (Input.GetMouseButton(0))
            {
                Vector2 mousePos = Input.mousePosition;
                Vector2 centrePosition = new Vector2(Screen.width / 2, Screen.height / 2);
                moveInput = (mousePos - centrePosition).normalized;
            }
            else
            {
                moveInput = Vector2.zero;
            }
        }
    }
    
    private void Move()
    {
        controller.Move(new Vector3(moveInput.x * moveSpeed * Time.deltaTime, 0f, moveInput.y * moveSpeed * Time.deltaTime));
    }
    private void Shoot()
    {
        var projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        projectile.GetComponent<ProjectileMover>().shotTarget = shotTarget;
        hasShot = true;
    }
    private void RotateFirePoint()
    {
        RaycastHit hit;
        var mousePos = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(mousePos.origin, mousePos.direction, out hit, 100f, LayerMask.GetMask("TriggerSquare")) && projectileActive)
        {
            RotateToMouseDirection(projectileSpawnPoint.gameObject, hit.point);
            shotTarget = hit.collider.gameObject;
            moveInput = Vector2.zero;
            canTakeInput = false;
            canShoot = true;
        }
        else
        {
            canTakeInput = true;
            canShoot = false;
        }

    }
    private void RotateToMouseDirection(GameObject obj, Vector3 destination)
    {
        shootDirection = (destination - obj.transform.position);
        rotation = Quaternion.LookRotation(shootDirection);
        obj.transform.localRotation = rotation;
    }
    public void TakeDamage()
    {
        if (isInvulnerable)
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
                justTookDamage = true;
            return;
        }

        health--;
        health = Mathf.Max(0, health);
        GameManager.instance.healthBars[health].SetActive(false);
        // Display Damage Effect

        if(health <= 0)
        {
            // Game Over
            GameManager.instance.GameEnd();
        }
    }
    public void IncreaseHealth(int amount)
    {
        health += amount;
        health = Mathf.Min(health, playerData.health);
        GameManager.instance.healthBars[health - 1].SetActive(true);
    }
}
