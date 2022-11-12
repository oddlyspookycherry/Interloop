public class KeyDot : Generatable {
    public override void Dispose()
    {
        anim.Play("KeyDotDisappear");
    }
    public void Outline()
    {
        anim.Play("GlowRed");
    }
}