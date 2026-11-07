using Unity.VisualScripting;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    [Header("Patrol")]
    public float patrolSpeed = 2f;
    public float patrolDistance = 4f;

    [Header("Chase & Attack")]
    public float detectRange = 6f;
    public float chaseSpeed = 3.5f;
    public float attackRange = 1.2f;
    public float attackCoolDown = 1.5f;
    public int attackDamage = 1;
    public float attackWindup = 0.35f;//sign before the damage lands
    public float attackRecovery = 0.4f;//freezes after the hit
    public float hitRange = 1.5f;//how close the hero needs to be for the hit to land
    bool isAttacking;

    [Header("Health")]
    public int maxHealth = 5;
    public float hurtStunTime = 0.4f;
    public float deathDelay = 1f;

    [Header("Sprite")]
    public bool spriteFacesRight = true;

    Transform hero;
    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sr;
    Collider2D col;

    Vector2 startPos;
    int patrolDir = 1;
    int health;
    float nextAttackTime;
    float stunUntil;
    bool dead;



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        rb.gravityScale = 0f;//I set the gravity to zero here as well to make sure character is flying in place and not falling down
        rb.freezeRotation = true;

        startPos = transform.position;
        health = maxHealth;

        GameObject h = GameObject.FindGameObjectWithTag("Player");
        if (h != null)
        {
            hero = h.transform;
        }
    }
    void Update()
    {
        if(dead)
        {
            return;
        }
        if(Time.time < stunUntil)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        float distToHero = hero != null
            ? Vector2.Distance(transform.position, hero.position)
            : float.MaxValue;
        if(distToHero <= attackRange)
        {
            rb.linearVelocity = Vector2.zero;
            FaceTowards(hero.position.x - transform.position.x);
            TryAttack();
        }
        else if (distToHero <= detectRange)
        {
            Vector2 dir = ((Vector2)hero.position - (Vector2)transform.position).normalized;
            rb.linearVelocity = dir * chaseSpeed;
            FaceTowards(dir.x);
        }
        else
        {
            Patrol();
        }
    }
    void Patrol()
    {
        if (transform.position.x > startPos.x + patrolDistance)
        {
            patrolDir = -1;
        }
        else if(transform.position.x < startPos.x - patrolDistance)
        {
            patrolDir = 1;
        }
        //go back to og floating position after chasing
        float vy = Mathf.Clamp(startPos.y - transform.position.y, -1f, 1f);

        rb.linearVelocity = new Vector2(patrolDir * patrolSpeed, vy);
        FaceTowards(patrolDir);

    }

    void TryAttack()
    {
        if(Time.time < nextAttackTime)
        {
            return;
        }
        nextAttackTime = Time.time + attackCoolDown;
        anim.SetTrigger("Attack");

        //Calls method takedDamage from player
        hero.SendMessage("TakeDamage", attackDamage);

    }
    void FaceTowards(float xDirection)
    {
        if(Mathf.Abs(xDirection) < 0.01f)
        {
            return;
        }
        bool movingRight = xDirection > 0;
        sr.flipX = (movingRight != spriteFacesRight);
    }
    public void TakeHit()
    {
        if(dead)
        {
            return;
        }
        health--;
        if(health <= 0)
        {
            Die();
        }
        else
        {
            anim.SetTrigger("Hurt");
            stunUntil = Time.time + hurtStunTime;
        }
    }
    void Die()
    {
        dead = true;
        rb.linearVelocity = Vector2.zero;
        col.enabled = false;
        anim.SetTrigger("Die");
        Destroy(gameObject.gameObject, deathDelay);
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
