using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreSystem
{
    public static class GameScore
    {
        public static int currentScoreMultiplier = 1;
        public static int maxMultiplier = 2;

        public static Dictionary<string, int> scores = new Dictionary<string, int>
        {
            { ConstNames.DISTANCE_COVERED, 0 },
            { ConstNames.NORMAL_TILES_HIT, 0 },
            { ConstNames.OBSTACLE_TILES_HIT, 0 },
            { ConstNames.POWERUP_TILES_HIT, 0 },
            { ConstNames.SLOWDOWN_TILES_HIT, 0 },
            { ConstNames.CURRENCY_TILES_HIT, 0 },
            { ConstNames.MAIN_SCORE, 0 },
        };

        public static void ResetScores()
        {
            List<string> keys = new List<string>();

            foreach (var score in scores)
            {
                var key = score.Key;
                keys.Add(key);
            }

            for (int i = 0; i < scores.Count; i++)
            {
                scores[keys[i]] = 0;
            }

            currentScoreMultiplier = 1;
        }
    }
}
