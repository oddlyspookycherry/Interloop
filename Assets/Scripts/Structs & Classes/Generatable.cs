using UnityEngine;

public class Generatable : MonoBehaviour {
    public int Index{get; set;}

    public int poolIndex{get; set;}

    public bool isActive{get; set;}

    protected Animation anim;

    private void OnEnable() {
        if(anim == null)
            anim = GetComponent<Animation>();
        isActive = false;
    }

    public virtual void Dispose()
    {
        Recycle();
    }

    protected virtual void Recycle()
    {
        Deactivate();
        GenerationManager.Instance.Reclaim(this);
    }

    public void Activate()
    {
        isActive = true;
    }
    public void Deactivate()
    {
        isActive = false;
    }
}
