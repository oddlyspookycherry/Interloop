using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public static UIManager Instance{get; private set;}
    public GameObject startPanel;

    public GameObject HUDPanel;

    [Header("Timer")]
    public Slider timerSlider; 
    public Image timerFill;
    public Gradient timerGradient;

    [Space(10f)]
    public Text levelCounter;
    public Text highScore;
    public Text puzzleHighScore;
    public Toggle skipTutorialToggle;

    private void Awake() {
        Instance = this;
    }

    public void SwitchPanels()
    {
        startPanel.SetActive(!startPanel.activeSelf);
        HUDPanel.SetActive(!HUDPanel.activeSelf);
    }

    public void UpdateHighScore()
    {
        int score = PlayerPrefs.GetInt("HighScore", 0);
        highScore.text = score.ToString();
    }

    public void UpdatePuzzleHighScore()
    {
        int score = PlayerPrefs.GetInt("PuzzleHighScore", 0);
        puzzleHighScore.text = score.ToString() + "/30";
    }

    public void SetupSkipTutorial()
    {
        int result = PlayerPrefs.GetInt("SkipTutorial?", 0);
        skipTutorialToggle.isOn = result == 0 ? false: true;
    }

    public void HUDDisappear()
    {
        HUDPanel.GetComponent<Animation>().Play("HUDDisappear", PlayMode.StopAll);
    }

    public void UpdateLevelCounter(int newValue)
    {
        levelCounter.text = newValue.ToString();
    }

    public void UpdateTimer(float newValue)
    {
        timerFill.color = timerGradient.Evaluate(newValue);
        timerSlider.value = newValue;
    }
}