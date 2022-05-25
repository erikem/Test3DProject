using Godot;
using System;

public class CameraController : Spatial
{
    private float lookSensitivity = 15.0f;
    private float minLookAngle = -25f;
    private float maxLookAngle = 5;
    private Vector2 mouseDelta;
    private KinematicBody player;
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        player = GetParent() as KinematicBody;
        Input.SetMouseMode(Input.MouseMode.Captured);
    }

    public override void _Input(InputEvent MouseEvent)
    {
        base._Input(MouseEvent);
        if (MouseEvent is InputEventMouseMotion
        //&& Input.IsActionPressed("RotateCameraHoldKey")
        )
        {
            mouseDelta = ((InputEventMouseMotion)MouseEvent).Relative;
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        Vector3 rot = new Vector3(mouseDelta.y, mouseDelta.x, 0) * lookSensitivity * delta;


        //this.RotationDegrees = new Vector3(Mathf.Clamp(this.RotationDegrees.x + rot.x, minLookAngle, maxLookAngle), this.RotationDegrees.y + rot.y, this.RotationDegrees.z);
        this.RotationDegrees = new Vector3(Mathf.Clamp(this.RotationDegrees.x + rot.x, minLookAngle, maxLookAngle), this.RotationDegrees.y, this.RotationDegrees.z);
        player.RotationDegrees = new Vector3(player.RotationDegrees.x, player.RotationDegrees.y - rot.y, player.RotationDegrees.z);
        mouseDelta = Vector2.Zero;

    }
}
