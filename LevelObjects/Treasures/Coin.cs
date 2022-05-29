using Godot;
using System;

public class Coin : Area
{
    [Export]
    public float GoldValue = 1;
    private float RespawnDelayMin = 10f;
    private float RespawnDelayMax = 60f;
    private bool PickedUp = false;
    private float RotationSpeed;

    private Spatial MainScene;
    private Timer RespawnTimer;
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        MainScene = GetNode("/root/MainScene/") as Spatial;
        RotationSpeed = (float)GD.RandRange(3, 5);
        RespawnTimer = GetNode("RespawnTimer") as Timer;

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        this.RotateY(RotationSpeed * delta);
    }

    private void _on_Coin_body_entered(Node body)
    {
        if (body.GetType().ToString() == "Character" && !PickedUp)
        {
            (body as Character).AddGold(GoldValue);
            PickedUp = true;
            this.Visible = false;
            RespawnTimer.WaitTime = (float)GD.RandRange(RespawnDelayMin, RespawnDelayMax);
            RespawnTimer.Start();
        }

    }

    private void _on_Respawn_timeout()
    {
        if (PickedUp)
        {
            this.Visible = true;
            PickedUp = false;
        }
    }
}
