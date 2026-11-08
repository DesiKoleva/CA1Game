using UnityEngine;
// Goblin and Skeleton use this. Set a different riddle on each in the Inspector.
public class RiddleEnemy : Enemy
{
    [Header("Riddle")]
    [TextArea(2, 5)] public string riddle = "Riddle goes here";
    public string correctAnswer = "Right answer";
    public string wrongAnswer = "Wrong answer";
    public float riddleRange = 4f;
    enum State { Waiting, Asking, Passive, Hostile }
    State state = State.Waiting;
    protected override void Awake()
    {
        base.Awake();
        hostile = false; // calm until the riddle is answered
    }
    protected override void Update()
    {
        var gm = GameManager.Instance;
        if (state == State.Waiting && player != null && gm != null && gm.IsPlaying && !gm.IsBusy &&
        Vector2.Distance(transform.position, player.position) < riddleRange)
        {
            state = State.Asking;
            gm.AskRiddle(riddle, correctAnswer, wrongAnswer, OnAnswered);
        }
        base.Update();
    }
    void OnAnswered(bool correct)
    {
        if (correct)
        {
            state = State.Passive;
            hostile = false;
            // Let the hero walk straight through him
            foreach (var mine in GetComponentsInChildren<Collider2D>())
                foreach (var theirs in player.GetComponentsInChildren<Collider2D>())
                    Physics2D.IgnoreCollision(mine, theirs, true);
        }
        else

        {
            state = State.Hostile;
            hostile = true; // now chases and attacks; hero must kill him
        }
    }
}
