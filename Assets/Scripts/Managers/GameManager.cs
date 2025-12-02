using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Setup")]
    // public PythonReceiver pythonReceiver;
    // public UIManager uiManager;
    public NoteSpawner noteSpawner;

    [Header("Glove")]
    public GloveScript leftGlove;
    public GloveScript rightGlove;
    [Space]
    [SerializeField] private string _leftGloveTag = "LeftGlove";
    [SerializeField] private string _rightGloveTag = "RightGlove";
    public TagHandle LeftGloveTag { get; private set; }
    public TagHandle RightGloveTag { get; private set; }

    [Header("Settings Stats")]
    public float latency = 0.1f;
    [Space]
    public float sensitivity = 10f;
    public Vector2 ScreenSize { get; private set; } = new Vector2(1920, 1080);
    public float ScreenRatio { get => ScreenSize.x / ScreenSize.y; }

    [Header("Game Stats")]
    public const int MAX_SCORE = 1000000;
    public float score = 0;
    public int GetScore { get => Mathf.Min((int)Mathf.Ceil(score * (float)MAX_SCORE / 100f), MAX_SCORE); }  //for displaying the score, millionmaster style

    [Header("Health")]
    [SerializeField] private float maxHP = 5f;
    public float currentHP { get; private set; }
    [SerializeField][Tooltip("Health regeneration per second")] private float regenHP = 0.1f;

    [Header("Events")]
    public UnityEvent onDamaged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            // DontDestroyOnLoad(this.gameObject);
        }
    }

    void Start()
    {
        currentHP = maxHP;

        LeftGloveTag = TagHandle.GetExistingTag(_leftGloveTag);
        RightGloveTag = TagHandle.GetExistingTag(_rightGloveTag);

        ScreenSize = new Vector2(Screen.width, Screen.height);
    }

    void Update()
    {
        currentHP = Mathf.Min(currentHP + (regenHP * Time.deltaTime), maxHP);
    }

    public void DealDamage()
    {
        currentHP -= 1f;
        onDamaged.Invoke();
    }
}
