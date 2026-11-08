using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Player : MonoBehaviour
{
    public int direction = 0;
    public int state = 0;
    public int speed;
    public float JumpHeight = 1f;
    public int maxJumps = 2;
    private int jumpCount = 0;
    private bool isJumping = false;
    private bool grounded = false;
    private Animator _animator;
    private Rigidbody2D _rigidbody2D;
    private HeroHealth _health;
    public int score = 0;
    public bool hasKey = false;
    

    void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _health = GetComponent<HeroHealth>();
        

    }

    void FixedUpdate()
    {
        grounded = false;
    }

    // Update is called once per frame
    void Update()
    {
        float move = Input.GetAxis("Horizontal");
        bool staggered = _health != null && _health.IsStaggered;

        if(!staggered)
        {
            _rigidbody2D.linearVelocity =
                new Vector2(move * speed, _rigidbody2D.linearVelocity.y);
        }
        if(move != 0 && !staggered)
        {
            state = move < 0 ? -1 : 1;
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (move < 0 ? -1 : 1);
            transform.localScale = scale;
        }
        _animator.SetBool("isRunning", move != 0);

        if(!grounded && jumpCount == 0)
        {
            jumpCount = 1;
        }
        if(jumpCount < maxJumps && Input.GetKeyDown(KeyCode.Space))
        {
            isJumping = true;
            jumpCount++;
            float jumpVelocity = Mathf.Sqrt(-2f * Physics2D.gravity.y * _rigidbody2D.gravityScale * JumpHeight);
            _rigidbody2D.linearVelocity =
            new Vector2(_rigidbody2D.linearVelocity.x, jumpVelocity);
        }
        _animator.SetBool("isJumping", isJumping);
       
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
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if(contact.normal.y > 0.5f)
            {
                grounded = true;
                if (_rigidbody2D.linearVelocity.y <= 0.1f)
                {
                    isJumping = false;
                    jumpCount = 0;
                }
                return;
            }
        }
    }

    //Collectables
   public void AddPresent(int amount)
    {
        score += amount;
    }
}
