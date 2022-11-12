using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Level", order = 0)]
public class Level : ScriptableObject {
    public Vector2[] nodes = new Vector2[0];

    [UnityEngine.Serialization.FormerlySerializedAs("dots")]
    public Vector2[] keyDots = new Vector2[0];
    public Vector2[] falseDots = new Vector2[0];
    public Vector2 portal;

    public float maxTime, minTime;
    public float time {
        get {
            return Mathf.Lerp(maxTime, minTime, GameState.Interpolant);
        }
    }

    public int nodeCount {
        get {
            return nodes.Length;
        }
    }

    public int keyDotCount {
        get {
            return keyDots.Length;
        }
    }

    public int falseDotCount {
        get {
            return falseDots.Length;
        }
    }
}