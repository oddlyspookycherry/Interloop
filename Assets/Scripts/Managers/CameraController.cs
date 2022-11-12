using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
    public static CameraController Instance{get; private set;}
    public float movementSpeed;

    private void OnEnable() {
        Instance = this;
    }

    public void Shift(Vector2 finalPosition)
    {
        StopAllCoroutines();
        StartCoroutine(Shifting(finalPosition));
    }

    IEnumerator Shifting(Vector2 finalPosition)
    {
        Vector2 step = (finalPosition - (Vector2)transform.position).normalized * movementSpeed * Time.fixedDeltaTime;
        int steps = (int)(Vector2.Distance(finalPosition, transform.position) / step.magnitude);

        for(int i = 0; i < steps; i++)
        {
            transform.position += (Vector3)step;
            yield return new WaitForFixedUpdate();
        }
        transform.position = finalPosition;
    }
}