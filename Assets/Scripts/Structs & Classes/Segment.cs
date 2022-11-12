using UnityEngine;

public class Segment : Generatable {

    public void Initialize(Vector2 pos1, Vector2 pos2, Color color, float width)
    {
        GetComponent<SpriteRenderer>().color = color;
        transform.localPosition = (pos1 + pos2) / 2f;
        transform.localScale = new Vector2(Vector2.Distance(pos1, pos2), width);
        transform.localRotation = Quaternion.FromToRotation(Vector2.right, pos2 - pos1);
    }

    public override void Dispose()
    {
        anim.Play("SegmentDisappear", PlayMode.StopAll);
    }
}