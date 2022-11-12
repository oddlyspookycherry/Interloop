using UnityEngine;

public class HintManager : MonoBehaviour {
    public static HintManager Instance{get; private set;}

    public Hint[] hints;

    private int shownIndex;

    private void Awake() {
        Instance = this;
    }

    public void ShowHint(int index)
    {
        hints[shownIndex].Stop();
        hints[index].Play();
        shownIndex = index;
    }
}