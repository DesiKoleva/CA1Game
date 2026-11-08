using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HeroHealth : MonoBehaviour
{
    public int maxHealth = 5;
    public float invincibleTime = 1f;

    [Header("Hit Feedback")]
    public float knockbackForce = 6f;
    public float staggerTime = 0.2f;
    public Slider healthBar;
    public AudioSource hitSound;

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
        if (hitSound != null)
        {
            hitSound.Play();
        }
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
            if(healthBar == null)
            {
                return;
            }
            healthBar.maxValue = maxHealth;
            healthBar.value = Mathf.Max(0, health);
        }

    void Die()
    {
        //Restart the level.Or at checkpoint.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
   
    void Update()
    {
        
    }
}
