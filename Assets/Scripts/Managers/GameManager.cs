using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance{get; private set;}
    public float plainWait, preWrongChainWait, preTimerWait, preHitDeathWait, preWinWait;

    private List<KeyDot> keyDots = new List<KeyDot>();
    private List<FalseDot> falseDots = new List<FalseDot>();
    private List<Vector2> keyDotPositions = new List<Vector2>();
    private List<Vector2> falseDotPositions = new List<Vector2>();

    private List<Generatable> generatables = new List<Generatable>();
    private List<int> keyDotI = new List<int>();
    private List<int> falseDotI = new List<int>();

    private bool skipTutorial;

    private int highScore 
    {
        get
        {
            return PlayerPrefs.GetInt("HighScore", 0);
        }
        set
        {
            PlayerPrefs.SetInt("HighScore", value);
        }
    }

    private int puzzleHighScore
    {
        get
        {
            return PlayerPrefs.GetInt("PuzzleHighScore", 0);
        }
        set
        {
            PlayerPrefs.SetInt("PuzzleHighScore", value);
        }
    }

    private void Awake() {
        Instance = this;   
    }

    private void Start() {
        UIManager.Instance.UpdateHighScore();
        UIManager.Instance.UpdatePuzzleHighScore();
        UIManager.Instance.SetupSkipTutorial();
        int result = PlayerPrefs.GetInt("SkipTutorial?", 0);
        skipTutorial = result == 0 ? false: true;
    }

    public void ToggleSkipTutorial()
    {
        skipTutorial = !skipTutorial;
        int result = skipTutorial ? 1: 0;
        PlayerPrefs.SetInt("SkipTutorial?", result);
    }

    public void InvokeDelayed(float time, DoubleVoid func)
    {
        StartCoroutine(DelayedExecution(time, func));
    }

    IEnumerator DelayedExecution(float time, DoubleVoid func)
    {
        yield return new WaitForSeconds(time);
        func();
    }

    public void AddKeyDot(KeyDot keyDot)
    {
        keyDots.Add(keyDot);
        keyDotPositions.Add(keyDot.transform.position);
    }

    public void AddFalseDot(FalseDot falseDot)
    {
        falseDots.Add(falseDot);
        falseDotPositions.Add(falseDot.transform.position);
    }

    public void AddGeneratable(Generatable generatable)
    {
        generatable.Index = generatables.Count;
        generatables.Add(generatable);
    }


    public void CheckForDotOverlap()
    {
        keyDotI = RayGraph.Instance.CheckForOverlap(keyDotPositions, false);
        falseDotI = RayGraph.Instance.CheckForOverlap(falseDotPositions, true);
        if(keyDotI.Count == 0 && falseDotI.Count == 0)
        {
            LevelReset();
        }
        else
        {
            GameEnd(GameOverType.WrongChain);
        }
    }

    public void GameStart(bool isPuzzle)
    {
        GameState.isPuzzle = isPuzzle;
        GameState.isGameOver = false;
        GameState.currentOffsetVector = GameState.totalOffsetVector = Vector2.zero;
        Camera.main.transform.position = Vector2.zero;
        GameState.SetProgKeys();
        if(skipTutorial)
        {
            GameState.LevelsCleared = 2;
            UIManager.Instance.UpdateLevelCounter(0);
        }
        else
        {
            GameState.LevelsCleared = 0;
        }
        ClearScene();
        RayGraph.Instance.Clear();
        GenerationManager.Instance.GenerateStart();
        UIManager.Instance.SwitchPanels();
        Player.Instance.NonControlable = false;
        BackGroundPlayer.Instance.EnableSmooth();
    }

    public void GameEnd(GameOverType type = GameOverType.Default)
    {
        if(!GameState.isGameOver)
            StartCoroutine(GameEnding(type));
        GameState.isGameOver = true;
    }

    IEnumerator GameEnding(GameOverType type)
    {
        Player.Instance.NonControlable = true;
        Timer.Instance.StopTimer();
        BackGroundPlayer.Instance.MuteSmooth();
        if(GameState.LevelsCleared > 2)
        {
            if(GameState.isPuzzle)
            {
                if(puzzleHighScore < GameState.LevelsCleared - 2)
                {
                    puzzleHighScore = GameState.LevelsCleared - 2;
                    UIManager.Instance.UpdatePuzzleHighScore();
                }
            }
            else if(highScore < GameState.LevelsCleared - 2)
            {
                highScore = GameState.LevelsCleared - 2;
                UIManager.Instance.UpdateHighScore();
            }
        }
        switch(type)
        {
            case GameOverType.WrongChain:
                for(int i = 0; i < keyDotI.Count; i++)
                    keyDots[keyDotI[i]].Outline();
                for(int k = 0; k < falseDotI.Count; k++)
                    falseDots[falseDotI[k]].Outline();
                yield return new WaitForSeconds(preWrongChainWait);
                Player.Instance.Disable();
                goto default;
            case GameOverType.Timer:
                Vector2 position;
                for(int i = 0; i < 30; i++)
                {
                    int count = Random.Range(1,4);
                    for(int k = 0; k < count; k++)
                    {
                        position = CustomRandom.inPlayzone;
                        GenerationManager.Instance.GenerateVFX(VFXType.TimerDeathEffect, position);
                    }
                    yield return new WaitForSeconds(Random.Range(0f, 0.1f));
                }
                yield return new WaitForSeconds(preTimerWait);
                Player.Instance.Disable();
                goto default;
            case GameOverType.Hit:
                GenerationManager.Instance.GenerateVFX(VFXType.HitGameOver, Player.Instance.transform.position);
                Player.Instance.gameObject.SetActive(false);
                yield return new WaitForSeconds(preHitDeathWait);
                goto default;
            case GameOverType.Victory:
                HintManager.Instance.ShowHint(2);
                GenerationManager.Instance.GenerateVFX(VFXType.HitGameOver, Player.Instance.transform.position);
                Player.Instance.gameObject.SetActive(false);
                yield return new WaitForSeconds(preWinWait);
                goto default;
            default:
                ClearScene();
                UIManager.Instance.HUDDisappear();
                yield return new WaitForSeconds(plainWait);
                UIManager.Instance.SwitchPanels();
                break;
        }
    }
    public void LevelReset()
    {
        if(GameState.isGameOver)
            return;
        GameState.LevelsCleared++;
        ClearScene();
        RayGraph.Instance.Clear();
        GenerationManager.Instance.CalculateLevel();
        GenerationManager.Instance.DisplayLevel();
        CameraController.Instance.Shift(GameState.totalOffsetVector);
    }

    public void ClearScene()
    {
        int length = generatables.Count;
        for (int i = 0; i < length; i++)
        {
            generatables[i].Dispose();
        }
        for(int i = 0; i < keyDots.Count; i++)
        {
            keyDots[i].Dispose();
        }
        for(int i = 0; i < falseDots.Count; i++)
        {
            falseDots[i].Dispose();
        }
        generatables.Clear();
        keyDots.Clear();
        falseDots.Clear();
        keyDotPositions.Clear();
        falseDotPositions.Clear();
    }

    
    public int numberToSkip = 1;

    #if UNITY_EDITOR
    private void OnGUI() {
        if(GUI.Button(new Rect(0f, 0f, 100f, 50f), "Skip"))
        {
            for(int i = 0; i < numberToSkip; i++)
            {
                LevelReset();
            }
        }
        if(GUI.Button(new Rect(120f, 0f, 100f, 50f), "GameEnd"))
        {
            Player.Instance.Disable();
            GameEnd();
        }
        if(GUI.Button(new Rect(240f, 0f, 100f, 50f), "RP"))
        {
            highScore = 0;
            puzzleHighScore = 0;
            UIManager.Instance.UpdateHighScore();
            UIManager.Instance.UpdatePuzzleHighScore();
        }
    }
    #endif
    
}
