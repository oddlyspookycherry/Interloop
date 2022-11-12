using UnityEngine;

[System.Serializable]
public struct FloatRange 
{
    public float min;
    public float max;

    public float RandowValue {
        get {
            return Random.Range(min, max);
        }
    }

    public FloatRange(float min, float max)
    {
        this.min = min;
        this.max = max;
    }
    public static implicit operator FloatRange(float n) => new FloatRange(n, n);
}
