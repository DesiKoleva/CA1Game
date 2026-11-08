using UnityEngine;

//Locks the chest until the player has collected the key

public class Chest : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] string openState = "open";
    [SerializeField] string lockedState = "Closed";

    bool opened = false;

    void Awake()
    {
        if(animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(opened)
        {
            return;
        }
        Player player = other.GetComponentInParent<Player>();
        if(player == null)
        {
            return;
        }
        if(player.hasKey)
        {
            opened = true;
            animator.Play(openState, 0, 0f);
        }
        else
        {
            animator.Play(lockedState, 0, 0f);
        }
    }

   
}
