using System.Collections;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

public class NoteVisual : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private GameObject noteObject;
    [SerializeField] private GameObject targetObject;

    [Header("Zone Indicator")]
    [SerializeField] private Ease indicatorEasing = Ease.InQuad;
    [SerializeField] private float indicatorGlowAmount = .2f;

    [Header("Flash")]
    [SerializeField] private float flashDuration;
    [SerializeField] private Ease hitEasing = Ease.InExpo;
    [SerializeField] private Ease missEasing = Ease.InBack;
    [Space]
    [SerializeField] private float noteHitScale;
    [SerializeField] private float noteFalloff = .7f;
    [Space]
    [SerializeField] private float noteMissRotationRand = 70f; //randomness of the rotation
    [Space]
    [SerializeField][ColorUsage(true, true)] private Color hitFlashColor;
    [SerializeField][ColorUsage(true, true)] private Color missColor = Color.red;
    // [SerializeField][ColorUsage(true, true)] private Color missFlashColor;

    private SpriteRenderer noteRenderer;
    private SpriteRenderer targetRenderer;

    private Material noteMaterial;
    private Material targetMaterial;

    private int flashAmount;
    private int flashColor;

    void Awake()
    {
        if (noteObject != null)
        {
            noteRenderer = noteObject.GetComponent<SpriteRenderer>();
            noteMaterial = noteRenderer.material;
        }
        else Debug.LogWarning("Note Object at " + gameObject.name + " has not been set");

        if (targetObject != null)
        {
            targetRenderer = targetObject.GetComponent<SpriteRenderer>();
            targetMaterial = targetRenderer.material;
        }
        else Debug.LogWarning("Target Object at " + gameObject.name + " has not been set");

        flashAmount = Shader.PropertyToID("_FlashIntensity");
        flashColor = Shader.PropertyToID("_FlashColor");
    }

    public void NoteSpawn()
    {
        StartCoroutine(NoteSpawnRoutine());
    }

    IEnumerator NoteSpawnRoutine()
    {
        targetRenderer.color -= new Color(0f, 0f, 0f, 1f);

        yield return null;
        targetRenderer.DOFade(1f, flashDuration);
    }

    public void NoteEnterZone(float delay)
    {
        StartCoroutine(NoteEnterZoneRoutine(delay));
    }

    IEnumerator NoteEnterZoneRoutine(float delay)
    {
        targetRenderer.sortingOrder++; //so its visible when indicating
        targetMaterial.DOFloat(indicatorGlowAmount, flashAmount, delay).SetEase(indicatorEasing);

        yield return new WaitForSeconds(delay);

        targetMaterial.SetFloat(flashAmount, 0f);
    }

    public void NoteHitTap()
    {
        StartCoroutine(NoteHitTapRoutine());
    }

    IEnumerator NoteHitTapRoutine()
    {
        noteMaterial.SetColor(flashColor, hitFlashColor);
        noteMaterial.SetFloat(flashAmount, 1f);

        yield return null;
        noteMaterial.DOFloat(0f, flashAmount, flashDuration/2f).SetEase(hitEasing);
        noteObject.transform.DOPunchScale(new Vector3(noteHitScale, noteHitScale), flashDuration / 2f, 4);
        targetRenderer.DOFade(0f, flashDuration);

        yield return new WaitForSeconds(flashDuration / 2f);
        noteObject.transform.DOScale(0f, flashDuration / 2f).SetEase(hitEasing);

        yield return new WaitForSeconds(flashDuration / 2f);

        Destroy(this.gameObject);
    }

    public void NoteHitArrow()
    {
        StartCoroutine(NoteHitArrowRoutine());
    }

    IEnumerator NoteHitArrowRoutine()
    {
        noteMaterial.SetColor(flashColor, hitFlashColor);
        noteMaterial.SetFloat(flashAmount, 1f);

        yield return null;
        noteObject.transform.DOLocalMove(noteObject.transform.up * noteFalloff, flashDuration).SetEase(hitEasing);
        noteMaterial.DOFloat(0f, flashAmount, flashDuration).SetEase(hitEasing);
        noteRenderer.DOFade(0f, flashDuration);
        targetRenderer.DOFade(0f, flashDuration);

        yield return new WaitForSeconds(flashDuration);

        Destroy(this.gameObject);
    }

    public void NoteMiss()
    {
        StartCoroutine(NoteMissRoutine());
    }

    IEnumerator NoteMissRoutine()
    {
        noteRenderer.color = missColor;

        yield return null;
        noteRenderer.DOFade(0f, flashDuration);
        noteObject.transform.DOMoveY(noteObject.transform.position.y - noteFalloff, flashDuration).SetEase(missEasing);
        noteObject.transform.DORotate(new Vector3(0f, 0f, noteObject.transform.rotation.z + UnityEngine.Random.Range(-noteMissRotationRand, noteMissRotationRand)), flashDuration).SetEase(missEasing);
        targetRenderer.DOFade(0f, flashDuration);

        yield return new WaitForSeconds(flashDuration);

        Destroy(this.gameObject);
    }
}