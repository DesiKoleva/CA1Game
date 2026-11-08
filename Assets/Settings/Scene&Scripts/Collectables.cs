using UnityEngine;

//The moneybags carry points
//The barell restores the health of the player
//The key is collected to allow the player to open the chest at the start of the map for a huge point present
public class Collectables : MonoBehaviour
{
    [SerializeField] int points = 10;
    [SerializeField] bool restoreHEalth = false; //wrote it wrong at first and now when i fix it wouldnt replace itself correctly now
    [SerializeField] bool floats = true;
    [SerializeField] float floatHeight = 0.1f;
    [SerializeField] float floatSpeed = 2f;
    [SerializeField] bool givesKey = false;

    Vector3 startPos;

    
    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if(!floats)
        {
            return;
        }
        float offset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = startPos + new Vector3(0f, offset, 0f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponentInParent<Player>();
        HeroHealth health = other.GetComponentInParent<HeroHealth>();

        if(player == null && health == null)
        {
            return;
        }
        if(player != null && points > 0)
        {
            player.AddPresent(points);
            if(GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(points);

            }
        }
        if(restoreHEalth && health != null)
        {
            health.RestoreFullHealth();
        }
        if(givesKey && player != null)
        {
            player.hasKey = true;
        }
            Destroy(gameObject);
        
    }
}
