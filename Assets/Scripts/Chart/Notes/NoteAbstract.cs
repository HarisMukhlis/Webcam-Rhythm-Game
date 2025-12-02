using UnityEngine;
using UnityEngine.Events;
public enum NoteType
{
    Tap,
    Arrow,
    Dodge,
}

[System.Serializable]
public class Note
{
    // [HideInInspector] 
    public float delay;
    [Space]
    public NoteType noteType;
    [Space]
    public Vector2 targetPos;
    public float dir = 0f;

    public Note() { }

    public Note(NoteType noteType, Vector2 targetPos, float dir = 0f)
    {
        this.noteType = noteType;
        this.targetPos = targetPos;
        this.dir = dir;
    }

    public Note(Note noteCopy)
    {
        this.noteType = noteCopy.noteType;
        this.targetPos = noteCopy.targetPos;
        this.dir = noteCopy.dir;
    }
}

public abstract class NoteAbstract : MonoBehaviour
{
    [Header("Setup")]
    // [SerializeField] protected NoteVisual noteVisual;
    [SerializeField] protected GameObject noteObject;
    [SerializeField] protected GameObject sprite;
    [SerializeField] protected GameObject noteTarget;
    // [SerializeField] protected int gloveLayer = 10;

    [Header("Note")]
    [SerializeField] protected float duration = 2f;
    [Space]
    public Note note;

    public float JudgmentPercent { get; protected set; } = 0f;
    [Header("Judgment")]
    [SerializeField][Tooltip("Tolerance before counting as a hit")] protected float judgmentTolerance = 0.3f;
    [SerializeField][Tooltip("Threshold for \"Perfect\" judgment, smaller than tolerance")] protected float judgmentDeadZone = 0.06f;
    [SerializeField][Tooltip("\"Miss\" timing, above this won't be counted as anything")] protected float judgmentMiss = 1f;
    [Space]
    [SerializeField][Tooltip("Good & Great thresholds (in that order). Calculated based on normalized value.")] protected float[] judgmentThresholds;

    [Header("Events")]
    public UnityEvent onSpawn;
    [Tooltip("Called when the note is hit")] public UnityEvent onHit;
    [SerializeField][Tooltip("")] private float zoneDelay;
    [Tooltip("Called when note enters the \"Indicator Zone\"")] public UnityEvent<float> onZone;
    [Tooltip("Called when the note is missed completely")] public UnityEvent onMiss;

    [Header("Sound FX")]
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip missSound;

    protected float elapsedTime = 0f;

    private bool isOnZoneInvoked = false;

    void Start()
    {
        // if (noteVisual == null)
        // {
        //     noteVisual = this.gameObject.GetComponent<NoteVisual>();
        // }

        if (noteObject == null || sprite == null)
        {
            Debug.LogWarning("Instances has not set at " + this.gameObject);
        }
        else
        {
            // noteVisual.NoteSpawn();
            SpawnNote();

            onSpawn.Invoke();
        }
    }

    virtual protected void Update()
    {
        elapsedTime += Time.deltaTime;

        CalculateJudgmentPercent();
        ZoneDetect();
        Move();

        DebugInput();
    }

    protected void ZoneDetect()
    {
        if (!isOnZoneInvoked && elapsedTime >= duration - zoneDelay)
        {
            onZone.Invoke(zoneDelay);
            isOnZoneInvoked = true;
        }
    }

    protected void CalculateJudgmentPercent()
    {
        float absElapsedTime = Mathf.Abs(elapsedTime - duration); //moves from (duration) to 0 to (duration) again

        float judgmentCutoff = Mathf.Clamp(absElapsedTime, judgmentDeadZone, judgmentTolerance);
        JudgmentPercent = Mathf.InverseLerp(judgmentTolerance, judgmentDeadZone, judgmentCutoff);
    }

    protected void Move()
    {
        noteObject.transform.localPosition -= new Vector3(0, ChartManager.Instance.noteSpeed * Time.deltaTime, 0);

        if (elapsedTime >= duration + judgmentTolerance)
        {
            JudgmentPercent = 0f;
            Hit(); //certain miss
        }
    }

    protected void AddScore()
    {
        float result = 0f;
        if (JudgmentPercent >= 0.001f)
        {
            result = (100 / (float)ChartManager.Instance.TotalNotes) * JudgmentPercent;
            // noteVisual.NoteHit();
            AudioManager.Instance.PlaySfx(hitSound);

            onHit.Invoke();
        }
        else
        {
            AudioManager.Instance.PlaySfx(missSound);
            UIManager.Instance.DamageEffect();
            onMiss.Invoke();
        }
        //     noteVisual.NoteMiss();

        GameManager.Instance.score += result;
    }

    protected void DisplayJudgment()
    {
        string judgmentDesc;
        Color judgmentColor;

        if (JudgmentPercent > 0.99f)
        {
            judgmentDesc = "Perfect!!";
            judgmentColor = Color.yellow;

            ChartManager.Instance.AddHit(JudgmentType.PerfectHit);
        }
        else if (JudgmentPercent >= judgmentThresholds[1])
        {
            judgmentDesc = "Great!";
            judgmentColor = Color.cyan;

            ChartManager.Instance.AddHit(JudgmentType.GreatHit);
        }
        else if (JudgmentPercent >= judgmentThresholds[0])
        {
            judgmentDesc = "Good!";
            judgmentColor = Color.blue;

            ChartManager.Instance.AddHit(JudgmentType.GoodHit);
        }
        else
        {
            judgmentDesc = "Miss!";
            judgmentColor = Color.gray;

            ChartManager.Instance.AddHit(JudgmentType.MissedHit);
        }

        UIManager.Instance.AddJudgment(judgmentDesc, judgmentColor);
        Debug.Log(judgmentDesc + " : " + JudgmentPercent + " | Score: " + GameManager.Instance.GetScore);
    }

    virtual protected void Hit()
    {
        AddScore();
        DisplayJudgment();
        UIManager.Instance.UpdateScore();

        // Destroy(this.gameObject);
        Destroy(this);
    }

    abstract protected void DebugInput();
    abstract protected void SpawnNote();
}