public class FalseDot : Generatable {
    public override void Dispose()
    {
        anim.Play("FalseDotDisappear");
    }
    public void Outline()
    {
        anim.Play("GlowGreen");
    }
}