using UnityEngine;

public struct Line
{
    public float a, b, c;

    public Vector2 normal;

    public Vector2 First;

    public Vector2 Second;
    public Line(Vector2 p1, Vector2 p2)
    {
        First = p1;
        Second = p2;

        p2 = p2 - p1;
        GeneralMethods.Swap(ref p2.x, ref p2.y);
        p2.x *= -1;
        normal = p2;
        a = p2.x; 
        b = p2.y;
        c = -(CustomMaths.Dot(p2, p1));
    }
}
