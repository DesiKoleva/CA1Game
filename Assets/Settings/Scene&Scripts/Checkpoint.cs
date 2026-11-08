using UnityEngine;

//The lamp post is the checkpoint for the player. After they walk by they are respawned back to there instead of the start if they die.
public class Checkpoint : MonoBehaviour
{
    public static Vector3 respawnPosition;
    public static bool hasCheckpoint = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.GetComponentInParent<HeroHealth>() == null)
        {
            return;
        }
        respawnPosition = other.transform.position;
        hasCheckpoint = true;
        //Debug.Log("Checkpoint saved");
    }
    
}
