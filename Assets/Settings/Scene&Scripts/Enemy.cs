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
    public bool hostile = true;
    public bool spriteFacesRight = true;
    public float deathDelay = 0.8f;
    [Header("Animation state names (must match the boxes in the Animator window)")]
    public string idleState = "MushroomIdle";
    public string runState = "RunM";
    public string attackState = "AttackM";
    public string hitState = "TakeHitM";
    public string deathState = "DeathM";
    public float attackAnimTime = 0.6f;
    public float hitAnimTime = 0.3f;
    protected Transform player;
    protected Rigidbody2D rb;
    protected SpriteRenderer sr;
    protected Animator anim;
    int health;
    float nextAttack;
    bool dead;
    float animLockUntil;
    string currentState;
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
        if (dead) return;
        if (!hostile || player == null || Time.timeScale == 0f)
        {
            StopX();
            ShowMovement(false);
            return;
        }
        float dist = Vector2.Distance(transform.position, player.position);
        if (dist > detectRange)
        {
            StopX();
            ShowMovement(false);
            return;
        }
        float dir = Mathf.Sign(player.position.x - transform.position.x);
        if (sr != null) sr.flipX = spriteFacesRight ? dir < 0 : dir > 0;
        if (dist > attackRange)
        {
            if (rb != null) rb.linearVelocity = new Vector2(dir * moveSpeed, rb.linearVelocity.y);
            else transform.Translate(Vector2.right * dir * moveSpeed * Time.deltaTime);
            ShowMovement(true);
        }
        else
        {
            StopX();
            ShowMovement(false);
            if (Time.time >= nextAttack)
            {
                nextAttack = Time.time + attackCooldown;
                PlayOnce(attackState, attackAnimTime);
                var hh = player.GetComponent<HeroHealth>();

                if (hh != null) hh.TakeDamageFrom(damage, transform.position);
            }
        }
    }
    void StopX()
    {
        if (rb != null) rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }
   
    public void TakeDamage(int amount)
    {
        if (!hostile || dead) return;
        health -= amount;
        if (health <= 0)
        {
            dead = true;
            hostile = false;
            StopX();
            animLockUntil = float.MaxValue;
            currentState = null;
            PlayState(deathState);
            if (GameManager.Instance != null) GameManager.Instance.AddScore(points);
            Destroy(gameObject, deathDelay);
        }
        else
        {
            PlayOnce(hitState, hitAnimTime);
            if (sr != null) StartCoroutine(Flash());
        }
    }
    // Had an issue with the animations of the mushroom playing so i decided ti directly call the animations from the code
    void ShowMovement(bool running)
    {
        if (Time.time < animLockUntil) return;
        PlayState(running ? runState : idleState);
    }
    void PlayOnce(string state, float length)
    {
        animLockUntil = Time.time + length;
        currentState = null;
        PlayState(state);
    }

    void PlayState(string state)
    {
        if (anim == null || string.IsNullOrEmpty(state) || state == currentState) return;
        int hash = Animator.StringToHash(state);
        if (!anim.HasState(0, hash)) return;
        anim.Play(hash, 0, 0f);
        currentState = state;
    }
    IEnumerator Flash()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        if (sr != null) sr.color = Color.white;
    }
}