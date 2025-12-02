using UnityEngine;

public class NoteTap : NoteAbstract
{
    protected override void DebugInput() //debug purposes only
    {
        if (Input.GetKeyDown(KeyCode.Space) && elapsedTime >= duration - judgmentMiss)
        {
            Hit();
        }
    }

    protected override void SpawnNote()
    {
        this.transform.localPosition = note.targetPos;

        noteObject.transform.localPosition = new Vector2(0, ChartManager.Instance.noteSpeed * duration);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (elapsedTime >= duration - judgmentMiss)
        {
            Hit();
        }
    }
}