using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance{get; private set;}
    public float speed;

    public bool computerControls = false;
    

    private InputCollider inputCollider;

    private bool canMove;

    private Rigidbody2D rb2d;

    public bool NonControlable{get; set;}
    private Animation anim;

    private void OnEnable() {
        Instance = this;
        if(anim == null)
            anim = GetComponent<Animation>();
        if(inputCollider == null)
            inputCollider = GetComponent<InputCollider>();
        if(rb2d == null)
            rb2d = GetComponent<Rigidbody2D>();
    }
    private void Update() {
        if(NonControlable)
        {
            rb2d.velocity = Vector2.zero;
            return;
        }
        
        if(computerControls && Input.GetMouseButton(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            rb2d.velocity = (mousePos - rb2d.position) * speed;
        }
    
        else if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);

            if(touch.phase == TouchPhase.Began && !inputCollider.IsInsideCollider(touchPos))
                canMove = false;

            if(inputCollider.IsInsideCollider(touchPos))
                canMove = true;

            if(canMove)       
                rb2d.velocity = (touchPos - rb2d.position) * speed;
        }
        else
        {
            rb2d.velocity = Vector2.zero;
        }
    }

    public void Disable()
    {
        anim.Play("PlayerDisappear", PlayMode.StopAll);
    }

    private void SwitchOff()
    {
        gameObject.SetActive(false);
    }
}
