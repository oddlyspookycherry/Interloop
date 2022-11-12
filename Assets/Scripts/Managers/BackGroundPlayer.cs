using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class BackGroundPlayer : MonoBehaviour {
    public static BackGroundPlayer Instance{get; private set;}
    private AudioSource source;
    public float muteLength;
    [Range(0f, 1f)]
    public float minVolume;

    public AudioClip[] clips;
    private int currentClip;
    private void Awake() {
        Instance = this;
        source = GetComponent<AudioSource>();
        source.volume = minVolume;
        int clipIndex = PlayerPrefs.GetInt("BGMclip", 0);
        source.clip = clips[clipIndex];
        currentClip = clipIndex;
        source.Play();
    }
    
    public void MuteSmooth()
    {
        StopAllCoroutines();
        StartCoroutine(Mute());
    }
    public void EnableSmooth()
    {
        StopAllCoroutines();
        StartCoroutine(Enable());
    }
    IEnumerator Mute()
    {
        float currentVolume = source.volume;
        int steps = (int)(muteLength / Time.fixedDeltaTime);
        float step = (currentVolume - minVolume)/ steps;
        for(int i = 0; i < steps; i++)
        {
            source.volume -= step;
            yield return new WaitForFixedUpdate();
        }
        source.volume = minVolume;
    }
    IEnumerator Enable()
    {
        float currentVolume = source.volume;
        int steps = (int)(muteLength / Time.fixedDeltaTime);
        float step = (1f - currentVolume) / steps;
        for(int i = 0; i < steps; i++)
        {
            source.volume += step;
            yield return new WaitForFixedUpdate();
        }
        source.volume = 1f;
    }

    public void SetNextSong()
    {
        source.Stop();
        currentClip = (currentClip + 1) % clips.Length;
        source.clip = clips[currentClip];
        PlayerPrefs.SetInt("BGMclip", currentClip);
        source.Play();
    }
}