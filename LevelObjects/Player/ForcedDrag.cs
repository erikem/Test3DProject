using Godot;
using System;

public class ForcedDrag
{
    public Vector3 DragVector;
    public float SpeedModifier = 0;
    public TimeSpan Duration;

    public ForcedDrag(Vector3 dragVector, float speedModifier, TimeSpan duration)
    {
        DragVector = dragVector;
        SpeedModifier = speedModifier;
        Duration = duration;
    }

    public ForcedDrag(Vector3 dragVector, float speedModifier, float duration)
    {
        DragVector = dragVector;
        SpeedModifier = speedModifier;
        Duration = TimeSpan.FromSeconds(duration);
    }
}
