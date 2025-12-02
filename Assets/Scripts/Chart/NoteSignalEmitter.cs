using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class NoteSignalEmitter : Marker, INotification
{
    public PropertyName id { get; } = new PropertyName();
    
    public Note[] notes;
}
