using UnityEngine;

public class Spikes : MonoBehaviour
{
   void OnTriggerEnter2D(Collider2D other)
    {
        var hh = other.GetComponentInParent<HeroHealth>();
        if(hh != null)
        {
            hh.TakeDamageFrom(9999, transform.position);
        }
    }
   void OnTriggerStay2D(Collider2D other)
    {
        OnTriggerEnter2D(other);
    }
}
