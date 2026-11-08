using UnityEngine;
// Put on the Well, Statue, Graveyard and Bulletin Board.
// Add a BoxCollider2D (Is Trigger ON) sized around the object, then type the story in the Inspector.
[RequireComponent(typeof(Collider2D))]
public class StoryPoint : MonoBehaviour
{
    [TextArea(3, 8)] public string story = "Write the story here...";
    public bool onlyOnce = false;
    bool shown;
    void Reset() { GetComponent<Collider2D>().isTrigger = true; }
    static bool IsPlayer(Collider2D c) => c.CompareTag("Player") || c.transform.root.CompareTag("Player");
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsPlayer(other) || (onlyOnce && shown)) return;
        shown = true;
        if (GameManager.Instance != null) GameManager.Instance.ShowStory(story);
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (!IsPlayer(other)) return;
        if (GameManager.Instance != null) GameManager.Instance.HideStory();
    }
}
