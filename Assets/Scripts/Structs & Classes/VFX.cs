using UnityEngine;

public class VFX : Generatable {
    private void OnEnable() {
        float time = GetComponent<ParticleSystem>().main.duration;
        GameManager.Instance.InvokeDelayed(time, Dispose);
    }
}