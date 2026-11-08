using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//This code deals with the hero's health and death
public class HeroHealth : MonoBehaviour
{
    public int maxHealth = 5;
    public float invincibleTime = 1f;

    [Header("Hit Feedback")]
    public float knockbackForce = 6f;
    public float staggerTime = 0.2f;
    public Slider healthBar;
    

    int health;
    float invincibleUntil;
    float staggerUntil;
    Rigidbody2D rb;
    SpriteRenderer sr;

    public bool IsStaggered => Time.time < staggerUntil;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    void Start()
    {
        if(Checkpoint.hasCheckpoint)
        {
            transform.position = Checkpoint.respawnPosition;
        }
        health = maxHealth;
        UpdateBar();
    }

    public bool IsFullHealth => health >= maxHealth;
    public void RestoreFullHealth()
    {
        health = maxHealth;
        UpdateBar();
        //Debug.Log("Hero health restored: " + health);
    }
    public void TakeDamage(int amount)
    {
        TakeDamageFrom(amount, transform.position);
    }

    public void TakeDamageFrom(int amount, Vector2 hitFrom)
    {
        if (Time.time < invincibleUntil)
        {
            return;
        }
        invincibleUntil = Time.time + invincibleTime;
        staggerUntil = Time.time + staggerTime;
        health -= amount;
        UpdateBar();
        //Debug.Log("Hero health: " + health);
        if (health <= 0)
        {
            Die();
            return;
        }
        Knockback(hitFrom);
        StartCoroutine(FlashRoutine());
    }
        void Knockback(Vector2 hitFrom)
        {
            if(rb == null)
            {
                return;
            }
            Vector2 dir = ((Vector2)transform.position - hitFrom);
            if(dir.sqrMagnitude < 0.0001f)
            {
                return;
            }
            rb.linearVelocity = Vector2.zero;
            rb.AddForce((dir.normalized + Vector2.up * 0.3f).normalized * knockbackForce, ForceMode2D.Impulse);
        }
        IEnumerator FlashRoutine()
        {
            if(sr == null)
            {
                yield break;
            }
            while (Time.time < invincibleUntil)
            {
                sr.color = Color.red;
                yield return new WaitForSeconds(0.08f);
                sr.color = Color.white;
                yield return new WaitForSeconds(0.08f);
            }
            sr.color = Color.white;
        }

        void UpdateBar()
        {
            if(GameManager.Instance != null)
        {
            GameManager.Instance.SetHealth(Mathf.Max(0, health), maxHealth);
        }
            if(healthBar == null)
        {
            return;
        }
        healthBar.maxValue = maxHealth;
        healthBar.value = Mathf.Max(0, health);
        }

    void Die()
    {
        var anim = GetComponentInChildren<Animator>();
        if (anim != null)
        {
            anim.updateMode = AnimatorUpdateMode.UnscaledTime;
            anim.Play("DeathH");
        }

        var player = GetComponent<Player>();
        if (player != null) player.enabled = false;

        var rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerDied();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }


}
