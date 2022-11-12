using UnityEngine;

public class CircleInputCollider : InputCollider
{
    public Vector2 Offset;

    public float Radius;

    public override bool IsInsideCollider(Vector2 point)
    {
        Vector2 relativePos = point - ((Vector2)transform.localPosition + Offset);

        return relativePos.magnitude <= Radius;
    
    }

    #if UNITY_EDITOR
        private void OnDrawGizmos() {

            UnityEditor.Handles.color = Color.green;

            UnityEditor.Handles.DrawWireDisc(transform.localPosition + (Vector3)Offset, Vector3.forward, Radius);
        }
    #endif

}
