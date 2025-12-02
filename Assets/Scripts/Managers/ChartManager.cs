using UnityEngine;

public enum JudgmentType
{
    PerfectHit,
    GreatHit,
    GoodHit,
    MissedHit
}

public class ChartManager : MonoBehaviour
{
    public static ChartManager Instance { get; private set; }

    [Header("Setup")]
    [SerializeField] private NoteSpawner noteSpawner;

    [Header("Settings")]
    public float noteSpeed = 10f;

    [Header("Chart Stats")]

    public int perfectNotes;
    public int greatNotes;
    public int goodNotes;
    public int missNotes;
    [Space]
    public int currentCombo = 0;
    public int maxCombo;

    //chart infos
    public int TotalNotes { get; private set; } = 1;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // void Start()
    // {
    //     CalculateTotalNotes();
    // }

    public void AddHit(JudgmentType judgmentType)
    {
        switch (judgmentType)
        {
            case JudgmentType.PerfectHit:
                perfectNotes++;
                AddCombo();
                break;

            case JudgmentType.GreatHit:
                greatNotes++;
                AddCombo();
                break;

            case JudgmentType.GoodHit:
                goodNotes++;
                AddCombo();
                break;

            case JudgmentType.MissedHit:
                missNotes++;
                currentCombo = 0;
                break;
        }
    }

    void AddCombo()
    {
        currentCombo++;
        if (currentCombo > maxCombo)
            maxCombo = currentCombo;
    }

    public void CalculateTotalNotes()
    {
        if (noteSpawner != null)
        {
            TotalNotes = noteSpawner.Notes.Count;
            if (TotalNotes <= 0) TotalNotes = 1;
        }
    }
}
