using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Score")]
    [SerializeField] private TextMeshProUGUI scoreText;
    private int _displayScore;
    public int displayScore
    {
        get => _displayScore;
        set
        {
            _displayScore = value;
            // Debug.Log("Score : " + _displayScore);
            scoreText.text = ((int)_displayScore).ToString();
        }
    }
    [SerializeField] private float scoreCounterDuration = .7f;

    [Header("Judgment")]
    [SerializeField] private TextMeshProUGUI judgmentText;
    [SerializeField] private float displayDuration = 1f;
    [SerializeField] private float fadeTime = 1f;

    [Header("Combo")]
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private GameObject comboPanel;

    [Header("Health")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Slider healthBar;
    [Space]
    [SerializeField][ColorUsage(true, true)] private Color damageFlashColor;
    [SerializeField] private UnityEngine.UI.Image damageFlashPanel;
    [SerializeField] private float damageFlashDuration;

    [Header("Chart")]
    [SerializeField] private TextMeshProUGUI chartTitleText;
    [SerializeField] private UnityEngine.UI.Slider chartDurationBar;

    [Header("Extra")]
    [SerializeField] private UnityEngine.UI.Image fadeOutPanel;

    private Tween scoreCounterTween;
    private Tween judgmentFadeTween;
    private Tween damageEffectTween;


    private void Awake()
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

    void Start()
    {
        if (judgmentText == null)
        {
            Debug.LogWarning("Judgment Text not set!" + this.gameObject);
        }
        else judgmentText.gameObject.SetActive(false);
    }

    public void ChangeChartTitle()
    {
        chartTitleText.text = GameManager.Instance.noteSpawner.ChartInfo.chartName;
    }

    public void StartDurationBar()
    {
        chartDurationBar.DOValue(1f, GameManager.Instance.noteSpawner.ChartInfo.duration).SetEase(Ease.Linear);
    }

    public void EndTransition(float duration = 1f)
    {
        fadeOutPanel.DOFade(1f, duration);
    }

    public void UpdateScore()
    {
        // Debug.Log("Score updated!");
        UpdateCombo();

        scoreCounterTween.Kill();
        scoreCounterTween = DOTween.To(() => displayScore, x => displayScore = x, GameManager.Instance.GetScore, scoreCounterDuration).SetEase(Ease.OutExpo);
    }

    public void UpdateCombo()
    {
        if (ChartManager.Instance.currentCombo > 5)
        {
            comboPanel.SetActive(true);

            comboText.text = $"x{ChartManager.Instance.currentCombo.ToString()}";
        }
        else
        {
            comboPanel.SetActive(false);
        }
    }

    public void AddJudgment(string text)
    {
        AddJudgment(text, Color.white);
    }

    public void AddJudgment(string text, Color color)
    {
        StopCoroutine("AddJudgmentRoutine");
        StartCoroutine(AddJudgmentRoutine(text, color));
    }

    IEnumerator AddJudgmentRoutine(string text, Color color)
    {
        // judgmentFadeTween.Kill();
        // judgmentText.DOFade(1f, 0f);
        judgmentText.gameObject.SetActive(true);
        judgmentText.text = text;
        judgmentText.color = color;

        yield return new WaitForSeconds(displayDuration);
        // judgmentFadeTween = judgmentText.DOFade(0f, fadeTime);

        // yield return new WaitForSeconds(fadeTime);

        judgmentText.gameObject.SetActive(false);
    }

    public void DamageEffect()
    {
        StartCoroutine(DamageEffectRoutine());
    }

    IEnumerator DamageEffectRoutine()
    {
        damageFlashPanel.color = damageFlashColor;
        damageEffectTween.Kill();

        yield return null;
        damageEffectTween = damageFlashPanel.DOFade(0f, damageFlashDuration).SetEase(Ease.OutExpo);
    }

}
