using UnityEngine;
using UnityEngine.InputSystem;

public class HeroCombat : MonoBehaviour
{
    public Transform attackPoint;
    public float attackRange = 0.8f;
    public LayerMask enemyLayers;
    public float attackCooldown = 0.5f;
    public float hitDelay = 0.15f;

    Animator anim;
    float nextAttackTime;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if(AttackPressed() && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;

            if (anim != null) anim.SetTrigger("Attack");
            Invoke(nameof(DealDamage), hitDelay);

        }
    }
    bool AttackPressed()
    {
        return (Keyboard.current.jKey.wasPressedThisFrame);
        
       
    }
    void DealDamage()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach(Collider2D hit in hits)
        {
            FlyingEnemy enemy = hit.GetComponent<FlyingEnemy>();
            if(enemy != null)
            {
                enemy.TakeHit();
            }
            Enemy groundEnemy = hit.GetComponentInParent<Enemy>();
            if(groundEnemy != null)
            {
                groundEnemy.TakeDamage(1);
            }
            RiddleEnemy riddleEnemy = hit.GetComponentInParent<RiddleEnemy>();
            if (riddleEnemy != null)
            {
                riddleEnemy.TakeDamage(1);
            }
        }
    }
    void OnDrawGizmosSelected()
    {
        if(attackPoint == null)
        {
            return;

        }
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
