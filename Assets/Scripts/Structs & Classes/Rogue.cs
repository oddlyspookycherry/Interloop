using UnityEngine;
using System.Collections;

public class Rogue : Generatable
{
    private float speed;

    private void OnTriggerEnter2D(Collider2D other) {
        if(isActive && other.CompareTag("Player"))
        {
            GameManager.Instance.GameEnd(GameOverType.Hit);
        }
    }

    public void Enable(float delay) {
        speed = GameState.rogueSpeed.RandowValue;
        StopAllCoroutines();
        StartCoroutine(Movement(delay));
    }
    IEnumerator Movement(float delay)
    {
        yield return new WaitForSeconds(delay);
        while(true)
        {
            Vector2 newPoint = GenerationManager.Instance.GetNodePosition(transform.position, false);
            Vector2 step = (newPoint - (Vector2)transform.localPosition).normalized * speed * Time.fixedDeltaTime;
            int steps = (int)(Vector2.Distance(transform.localPosition, newPoint) / step.magnitude);
            for(int i = 0; i < steps; i++)
            {
                if(GameState.isGameOver)
                    yield break;
                transform.localPosition += (Vector3)step;
                yield return new WaitForFixedUpdate();
            }
            transform.localPosition = newPoint;
        }
    }

    public override void Dispose()
    {
        StopAllCoroutines();
        anim.Play("SawDisappear");
    }

    protected override void Recycle()
    {
        base.Recycle();
    }

}