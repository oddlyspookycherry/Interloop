using UnityEngine;
public static class GameState
{

    #region  General Settings
    private static int levelsCleared;
    private const int edgeLevel = 29;
    public static float Interpolant
    {
        get
        {
            if(levelsCleared < 3)
                return 0f;
            float result = (float)(levelsCleared - 2) / edgeLevel;
            return Mathf.Clamp01(result);
        }
    }
    public static IntRange rows { get; private set; }
    public static IntRange columns { get; private set; }

    public static IntRange dotCount { get; private set; }
    public static float falseDotPercent { get; private set; }

    public static IntRange obstaclesCount { get; private set; }

    public static float time { get; private set; }
    public static float customLevelProbability { get; private set; }
    #endregion

    #region  Rogue Settings
    public static FloatRange rogueSpeed { get; private set; }
    public static IntRange rogueCount { get; private set; }
    #endregion 

    public static bool isPuzzle {get; set;}

    private static Vector2 _currentOffsetVector;
    public static Vector2 totalOffsetVector { get; set; }
    public static Vector2 currentOffsetVector
    {
        get
        {
            return _currentOffsetVector;
        }
        set
        {
            totalOffsetVector += value;
            _currentOffsetVector = value;
        }
    }

    public static bool isGameOver{get; set;}

    private static int[] t = new int[8];
    public static int LevelsCleared
    {
        get
        {
            return levelsCleared;
        }
        set
        {
            if(value == 0)
                UIManager.Instance.UpdateLevelCounter(value);
            else if(value > 2)
                UIManager.Instance.UpdateLevelCounter(value - 2);
            levelsCleared = value;
            AdjustConfig();
        }
    }

    public static void SetProgKeys()
    {
        t[0] = 0;
        t[1] = 1;
        t[2] = 2;
        t[3] = Random.Range(4, 7);
        t[4] = Random.Range(10, 13);
        t[5] = Random.Range(16, 18);
        t[6] = Random.Range(21, 23);
        t[7] = Random.Range(24, 27);
    }

    private static void AdjustConfig()
    {
        if (levelsCleared == t[0])
        {
            customLevelProbability = 0f;
            rows = 2;
            columns = 2;

            dotCount = 1;
            falseDotPercent = 0f;
            obstaclesCount = 0;
            rogueCount = 0;
            time = 45f;

            HintManager.Instance.ShowHint(0);
            return;
        }
        if (levelsCleared == t[1])
        {
            rows = 3;
            columns = 2;

            dotCount = 3;
            falseDotPercent = 0.5f;

            time = 30f;
            HintManager.Instance.ShowHint(1);
            return;
        }
        if (levelsCleared == t[2])
        {
            if(isPuzzle == true)
                customLevelProbability = 1f;
            else
                customLevelProbability = 0f;
            rows = 3;
            columns = 3;

            dotCount = new IntRange(6, 7);
            falseDotPercent = 0.4f;
            obstaclesCount = 0;
            rogueCount = 0;

            time = 11f;
            return;
        }

        if(isPuzzle)
            return;

        if (levelsCleared == t[3])
        {
            customLevelProbability = 0.05f;
            columns = 4;

            dotCount = new IntRange(9, 11);

            time = 9.2f;
            return;
        }
        if (levelsCleared == t[4])
        {
            customLevelProbability = 0.1f;
            rows = 4;

            dotCount = new IntRange(15, 17);

            rogueCount = 1;
            rogueSpeed = new FloatRange(5f, 6.5f);
            falseDotPercent = 0.25f;
        
            time = 13.7f;
            return;
        }
        if (levelsCleared == t[5])
        {
            customLevelProbability = 0.15f;

            falseDotPercent = 0.3f;
            rogueCount = 2;
            rogueSpeed = new FloatRange(6f, 7f);

            time = 14.5f;
            return;
        }
        if (levelsCleared == t[6])
        {
            customLevelProbability = 0.3f;

            columns = 5;

            dotCount = new IntRange(17, 22);

            rogueCount = 3;
            rogueSpeed = new FloatRange(5f, 6f);
            falseDotPercent = 0.35f;

            time = 15.5f;
            return;
        }
        if (levelsCleared == t[7])
        {
            rogueCount = 4;
            rogueSpeed = new FloatRange(5.5f, 6.5f);
            falseDotPercent = 0.4f;
            time = 16.2f;
            return;
        }
    }

}
