using System.Collections;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class EndscreenUIManager : MonoBehaviour
{
    public static EndscreenUIManager Instance { get; private set; }

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI gradeText;
    [Space]
    [SerializeField] private TextMeshProUGUI chartTitleText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [Space]
    [SerializeField] private TextMeshProUGUI perfectText;
    [SerializeField] private TextMeshProUGUI greatText;
    [SerializeField] private TextMeshProUGUI goodText;
    [SerializeField] private TextMeshProUGUI missText;
    [SerializeField] private TextMeshProUGUI maxComboText;
    [Space]
    [SerializeField] private TextMeshProUGUI highScoreText;

    [Header("Animation")]
    [SerializeField] private RectTransform middlePanel;
    [SerializeField] private RectTransform centerPanel;
    [SerializeField] private RectTransform lowerPanel;
    [Space]
    [SerializeField] private CanvasGroup gradePanel;
    [SerializeField] private CanvasGroup backButtonPanel;
    [Space]
    [SerializeField][Tooltip("Perfect, Great, Good, Miss, Max combo, Highscore panels in that order")] private CanvasGroup[] scorePanels;
    [Space]
    [SerializeField] private float transitionDuration = 2f;
    [SerializeField] private Ease transitionEasing = Ease.OutQuart;

    [Header("Sound FX")]
    [SerializeField] private AudioClip endSFX;

    // [Header("Animation Panels")]
    // [SerializeField] private 

    private string finalGrade = "A";
    private int highScore = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else Instance = this;
    }

    public void DisplayEndscreen()
    {
        try
        {
            string titleName = $"{StaticManager.Instance.chartInfo.songArtist} - {StaticManager.Instance.chartInfo.chartName}"; //merge the title

            chartTitleText.text = titleName;
        }
        catch { }

        CalculateGrade();

        gradeText.text = finalGrade;

        scoreText.text = StaticManager.Instance.finalScore.ToString();

        perfectText.text = StaticManager.Instance.perfectNotes.ToString();
        greatText.text = StaticManager.Instance.greatNotes.ToString();
        goodText.text = StaticManager.Instance.goodNotes.ToString();
        missText.text = StaticManager.Instance.missNotes.ToString();
        maxComboText.text = StaticManager.Instance.maxCombo.ToString();

        CalculateHighScore();
        highScoreText.text = highScore.ToString();

        AnimateEndscreen();

        AudioManager.Instance.PlaySfx(endSFX);
    }

    [ContextMenu("Animate Endscreen")]
    public void AnimateEndscreen()
    {
        StartCoroutine(AnimateEndscreenRoutine());
    }

    IEnumerator AnimateEndscreenRoutine()
    {
        //setup
        Vector2 initLowerPanelPos = lowerPanel.transform.localPosition;
        Vector2 initCenterPanelPos = centerPanel.transform.localPosition;

        middlePanel.DOScaleY(5f, 0f);
        centerPanel.DOLocalMoveX(1920f, 0f);
        centerPanel.DOLocalRotate(new Vector3(0f, 0f, 90f), 0f);
        lowerPanel.DOLocalMoveY(-500f, 0f);

        gradePanel.DOFade(0f, 0f);
        backButtonPanel.DOFade(0f, 0f);

        foreach (var panel in scorePanels)
        {
            panel.DOFade(0f, 0f);
        }

        yield return null;

        //animate
        middlePanel.DOScaleY(1f, transitionDuration / 2f).SetEase(transitionEasing);
        centerPanel.DOLocalMove(initCenterPanelPos, transitionDuration).SetEase(transitionEasing);
        centerPanel.DOLocalRotate(new Vector3(0f, 0f, 0f), transitionDuration).SetEase(transitionEasing);

        yield return new WaitForSeconds(transitionDuration / 2f);
        lowerPanel.DOLocalMove(initLowerPanelPos, transitionDuration / 2f).SetEase(transitionEasing);
        gradePanel.DOFade(1f, transitionDuration / 2f);
        backButtonPanel.DOFade(1f, transitionDuration / 2f);

        float delays = (transitionDuration/2f) /scorePanels.Count();
        foreach(var panel in scorePanels)
        {
            panel.DOFade(1f, transitionDuration/2f);
            yield return new WaitForSeconds(delays);
        }
    }

    void CalculateHighScore()
    {
        highScore = PlayerPrefs.GetInt("highscore", 0);

        int finalScore = StaticManager.Instance.finalScore;
        if (highScore < finalScore)
        {
            PlayerPrefs.SetInt("highscore", finalScore);
            highScore = finalScore;
        }
    }

    void CalculateGrade()
    {
        finalGrade = StaticManager.Instance.finalScore switch
        {
            >= 999999 => "S+",
            > 900000 => "S",
            > 800000 => "A",
            > 700000 => "B",
            > 500000 => "C",
            _ => "D"
        };
    }

    public void GoBackToSelectionScene()
    {
        StaticManager.Instance.LoadScene("SelectionScreen");
    }
}