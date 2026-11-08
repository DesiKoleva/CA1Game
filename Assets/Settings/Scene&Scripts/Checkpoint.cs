using UnityEngine;

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
    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
}
