using Unity.Mathematics;
// using UnityEditor.EditorTools;
using UnityEngine;


public class NoteArrow : NoteAbstract
{
    [Header("Directional Note")]
    [SerializeField] private float directionTolerance = 45f;

    protected override void DebugInput() //debug purposes only
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        float dir;
        if (horizontalInput > 0 && verticalInput == 0)
        {
            dir = 270f;
        }
        else if (horizontalInput < 0 && verticalInput == 0)
        {
            dir = 90f;
        }
        else if (horizontalInput == 0 && verticalInput > 0)
        {
            dir = 0f;
        }
        else if (horizontalInput == 0 && verticalInput < 0)
        {
            dir = 180f;
        }
        else return;

        if (elapsedTime >= duration - judgmentMiss && IsOnDirection(dir))
        {
            Hit();
        }
    }

    protected override void SpawnNote()
    {
        this.transform.localPosition = note.targetPos;

        noteObject.transform.localPosition = new Vector2(0, ChartManager.Instance.noteSpeed * duration);
        sprite.transform.rotation = Quaternion.Euler(0, 0, note.dir);
        noteTarget.transform.rotation = Quaternion.Euler(0, 0, note.dir);

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (elapsedTime >= duration - judgmentMiss)
        {
            if (col.gameObject.CompareTag(GameManager.Instance.LeftGloveTag) && IsOnDirection(GameManager.Instance.leftGlove.moveDir))
            {
                Hit();
            }
            else if (col.gameObject.CompareTag(GameManager.Instance.RightGloveTag) && IsOnDirection(GameManager.Instance.rightGlove.moveDir))
            {
                Hit();
            }
        }
    }

    bool IsOnDirection(float dir)
    {
        if (dir > note.dir + directionTolerance && dir < 360 + note.dir - directionTolerance || dir < note.dir - directionTolerance && dir > -note.dir + directionTolerance)
        {
            return false;
        }
        else return true;
    }
}