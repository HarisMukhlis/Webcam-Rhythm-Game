using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StaticManager : MonoBehaviour
{
    public static StaticManager Instance { get; private set; }

    [Header("Scene Instances")]
    [SerializeField] private int gameplayScene;
    [SerializeField] private int menuScene;
    [SerializeField] private int endscreenScene;

    [Header("Chart Load")]
    public ChartInfo chartInfo;

    [Header("End Screen Stats")]
    public int finalScore;
    [Space]
    public int perfectNotes;
    public int greatNotes;
    public int goodNotes;
    public int missNotes;
    [Space]
    public int maxCombo;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    // void SetChart(ChartInfo chartInfo)
    // {
    //     this.chartInfo = chartInfo;
    // }

    void SetEndStats(ChartManager chartManager)
    {
        perfectNotes = chartManager.perfectNotes;
        greatNotes = chartManager.greatNotes;
        goodNotes = chartManager.goodNotes;
        missNotes = chartManager.missNotes;
        maxCombo = chartManager.maxCombo;
    }

    public void PlayChart(ChartInfo chartInfo)
    {
        // SetChart(chartInfo);
        this.chartInfo = chartInfo;

        SceneManager.sceneLoaded += MainSceneLoaded;
        LoadScene(gameplayScene);
    }

    void MainSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Main scene loaded!");
        NoteSpawner noteSpawner = GameManager.Instance.noteSpawner;
        noteSpawner.LoadTimeline(chartInfo);
        noteSpawner.BeginChart();

        SceneManager.sceneLoaded -= MainSceneLoaded;
    }

    public void EndChart(ChartManager _chartManager)
    {
        SetEndStats(_chartManager);
        finalScore = GameManager.Instance.GetScore;

        Debug.Log("End Screen Called");

        SceneManager.sceneLoaded += EndscreenSceneLoaded;
        LoadScene(endscreenScene);
    }

    void EndscreenSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EndscreenUIManager.Instance.DisplayEndscreen();

        SceneManager.sceneLoaded -= EndscreenSceneLoaded;
    }
}