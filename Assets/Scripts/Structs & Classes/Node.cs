using UnityEngine;

public class Node : Generatable {
    
    public bool isInGraph{get; set;}

    public int graphIndex{get; set;}

    SpriteRenderer rend;

    public void SetColor(Color color)
    {
        if(rend == null)
            rend = GetComponent<SpriteRenderer>();
        rend.color = color;
    }

    public override void Dispose()
    {
        anim.Play("NodeDisappear");
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(isActive && other.CompareTag("Player"))
        {
            if(!isInGraph)
            {
                GenerationManager.Instance.GenerateVFX(VFXType.NodeBlast, transform.position);
                RayGraph.Instance.AddToGraph(this);
            }
            else 
            {
                if(RayGraph.Instance.CompleteCycle(this))
                    GameManager.Instance.CheckForDotOverlap();
            }
        }
    }
}