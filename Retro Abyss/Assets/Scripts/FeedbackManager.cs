using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class FeedbackManager : SingletonDontDestroy<FeedbackManager>
{
    public List<MMFeedbacks> feedbacks = new List<MMFeedbacks>();
    public Dictionary<string, MMFeedbacks> feedbacksDict = new Dictionary<string, MMFeedbacks>();
    private string[] feedbackNames = new string[]
    {
        ConstNames.COLLISION_FEEDBACK,
        ConstNames.DAMAGE_FEEDBACK,
        ConstNames.POWERUP_BUTTON_FEEDBACK,
        ConstNames.PAUSE_FEEDBACK,
        ConstNames.UNPAUSE_FEEDBACK,
        ConstNames.INVINCIBILITY_END_FEEDBACK,
        ConstNames.SCORE_TEXT_FEEDBACK,
    };

    private WorldController worldController;
    private PlayerController playerController;
  //  public float effectAcceleration;

    [Space]
    [Header("CAMERA SHAKING")]
    public CinemachineVirtualCamera cmVcam;
    public CinemachineBasicMultiChannelPerlin camNoise;
    public NoiseSettings normalShake;
    public NoiseSettings extremeShake;
    public float lowAmplitudeGain = .5f;
    public float maxAmplitudeGain = 2.5f;

    [Header("SPEED EFFECTS")]
    public float maxSpeed = 7500;
    public float maxMoveNormalizedValue = 1f;
    public float maxExtremeMoveNormalizedValue = 1.2f;
    public float maxLensDistortion = .6f;
    private float moveSpeedNormalized;
    private float defaultChromaticAberrationIntensity;
    private float defaultLensDistortionIntensity;

    [Header("SLOW DOWN FROST")]
    public float maxFrostAmount = 0.5f;
    public float frostFormSpeed = .2f;
    public float frostMeltSpeed = 1f;
    public float frostLifetime = 3f;
    private bool frostOnScreen;
    private Timer frostLifetimeTimer;

    [Header("DAMAGE")]
    public float vignetteDamageLerpTime = .4f;
    public float vignetteNormalLerpTime = 1f;
    private bool damaging;

    private void Start()
    {
        for (int i = 0; i < feedbacks.Count; i++)
        {
            feedbacks[i].Initialization();
            feedbacksDict.Add(feedbackNames[i], feedbacks[i]);
        }

        playerController = FindObjectOfType<PlayerController>();
        defaultChromaticAberrationIntensity = PostProcessingHandler.instance.chromaticAberration.intensity.value;
        defaultLensDistortionIntensity = PostProcessingHandler.instance.lensDistortion.intensity.value;

        frostLifetimeTimer = new Timer(frostLifetime);
        frostLifetimeTimer.SetTimer();
        frostLifetimeTimer.StopTimer();
    }
    private void Update()
    {
        SetupCinemachine();
        SetCurrentMoveSpeed();
        HandleCameraShake();
        SetSpeedEffects();
        SetFrost();
        SetVignetteRed();
        SetVignetteNormal();
    }

    #region CAMERA SHAKING
    public void SetupCinemachine()
    {
        if (cmVcam == null)
        {
            cmVcam = FindObjectOfType<CinemachineVirtualCamera>();
            camNoise = cmVcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }

        if (!GameManager.instance.inGame)
        {
            camNoise.m_NoiseProfile = null;
        }
    }
    private void HandleCameraShake()
    {
        if (GameManager.instance.inGame)
        {
            if (!playerController.isInvincible)
            {
                camNoise.m_NoiseProfile = normalShake;
                camNoise.m_AmplitudeGain = Remap(moveSpeedNormalized, 0, 1, lowAmplitudeGain, maxAmplitudeGain);
            }
            else
            {
                camNoise.m_NoiseProfile = extremeShake;
                camNoise.m_AmplitudeGain = 1.5f;
            }
        }
        else
        {
            camNoise.m_AmplitudeGain = 1f;
        }
    }
    #endregion

    #region SPEED
    private void SetCurrentMoveSpeed()
    {
        worldController = FindObjectOfType<WorldController>();
        if (GameManager.instance.inGame)
        {
            //if (!GameManager.instance.isGameOver)
            //    moveSpeedNormalized += effectAcceleration;
            //else
            //    moveSpeedNormalized -= effectAcceleration;
            if(!playerController.isInvincible)
                moveSpeedNormalized = worldController._moveSpeed / maxSpeed;
        }

        if (playerController == null)
            playerController = FindObjectOfType<PlayerController>();
        if (!playerController.isInvincible)
            moveSpeedNormalized = Mathf.Min(moveSpeedNormalized, maxMoveNormalizedValue);
        else
        {
         //   moveSpeedNormalized = Mathf.Min(moveSpeedNormalized, maxExtremeMoveNormalizedValue);
            moveSpeedNormalized = Mathf.Lerp(moveSpeedNormalized, maxExtremeMoveNormalizedValue, Time.deltaTime);
        }

     //   moveSpeedNormalized = Mathf.Max(moveSpeedNormalized, 0);
    }
    
    private void SetSpeedEffects()
    {
        if (GameManager.instance.inGame)
        {
            PostProcessingHandler.instance.chromaticAberration.intensity.Override(moveSpeedNormalized);
            PostProcessingHandler.instance.lensDistortion.intensity.Override(-Remap(moveSpeedNormalized, 1f, maxLensDistortion));
        }
        else
        {
            PostProcessingHandler.instance.chromaticAberration.intensity.Override(defaultChromaticAberrationIntensity);
            PostProcessingHandler.instance.lensDistortion.intensity.Override(defaultLensDistortionIntensity);
        }
    }
    public void ResetMoveSpeedNormalized()
    {
        moveSpeedNormalized = 0; 
    }
    #endregion

    #region FROST
    [ContextMenu("StartFrost")]
    public void StartScreenFrost()
    {
        frostOnScreen = true;
        frostLifetimeTimer.StopTimer();
    }
    public void SetFrost()
    {
        if (frostOnScreen)
        {
            if (!frostLifetimeTimer.timerStarted)
            {
                frostLifetimeTimer.ResetTimer();
            }
            else
            {
                if (frostLifetimeTimer.isTimeUp)
                {
                    PostProcessingHandler.instance.VignetteIntensity = Mathf.Lerp(PostProcessingHandler.instance.VignetteIntensity, 0, frostMeltSpeed);
                    if (PostProcessingHandler.instance.VignetteIntensity <= 0.05f)
                    {
                        frostLifetimeTimer.StopTimer();
                        frostOnScreen = false;
                    }
                }
                else
                {
                    frostLifetimeTimer.UpdateTimer();
                    PostProcessingHandler.instance.VignetteIntensity = Mathf.Lerp(PostProcessingHandler.instance.VignetteIntensity, maxFrostAmount, frostFormSpeed);
                }
            }
        }
    }
    #endregion

    #region DAMAGE
    public void SetVignetteRed()
    {
        if(damaging)
            PostProcessingHandler.instance.vignette.color.Interp(PostProcessingHandler.instance.vignette.color.value, Color.red, vignetteDamageLerpTime);
    }
    public void SetVignetteNormal()
    {
        if (!damaging)
        {
            if(PostProcessingHandler.instance.vignette.color.value != Color.black)
                PostProcessingHandler.instance.vignette.color.Interp(PostProcessingHandler.instance.vignette.color.value, Color.black, vignetteNormalLerpTime);
        }
    }
    public void StartDamageFeedback() => damaging = true;
    public void EndDamageFeedback() => damaging = false;
    #endregion

    private float Remap(float initialFirstValue, float finalFirstValue, float finalRemapValue)
    {
        return (initialFirstValue * finalRemapValue) / finalFirstValue;
    }
    private float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}
