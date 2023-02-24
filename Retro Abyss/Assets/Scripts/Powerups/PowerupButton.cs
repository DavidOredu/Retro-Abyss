using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class PowerupButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public PowerupBehaviour powerupBehaviour;
    private PlayerController player;
    private WorldController worldController;

    [SerializeField]
    private Button button;
    [SerializeField]
    private TextMeshProUGUI powerupAmmoText;

    private bool isSelected;
    // Start is called before the first frame update
    void Start()
    {
        ResolveVariables();
        button = GameManager.instance.PowerupButton;

        button.image.enabled = false;
        powerupAmmoText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        ResolveVariables();
        DisableButtonOnCondition();
        UpdateSelectablePowerupActivity();
        UpdatePowerupAmmoText();
        TurnOffPowerup();
    }
    private void ResolveVariables()
    {
        if (player == null)
            player = FindObjectOfType<PlayerController>();
        if(worldController == null)
            worldController = FindObjectOfType<WorldController>();
    }
    private void UpdateSelectablePowerupActivity()
    {
        if(powerupBehaviour == null) { return;}

        if (powerupBehaviour.powerup.isSelectable)
        {
            if (isSelected)
            {
                powerupBehaviour.powerup.SelectedActive();
            }
        }
    }
    private void DisableButtonOnCondition()
    {
        if(powerupBehaviour == null) { return; }

        switch (powerupBehaviour.powerup.name)
        {
            case "SoundCannon":
                if (worldController.AllSquaresDestroyed() || !worldController.HasTriggerSquareType(SquareType.Obstacle))
                    button.interactable = false;
                else
                    button.interactable = true;
                break;
            default:
                button.interactable = true;
                break;
        }
    }
    public void SetupButton(PowerupBehaviour powerupBehaviour)
    {
        if(this.powerupBehaviour != null && !powerupBehaviour.powerup.isInstant)
        {
            if (this.powerupBehaviour.powerup.isSelectable)
            {
                this.powerupBehaviour.powerup.value = 0;
                DeselectPowerup();
            }

            this.powerupBehaviour.powerup.powerupAmmo = 0;
        }

        if (!powerupBehaviour.powerup.isInstant)
        {
            this.powerupBehaviour = powerupBehaviour;
            button.image.sprite = powerupBehaviour.powerup.powerupIcon;
            button.image.enabled = true;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => ActivatePowerup());
            FeedbackManager.instance.feedbacksDict[ConstNames.POWERUP_BUTTON_FEEDBACK].PlayFeedbacks();
        }
        else
        {
            powerupBehaviour.ActivatePowerup();
        }
    }
    private void ActivatePowerup()
    {
        if (powerupBehaviour == null) { return; }
        if (powerupBehaviour.powerup.powerupAmmo == 0) { return; }

        if (powerupBehaviour.powerup.isSelectable)
        {
            if(!powerupBehaviour.powerupController.activePowerups.ContainsKey(powerupBehaviour.powerup.name))
                powerupBehaviour.ActivatePowerup();

            if (!isSelected)
            {
                SelectPowerup();
            }
            else
            {
                DeselectPowerup();
            }
        }
        else
        {
            powerupBehaviour.ActivatePowerup();
            powerupBehaviour.powerup.powerupAmmo--;
        }

        
            
        
    }
    public void SelectPowerup()
    {
        powerupBehaviour.powerup.SelectedStart();
        isSelected = true;
    }
    public void DeselectPowerup()
    {
        powerupBehaviour.powerup.SelectedEnd();
        isSelected = false;
    }
    private void UpdatePowerupAmmoText()
    {
        if(powerupBehaviour == null) { return; }

        if(powerupAmmoText == null)
        {
            powerupAmmoText = GameManager.instance.powerupAmmoText;
        }

        if(powerupBehaviour.powerup.powerupAmmo <= 1)
        {
            powerupAmmoText.gameObject.SetActive(false);
        }
        else
        {
            powerupAmmoText.gameObject.SetActive(true);
            powerupAmmoText.text = powerupBehaviour.powerup.powerupAmmo.ToString();
        }
    }
    private void TurnOffPowerup()
    {
        if(powerupBehaviour == null) { return; }

        if (powerupBehaviour.powerup.powerupAmmo <= 0)
        {
            button.image.enabled = false;
            powerupBehaviour = null;
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        player.isClickingPowerupButton = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        player.isClickingPowerupButton = false;
    }
}
