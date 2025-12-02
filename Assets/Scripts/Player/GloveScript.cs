using UnityEngine;

public enum InputType
{
    LeftWrist,
    RightWrist
}

public class GloveScript : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private GameObject sprite;
    [SerializeField] private InputType inputType;

    [Header("Smoothing")]
    [SerializeField] private float posSmoothness = 0.4f;
    [SerializeField] private float angleSmoothness = 0.2f;
    [Space]
    [SerializeField][Tooltip("Minimum movement for direction")] private float movementDeadZone = 0.1f;

    [Header("UI Interaction")]
    [SerializeField][Tooltip("If on, this can interact with the UI when holding position")] private bool uiInteractable = false;
    [SerializeField][Tooltip("Holding time before sending as a \"Click\"")] private float interactionDuration = 1f;
    [SerializeField][Tooltip("Minimum movement before cancelling a click")] private float interactionDeadZone = 0.8f;
    public float moveDir { get; private set; } = 0f;

    private Vector2 smoothPos;
    private float holdingDuration = 0f;
    private Message pythonMsg;

    void Update()
    {
        if (PythonReceiverManager.Instance != null)
            pythonMsg = PythonReceiverManager.Instance.LastMessage;

        SmoothenPos();

        if (uiInteractable)
        {
            CheckInteract();
        }

        MoveGlove();
    }

    void CheckInteract()
    {
        if (Vector2.Distance(transform.position, smoothPos) >= interactionDeadZone)
        {
            holdingDuration = 0f;
            return;
        }
        else
        {
            holdingDuration += Time.deltaTime;
        }

        if (holdingDuration >= interactionDuration)
        {
            holdingDuration = 0f;

            Vector2 pixelPos = (inputType == InputType.LeftWrist) ? pythonMsg.LWristPosPixel : pythonMsg.RWristPosPixel;
            try
            {
                MenuUIManager.Instance.ClickAtPosition(pixelPos);
            }
            catch { }
        }
    }

    void MoveGlove()
    {
        if ((pythonMsg.left_vis && inputType == InputType.LeftWrist) || (pythonMsg.right_vis && inputType == InputType.RightWrist))
        {
            sprite.SetActive(true);

            Vector2 newPos = smoothPos;

            if (Vector2.Distance(transform.position, newPos) >= movementDeadZone)
            {
                float rawAngle = VectorDirection(transform.position, newPos);
                moveDir = Mathf.LerpAngle(moveDir, rawAngle, angleSmoothness);
            }

            transform.position = newPos;
            transform.rotation = Quaternion.Euler(0f, 0f, moveDir);
        }
        else sprite.SetActive(false);
    }

    void SmoothenPos()
    {
        Vector2 targetPos = (inputType == InputType.LeftWrist) ? pythonMsg.LWristPos : pythonMsg.RWristPos;
        smoothPos = Vector2.Lerp(smoothPos, targetPos, posSmoothness);
    }

    float VectorDirection(Vector2 oldPos, Vector2 newPos)
    {
        Vector2 diff = newPos - oldPos;
        float deg = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        return (deg < 0) ? deg + 360f : deg;
    }
}
