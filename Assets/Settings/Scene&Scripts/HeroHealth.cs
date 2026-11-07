using UnityEngine;
using UnityEngine.SceneManagement;

public class HeroHealth : MonoBehaviour
{
    public int maxHealth = 5;
    public float invincibleTime = 1f;
    int health;
    float invincibleUntil;

    void Start()
    {
        health = maxHealth;
    }
    public void TakeDamage(int amount)
    {
        if(Time.time < invincibleUntil)
        {
            return;
        }
        invincibleUntil = Time.time + invincibleTime;
        health -= amount;
        Debug.Log("Hero health: " + health);

        if(health <= 0)
        {
            Die();
        }
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
