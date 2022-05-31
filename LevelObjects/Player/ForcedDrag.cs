using Godot;
using System;

public class ForcedDrag
{
    public Vector3 DragVector;
    public float SpeedModifier = 0;
    public TimeSpan Duration;
    public string Name = "";

    public ForcedDrag(Vector3 dragVector, float speedModifier, TimeSpan duration, string name)
    {
        DragVector = dragVector;
        SpeedModifier = speedModifier;
        Duration = duration;
        Name = name;
    }

    public ForcedDrag(Vector3 dragVector, float speedModifier, float duration, string name)
    {
        DragVector = dragVector;
        SpeedModifier = speedModifier;
        Duration = TimeSpan.FromSeconds(duration);
        Name = name;
    }
}
