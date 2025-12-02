using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChartPanelUI : MonoBehaviour
{
    public int index = 0;

    [Header("Setup")]
    [SerializeField] private Button chartButton;
    [Space]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI artistText;

    [Header("Stats")]
    [SerializeField] private ChartInfo chartInfo;
    private string titleName {
        get { return titleName; }
        set
        {
            titleName = titleText.text = value;
        }
    }
    private string artistName {
        get { return artistName; }
        set
        {
            artistName = titleText.text = value;
        }
    }

    public void SetChartInfo(ChartInfo chartInfo, int index = 0)
    {
        this.chartInfo = chartInfo;
        this.index = index;

        titleName = chartInfo.chartName;
        artistName = chartInfo.songArtist;
    }

    public void Callback()
    {
        MenuUIManager.Instance.ChartButtonClick(index);
    }
}
