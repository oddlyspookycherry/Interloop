using UnityEngine;

public static class CustomRandom
{
    public static int sign
    {
        get
        {
            return Random.value > 0.5f ? 1 : -1;
        }
    }

    public static Vector2 inPlayzone
    {
        get
        {
            float dimensionX = GenerationManager.Instance.dimensions.x / 2f;
            float dimensionY = GenerationManager.Instance.dimensions.y / 2f;
            float x = Random.Range(-dimensionX, dimensionX);
            float y = Random.Range(-dimensionY, dimensionY);
            return new Vector2(x, y) + GameState.totalOffsetVector;
        }
    }
}
