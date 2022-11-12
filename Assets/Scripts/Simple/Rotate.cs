using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float fancyRotationSpeed;
    private void Update() {
        transform.Rotate(0f,0f,fancyRotationSpeed * Time.deltaTime);
    }
}
