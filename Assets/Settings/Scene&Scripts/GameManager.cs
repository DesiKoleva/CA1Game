using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
   public static GameManager Instance 
    { 
        get; 
        private set; 
    }

    public bool IsPlaying 
    { 
        get; 
        private set;
    }
    public bool IsBusy 
    { 
        get; 
        private set; 
    }
    public int Score 
    { 
        get; 
        private set;
    }

    [Header("Settings")]
    public int riddleCorrectPoints = 50;
    public float deathScreenSeconds = 5f;
    public float typeSpeed = 0.03f;

    GameObject menuPanel, hudPanel, deathPanel, riddlePanel, storyPanel;
    RectTransform healthFill;
    Text scoreText, riddleQuestion, riddleOptions, riddleResult, storyText;
    Font font;
    Coroutine typing;
    bool dead;

    void Awake()
    {
        Instance = this;
        font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        EnsureEventSystem();
        BuildUI();
        ShowMenu();
    }

    public void AddScore(int amount)
    {
        Score += amount;
        scoreText.text = "Score: " + Score;
    }
    public void SetHealth(float current, float max)
    {
        float r = Mathf.Clamp01(current / Mathf.Max(1f, max));
        healthFill.anchorMax = new Vector2(r, 1f);
    }
    public void PlayerDied()
    {
        if(dead)
        {
            return;
        }
        dead = true;
        IsPlaying = false;
        deathPanel.SetActive(true);
        StartCoroutine(RestartRoutine());
    }
    public void AskRiddle(string question, string correct, string wrong, Action<bool> onDone)
    {
        if(!IsBusy)
        {
            StartCoroutine(RiddleRoutine(question, correct, wrong, onDone));
        }
    }
    public void ShowStory(string text)
    {
        if(IsBusy)
        {
            return;
        }
        storyPanel.SetActive(true);
        if (typing != null) StopCoroutine(typing);
        typing = StartCoroutine(TypeRoutine(text));
    }
    public void HideStory()
    {
        if(typing != null)
        {
            StopCoroutine(typing);
        }
        storyPanel.SetActive(false);
    }
    void ShowMenu()
    {
        Time.timeScale = 0f;
        IsPlaying = false;
        menuPanel.SetActive(true);
        hudPanel.SetActive(false);
        deathPanel.SetActive(false);
        riddlePanel.SetActive(false);
        storyPanel.SetActive(false);
    }
    void StartGame()
    {
        menuPanel.SetActive(false);
        hudPanel.SetActive(true);
        Time.timeScale = 1f;
        IsPlaying = true;
    }
    IEnumerator RestartRoutine()
    {
        yield return new WaitForSecondsRealtime(deathScreenSeconds);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }
    IEnumerator RiddleRoutine(string q, string correct, string wrong, Action<bool> onDone)
    {
        IsBusy = true;
        HideStory();
        Time.timeScale = 0f;
        bool correctIsOne = UnityEngine.Random.value > 0.5f;
        string a1 = correctIsOne ? correct : wrong;
        string a2 = correctIsOne ? wrong : correct;

        riddleQuestion.text = q;
        riddleOptions.text = "[1] " + a1 + "\n[2] " + a2;
        riddleResult.text = "Press 1 or 2";
        riddlePanel.SetActive(true);

        bool right;
        while (true)
        {
            if (GameInput.PressedOne())
            {
                right = correctIsOne;
                break;
            }
            if (GameInput.PressedTwo())
            {
                right = !correctIsOne;
                break;
            }
            yield return null;

        }
        riddleResult.text = right ? "Correct! You may continue wise hero." : "Wrong! I won't let you pass without a duel!";
        yield return new WaitForSecondsRealtime(1.5f);

        riddlePanel.SetActive(false);
        Time.timeScale = 1f;
        IsBusy = false;
        if (right)
        {
            AddScore(riddleCorrectPoints);
            onDone?.Invoke(right);
        }
    }
        IEnumerator TypeRoutine(string full)
        {
            storyText.text = "";
            foreach(char c in full)
            {
                storyText.text += c;
                yield return new WaitForSeconds(typeSpeed);
            }
        }
    

    void EnsureEventSystem()
    {
        if (FindAnyObjectByType<EventSystem>() != null) return;
        var es = new GameObject("EventSystem", typeof(EventSystem));
#if ENABLE_INPUT_SYSTEM
        es.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
#else
es.AddComponent<StandaloneInputModule>();
#endif
    }
    void BuildUI()
    {
        var canvasGo = new GameObject("GameCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasGo.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        var scaler = canvasGo.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        Transform root = canvasGo.transform;
        // Start menu
        menuPanel = Panel("Menu", root, new Color(0.05f, 0.05f, 0.1f, 0.95f), Vector2.zero, Vector2.one).gameObject;
        Label("Title", menuPanel.transform, "CA1 GAME", 110, TextAnchor.MiddleCenter, new Vector2(0.1f, 0.55f), new Vector2(0.9f, 0.85f));
        var btnRt = Panel("StartButton", menuPanel.transform, new Color(0.2f, 0.7f, 0.3f), new Vector2(0.4f, 0.3f), new Vector2(0.6f, 0.42f));
        btnRt.gameObject.AddComponent<Button>().onClick.AddListener(StartGame);
        Label("StartLabel", btnRt, "START", 56, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one);
        // HUD (health bar + score, top-left)
        hudPanel = new GameObject("HUD", typeof(RectTransform));
        hudPanel.transform.SetParent(root, false);
        Stretch(hudPanel.GetComponent<RectTransform>());
        var bg = Panel("HealthBG", hudPanel.transform, new Color(0.15f, 0.15f, 0.15f, 0.9f), new Vector2(0.02f, 0.92f), new Vector2(0.27f, 0.97f));
        healthFill = Panel("HealthFill", bg, new Color(0.85f, 0.15f, 0.15f), Vector2.zero, Vector2.one);
        scoreText = Label("Score", hudPanel.transform, "Score: 0", 52, TextAnchor.MiddleLeft, new Vector2(0.29f, 0.91f), new Vector2(0.55f, 0.98f));
        // Riddle box
        riddlePanel = Panel("Riddle", root, new Color(0, 0, 0, 0.85f), new Vector2(0.15f, 0.05f), new Vector2(0.85f, 0.4f)).gameObject;
        riddleQuestion = Label("Q", riddlePanel.transform, "", 40, TextAnchor.UpperCenter, new Vector2(0.03f, 0.5f), new Vector2(0.97f, 0.97f));
        riddleOptions = Label("Opts", riddlePanel.transform, "", 40, TextAnchor.MiddleLeft, new Vector2(0.08f, 0.2f), new Vector2(0.97f, 0.52f));
        riddleResult = Label("Res", riddlePanel.transform, "", 36, TextAnchor.MiddleCenter, new Vector2(0.03f, 0.02f), new Vector2(0.97f, 0.2f));
        riddleResult.color = Color.yellow;
        // Story box
        storyPanel = Panel("Story", root, new Color(0, 0, 0, 0.8f), new Vector2(0.15f, 0.72f), new Vector2(0.85f, 0.9f)).gameObject;
        storyText = Label("StoryText", storyPanel.transform, "", 38, TextAnchor.MiddleLeft, new Vector2(0.03f, 0.05f), new Vector2(0.97f, 0.95f));
        // Defeated sign
        deathPanel = Panel("Death", root, new Color(0.3f, 0, 0, 0.85f), Vector2.zero, Vector2.one).gameObject;
        var dt = Label("DeathText", deathPanel.transform, "YOU HAVE BEEN DEFEATED", 100, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one);
        dt.color = new Color(1f, 0.3f, 0.3f);
    }
    static void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }
    RectTransform Panel(string name, Transform parent, Color c, Vector2 aMin, Vector2 aMax)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = aMin; rt.anchorMax = aMax;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
        go.GetComponent<Image>().color = c;
        return rt;
    }
    Text Label(string name, Transform parent, string s, int size, TextAnchor anchor, Vector2 aMin, Vector2 aMax)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Text));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = aMin; rt.anchorMax = aMax;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
        var t = go.GetComponent<Text>();
        t.font = font; t.text = s; t.fontSize = size; t.alignment = anchor;
        t.color = Color.white;
        t.horizontalOverflow = HorizontalWrapMode.Wrap;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        t.raycastTarget = false;
        return t;

    }
}
