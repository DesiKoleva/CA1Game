using JetBrains.Annotations;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int direction = 0;
    public int state = 0;
    public int speed;
    public float JumpHeight = 1f;
    public int maxJumps = 2;
    private int jumpCount = 0;
    private bool isJumping = false;
    private Animator _animator;
    private Rigidbody2D _rigidbody2D;
    

    void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        

    }

    // Update is called once per frame
    void Update()
    {
        Vector2 position = transform.position;
        float move = Input.GetAxis("Horizontal");
        position.x = position.x + speed * Time.deltaTime * move;
        transform.position = position;
        _rigidbody2D.linearVelocity = new Vector2(move * speed, _rigidbody2D.linearVelocity.y);

        if(move != 0)
        {
            state = move < 0 ? -1 : 1;
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (move < 0 ? -1 : 1);
            transform.localScale = scale;


            //_sprite.flipX = move < 0; //stays facing in the direction that it was flipped in
           // Debug.Log("move = " + move + "flipX = " + _sprite.flipX);//having issues with turning left
        }
        _animator.SetBool("IsRunning", move != 0);
        
        if(jumpCount < maxJumps && Input.GetKeyDown(KeyCode.Space))
        {
            isJumping = true;
            jumpCount++;
            float jumpVelocity = Mathf.Sqrt(-2f * Physics2D.gravity.y * _rigidbody2D.gravityScale * JumpHeight);
            _rigidbody2D.linearVelocity = new Vector2(_rigidbody2D.linearVelocity.x, jumpVelocity);
        }

        _animator.SetBool("IsJumping", isJumping);
       
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        CheckGround(collision);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        CheckGround(collision);
    }

    void CheckGround(Collision2D collision)
    {
        if (_rigidbody2D.linearVelocity.y > 0.1f) return;
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if(contact.normal.y > 0.5f)
            {
                isJumping = false;
                jumpCount = 0;
                return;
            }
        }
    }
}
