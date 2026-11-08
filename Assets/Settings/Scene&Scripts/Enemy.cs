using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 3;
    public int damage = 1;
    public int points = 25;
    [Header("Behaviour")]
    public float moveSpeed = 1.5f;
    public float detectRange = 6f;
    public float attackRange = 0.9f;
    public float attackCooldown = 1.2f;
    public float deathDelay = 0.8f;
    public bool hostile = true;
    public bool spriteFacesRight = true;
    protected Transform player;
    protected Rigidbody2D rb;
    protected SpriteRenderer sr;
    protected Animator anim;
    bool dead;
    int health;
    float nextAttack;
    protected virtual void Awake()
    {
        health = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
    }
    protected virtual void Start()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }
    protected virtual void Update()
    {
        if (!hostile || player == null || Time.timeScale == 0f) { StopX(); return; }
        float dist = Vector2.Distance(transform.position, player.position);

        if (dist > detectRange) { StopX(); return; }
        float dir = Mathf.Sign(player.position.x - transform.position.x);
        if (sr != null) sr.flipX = spriteFacesRight ? dir < 0 : dir > 0;
        if (dist > attackRange)
        {
            if (rb != null) rb.linearVelocity = new Vector2(dir * moveSpeed, rb.linearVelocity.y);
            else transform.Translate(Vector2.right * dir * moveSpeed * Time.deltaTime);
        }
        else
        {
            StopX();
            if (Time.time >= nextAttack)
            {
                nextAttack = Time.time + attackCooldown;
                var hh = player.GetComponent<HeroHealth>();
               if(hh != null)
                {
                    hh.TakeDamageFrom(damage, transform.position);
                }
            }
        }
    }
    void StopX()
    {
        SetRunning(false);
        if (rb != null) rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }
    public void TakeDamage(int amount)
    {
        if (!hostile) return;
        health -= amount;
       if(health <= 0)
        {
            dead = true;
            hostile = false;
            SetRunning(false);
            Trigger("Die");
            if(rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
            if(GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(points);
            }
            Destroy(gameObject, deathDelay);
        }
        else
        {
            Trigger("Hit");
            if(sr != null)
            {
                StartCoroutine(Flash());
            }
        }
    }
    void SetRunning(bool running)
    {
        if(anim != null)
        {
            anim.SetBool("IsRunning", running);
        }
    }
    void Trigger(string name)
    {
        if(anim!= null)
        {
            anim.SetTrigger(name);
        }
    }
    IEnumerator Flash()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        if (sr != null) sr.color = Color.white;
    }
}