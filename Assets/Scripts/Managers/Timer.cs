using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour {

    public static Timer Instance{get; private set;}
    private bool isWorking = false;

    private IEnumerator timerCoroutine;

    private void Awake() {
        Instance = this;
    }
    public void SetTimer(float time)
    {
        if(isWorking)
        {
            StopTimer();
        }
        timerCoroutine = TimerCoroutine(time);
        StartCoroutine(timerCoroutine);
    }

    public void StopTimer()
    {
        if(isWorking)
        {
            StopCoroutine(timerCoroutine);
            isWorking = false;
        }
        
    }

    IEnumerator TimerCoroutine(float time)
    {
        float timeElapsed = 0;
        isWorking = true;
        while(timeElapsed < time)
        {
            timeElapsed += Time.deltaTime;
            UIManager.Instance.UpdateTimer((time - timeElapsed) / time);
            yield return new WaitForEndOfFrame();
        }
        isWorking = false;

        GameManager.Instance.GameEnd(GameOverType.Timer);
    }
}