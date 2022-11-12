using UnityEngine;

public abstract class InputCollider : MonoBehaviour
{
    public abstract bool IsInsideCollider(Vector2 point);
}
