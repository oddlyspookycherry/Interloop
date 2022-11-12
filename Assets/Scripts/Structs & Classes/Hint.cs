using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Hint : MonoBehaviour
{

    public float time;
    private const float animTime = 0.6f;
    private float step;
    private Text hintText;

    private void OnEnable()
    {
        if (hintText == null)
            hintText = GetComponent<Text>();
    }

    public void Play()
    {
        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(Playback());
    }

    public void Stop()
    {
        StopAllCoroutines();
        if (gameObject.activeSelf)
            StartCoroutine(Disappear());
    }

    private IEnumerator Disappear()
    {
        int steps = (int)(hintText.color.a / step) + 1;
        for (int i = 0; i < steps; i++)
        {

            float value = hintText.transform.localScale.x - step;
            hintText.transform.localScale = Mathf.Clamp01(value) * Vector3.one;
            yield return new WaitForFixedUpdate();
        }
        gameObject.SetActive(false);
    }

    private IEnumerator Playback()
    {
        if (time < 2f * animTime)
            throw new System.Exception("Invalid hint time.");

        int steps = (int)(animTime / Time.fixedDeltaTime);
        step = 1f / steps;

        for (int i = 1; i <= steps; i++)
        {
            hintText.transform.localScale = i * step * Vector3.one;
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(time - animTime);

        for (int i = steps - 1; i >= 0; i--)
        {

            hintText.transform.localScale = i * step * Vector3.one;
            yield return new WaitForFixedUpdate();
        }
        gameObject.SetActive(false);
    }
}