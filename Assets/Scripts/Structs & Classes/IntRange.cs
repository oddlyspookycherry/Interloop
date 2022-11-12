using UnityEngine;

[System.Serializable]
public struct IntRange 
{
    public int min;
    public int max;

    public int RandowValue {
        get {
            return Random.Range(min, max + 1);
        }
    }

    public IntRange(int min, int max)
    {
        this.min = min;
        this.max = max;
    }
    public static implicit operator IntRange(int n) => new IntRange(n, n);
}
