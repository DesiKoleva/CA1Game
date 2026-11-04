using UnityEngine;

public class Player : MonoBehaviour
{
    public int direction = 0;
    public int speed;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 position = transform.position;
        float move = Input.GetAxis("Horizontal");
        position.x = position.x + speed * Time.deltaTime * move;
        transform.position = position;
    }
}
