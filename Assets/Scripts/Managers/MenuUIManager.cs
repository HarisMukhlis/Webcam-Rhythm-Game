using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine.Events;
using TMPro;
using System.Linq;

public class MenuUIManager : MonoBehaviour
{
    public static MenuUIManager Instance { get; private set; }

    [Header("Setup")]
    [SerializeField] private GraphicRaycaster raycaster;
    [SerializeField] private EventSystem eventSystem;

    [Header("Chart Selection Screen")]
    [SerializeField] private RectTransform chartPanelParent;
    [SerializeField] private GameObject chartPanelPrefab;

    [Header("Chart Info Display")]
    [SerializeField] private TextMeshProUGUI chartTitleText;
    [SerializeField] private TextMeshProUGUI songArtistText;
    [Space]
    [SerializeField] private UnityEngine.UI.Image coverImageDisplay;
    [Space]
    [SerializeField] private TextMeshProUGUI difficultyRatingText;
    [SerializeField] private TextMeshProUGUI bpmText;
    [SerializeField] private TextMeshProUGUI durationText;

    [Header("Lists")]
    [SerializeField] private List<ChartInfo> charts = new List<ChartInfo>();
    [SerializeField] private List<GameObject> chartPanels = new List<GameObject>();

    public PointerEventData PointerData { get; private set; }

    private int lastDisplayedIndex = -1;


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else Instance = this;
    }

    void Start()
    {
        PointerData = new PointerEventData(eventSystem);

        UpdateCharts();
    }

    public void ClickAtPosition(Vector2 screenPosition)
    {
        PointerData.position = screenPosition;

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(PointerData, results);

        if (results.Count > 0)
        {
            foreach (var result in results)
            {
                GameObject target = result.gameObject;

                try
                {
                    GloveInteractable interact = target.GetComponent<GloveInteractable>();

                    interact.ButtonInteract();
                    Debug.Log("Object " + target.name + " at " + screenPosition + " clicked!");
                }
                catch
                {
                    Debug.Log("Tried to click object " + target.name + " at " + screenPosition + " but it cannot be interacted with.");
                }
            }
        }
        else
            Debug.Log("Tried to click at " + screenPosition);
    }

    public void UpdateCharts()
    {
        foreach (var chartPanel in chartPanels)
        {
            Destroy(chartPanel);
        }
        chartPanels.Clear();

        int index = 0;
        foreach (var chart in charts)
        {
            try
            {
                GameObject _obj = Instantiate(chartPanelPrefab, chartPanelParent);

                ChartPanelUI chartPanelUI = _obj.GetComponent<ChartPanelUI>();
                chartPanelUI.SetChartInfo(chart, index);
                chartPanels.Append(_obj);
            }
            catch
            {
                Debug.LogWarning("ChartPanelPrefab does not set, or cannot found ChartPanelUI script!");
            }
            index++;
        }
    }

    public void ChartButtonClick(int index)
    {
        if (index != lastDisplayedIndex) //havent displayed before
        {
            lastDisplayedIndex = index;
            DisplayChartInfo(charts[index]);
        }
        else //is the chart displaying
        {
            StaticManager.Instance.PlayChart(charts[index]);
        }
    }

    void DisplayChartInfo(ChartInfo chartInfo)
    {
        chartTitleText.text = chartInfo.chartName.ToString();
        songArtistText.text = chartInfo.songArtist.ToString();
        difficultyRatingText.text = chartInfo.difficultyRating.ToString();
        bpmText.text = chartInfo.bpm.ToString();

        string durationTimestamp = SecondsToTimestamp(chartInfo.duration);
        durationText.text = durationTimestamp;

        coverImageDisplay.sprite = chartInfo.coverImage;
    }

    string SecondsToTimestamp(float time)
    {
        int minutes = (int)(Mathf.Floor(time / 60f));
        int seconds = (int)(Mathf.Floor(time % 60f));

        string result = $"{minutes:D2}:{seconds:D2}";

        return result;
    }
}
