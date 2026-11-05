using UnityEngine;

public class Collectables : MonoBehaviour
{
    [SerializeField]
    Player player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("Hero").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player"
            && this.tag == "Present")
        {
            Destroy(this.gameObject);
            player.AddPresent();
        }
    }
}
