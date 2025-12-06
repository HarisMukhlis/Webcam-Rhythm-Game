using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Timeline;

public class NoteSpawner : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Transform noteParent;
    [Space]
    [SerializeField] private ChartInfo _chartInfo;
    public ChartInfo ChartInfo => _chartInfo;

    [Header("Settings")]
    [SerializeField] private float startDelay;
    [SerializeField] private float transitionDuration = 1f;

    [Header("Prefabs")]
    [SerializeField] private GameObject noteArrowPrefab;
    [SerializeField] private GameObject noteTapPrefab;

    public List<Note> Notes { get; private set; } = new List<Note>();

    [Header("Events")]
    [Tooltip("Called when chart is fully loaded")] public UnityEvent onChartLoaded;
    [Tooltip("Called when chart has been started")] public UnityEvent onChartStart;
    [Tooltip("Called when the chart's duration has ended")] public UnityEvent onChartEnded;

    private float elapsedTime = 0f;
    private bool isStarted = false;

    void Start()
    {
        // LoadChart();
        LoadTimeline(_chartInfo);

        ChartManager.Instance.CalculateTotalNotes();
        BeginChart();
    }

    void Update()
    {
        if (isStarted)
        {
            elapsedTime += Time.deltaTime;

        }
        if (elapsedTime >= _chartInfo.duration)
        {
            StopAllCoroutines();
            UIManager.Instance.EndTransition(transitionDuration);

            if (elapsedTime >= _chartInfo.duration + transitionDuration)
            {
                StaticManager.Instance.EndChart(ChartManager.Instance);
            }
            // EndChart();
        }
    }

    public void LoadTimeline(ChartInfo _chartInfo)
    {
        this._chartInfo = _chartInfo;

        Notes.Clear();

        foreach (var track in _chartInfo.chartTimeline.GetOutputTracks())
        {
            if (track is SignalTrack signalTrack)
            {
                foreach (var marker in signalTrack.GetMarkers())
                {
                    NoteSignalEmitter noteMarker = marker as NoteSignalEmitter;

                    if (noteMarker == null)
                    {
                        Debug.LogWarning("Note Marker at " + marker.time + " has not set");
                    }
                    else
                    {
                        foreach (var note in noteMarker.notes)
                        {
                            Note _note = new Note(note); //temp note

                            _note.delay = CalculateFromBPM((float)marker.time);

                            // Debug.Log("Note added at " + _time + " from " + marker.time + ", previous time " + prevTime);

                            Notes.Add(_note);
                        }
                    }
                }
            }
        }

        List<Note> sortedNotes = Notes.OrderBy(i => i.delay).ToList();
        Notes = sortedNotes;

        float prevTime = 0f;
        foreach (var note in Notes)
        {
            float _time = note.delay;
            note.delay -= prevTime;
            prevTime = _time;
        }

        onChartLoaded.Invoke();
    }

    void EndChart()
    {
        StopAllCoroutines();
        StaticManager.Instance.EndChart(ChartManager.Instance);
        StartCoroutine(EndChartRoutine());
    }

    IEnumerator EndChartRoutine()
    {
        UIManager.Instance.EndTransition(transitionDuration);

        yield return new WaitForSeconds(transitionDuration);
    }

    public void BeginChart(float delay = -1f)
    {
        StopAllCoroutines();
        StartCoroutine(BeginChartRoutine((delay < 0f) ? startDelay : delay));
    }

    IEnumerator BeginChartRoutine(float delay)
    {
        AudioManager.Instance.PreloadMusic(_chartInfo.musicClip);
        yield return new WaitForSeconds(delay - GameManager.Instance.noteDelay);

        Debug.Log("Chart " + _chartInfo.name + " Started!");
        onChartStart.Invoke();
        isStarted = true;

        StartCoroutine(SpawnNotesRoutine());
        
        yield return new WaitForSeconds(GameManager.Instance.noteDelay + GameManager.Instance.latency);

        StartMusic();
    }

    void StartMusic()
    {
        if (_chartInfo.musicClip != null)
            AudioManager.Instance.PlayPreloadedMusic();
        else Debug.LogWarning("Music of " + _chartInfo.name + " is empty");        
    }

    IEnumerator SpawnNotesRoutine()
    {
        foreach (var note in Notes)
        {
            yield return new WaitForSeconds(note.delay);

            SpawnNote(note);
        }        
    }

    void SpawnNote(Note note)
    {
        try
        {
            GameObject _obj = note.noteType switch
            {
                NoteType.Tap => Instantiate(noteTapPrefab, noteParent),
                NoteType.Arrow => Instantiate(noteArrowPrefab, noteParent),
                _ => null
            };

            NoteAbstract noteAbstract = _obj.GetComponent<NoteAbstract>();
            noteAbstract.note = note;
        }
        catch
        {
            Debug.LogWarning("Note " + note + " is not valid!");
        }
    }

    float CalculateFromBPM(float input)
    {
        return CalculateFromBPM(input, _chartInfo.bpm);
    }

    float CalculateFromBPM(float input, float bpm) //converts beat value to seconds, based on the bpm
    {
        float time = input * (60f / bpm);
        return time;
    }
}
