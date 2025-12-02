using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Timeline;

[CreateAssetMenu(menuName = "BEATTOUCHER/Chart", fileName = "New Chart")]
public class ChartInfo : ScriptableObject
{
    [Header("Audio")]
    public AudioClip musicClip;

    [Header("Basic Info")]
    public string chartName = "Chart Name";
    public string songArtist = "Artist Name";
    public Sprite coverImage;
    [Tooltip("Name of the original chart designer")]public string charterArtist = "";
    [Space]
    [Tooltip("Difficulty star rating. Normally ranges from 1 - 10, but can be any range")] public int difficultyRating;
    [Tooltip("Beats per Minute. Do NOT set to 0 or below")][FormerlySerializedAs("BPM")]public float bpm = 60f;
    [Tooltip("Duration in total seconds")] public float duration = 1f;

    [Header("Timeline")]
    public TimelineAsset chartTimeline;

    public ChartInfo() {}

    public ChartInfo(ChartInfo chartInfo)
    {
        this.musicClip = chartInfo.musicClip;
        this.chartName = chartInfo.chartName;
        this.songArtist = chartInfo.songArtist;
        this.coverImage = chartInfo.coverImage;
        this.charterArtist = chartInfo.charterArtist;
        this.difficultyRating = chartInfo.difficultyRating;
        this.bpm = chartInfo.bpm;
        this.duration = chartInfo.duration;
        this.chartTimeline = chartInfo.chartTimeline;
    }
}
