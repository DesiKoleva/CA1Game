using UnityEngine;

public class Player : MonoBehaviour
{
    public int direction = 0;
    public int state = 0;
    public int speed;
    public float JumpHeight = 1f;
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
        
        if(!isJumping && Input.GetKeyDown(KeyCode.Space))
        {
            isJumping = true;
            _rigidbody2D.linearVelocity = Vector2.zero;
            _rigidbody2D.AddForce(new Vector2(0, Mathf.Sqrt(-2 * Physics2D.gravity.y *
                JumpHeight)), ForceMode2D.Impulse);
        }

        _animator.SetBool("IsJumping", isJumping);
       
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        //if player collides with anything above when jumping
        if(collision.GetContact(0).normal.y > 0.5f)
        {
            isJumping = false;
        }
    }
}
