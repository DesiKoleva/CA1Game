using UnityEngine;

public class Ladder : MonoBehaviour
{
    public float climbSpeed = 3f;
    Rigidbody2D hero;
    float normalGravity;
    bool climbing;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hero != null) return;
        var rb = other.attachedRigidbody;
        if (rb != null && rb.GetComponent<HeroHealth>() != null)
        {
            hero = rb;
            normalGravity = rb.gravityScale;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (hero != null && other.attachedRigidbody == hero) LetGo();
    }

    void Update()
    {
        if (hero == null) return;

        bool up = Input.GetKey(KeyCode.UpArrow);
        bool down = Input.GetKey(KeyCode.DownArrow);

        if (up || down)
            climbing = true;

        if (climbing)
        {
            hero.gravityScale = 0f;
            float v = up ? climbSpeed : down ? -climbSpeed : 0f;
            hero.linearVelocity = new Vector2(hero.linearVelocity.x, v);
        }
    }

    void LetGo()
    {
        hero.gravityScale = normalGravity;
        hero = null;
        climbing = false;
    }
}
