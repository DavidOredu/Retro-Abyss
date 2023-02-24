using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MoreMountains.Feedbacks;

public class ScoreUI : MonoBehaviour
{
    [Header("IN-GAME UI")]
    public TextMeshProUGUI mainScoreText;
    public TextMeshProUGUI distanceCoveredText;
    //  public TextMeshProUGUI currentScoreMultiplierText;

    [Header("GAME OVER POPUP")]
    public TextMeshProUGUI whiteTilesText;
    public TextMeshProUGUI redTilesText;
    public TextMeshProUGUI greenTilesText;
    public TextMeshProUGUI blueTilesText;
    public TextMeshProUGUI yellowTilesText;
    public TextMeshProUGUI allTilesText;
    public TextMeshProUGUI distanceText;
    public TextMeshProUGUI diamondsEarnedText;

    public MMFeedbacks scoreChangeFeedback;
    public MMFeedbacks multiplierChangeFeedback;
    public MMFeedbacks newHighScoreFeedback;
    public MMFeedbacks newMaxKillsFeedback;
    public void PlayCollsionFeedback()
    {
        scoreChangeFeedback.PlayFeedbacks();
        multiplierChangeFeedback.PlayFeedbacks();
    }

    public void PlayDamageFeedback()
    {
       
    }
    void Awake()
    {
    //    GameManager.OnGameStart += SetMultiplierTextColor;
    }
    // Start is called before the first frame update
    void Start()
    {
        GameManager.OnGameEnd += SetGameOverStats;
    }
    private void SetMultiplierTextColor()
    {
      //  currentScoreMultiplierText.color = Resources.Load<PlayerData>("PlayerData").color;
    }
    // Update is called once per frame
    void Update()
    {
        mainScoreText.text = ScoreSystem.GameScore.scores[ConstNames.MAIN_SCORE].ToString("N0");
        distanceCoveredText.text = ScoreSystem.GameScore.scores[ConstNames.DISTANCE_COVERED].ToString("N0");

        //currentScoreMultiplierText.gameObject.SetActive(ScoreSystem.GameScore.currentScoreMultiplier > 1);
        //currentScoreMultiplierText.text = "X" + ScoreSystem.GameScore.currentScoreMultiplier.ToString();
    }
    private void SetGameOverStats()
    {
        whiteTilesText.text = ScoreSystem.GameScore.scores[ConstNames.NORMAL_TILES_HIT].ToString("N0");
        redTilesText.text = ScoreSystem.GameScore.scores[ConstNames.OBSTACLE_TILES_HIT].ToString("N0");
        greenTilesText.text = ScoreSystem.GameScore.scores[ConstNames.POWERUP_TILES_HIT].ToString("N0");
        blueTilesText.text = ScoreSystem.GameScore.scores[ConstNames.SLOWDOWN_TILES_HIT].ToString("N0");
        yellowTilesText.text = ScoreSystem.GameScore.scores[ConstNames.CURRENCY_TILES_HIT].ToString("N0");
        allTilesText.text = ScoreSystem.GameScore.scores[ConstNames.MAIN_SCORE].ToString("N0");
        distanceText.text = ScoreSystem.GameScore.scores[ConstNames.DISTANCE_COVERED].ToString("N0");
    }

    
}
