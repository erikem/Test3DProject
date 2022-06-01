using System;

public class Flower001_Normalized : PassableProp
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _complimentaryProps.Add("res://LevelObjects/Props/Flower002_Normalized.tscn");
        _complimentaryProps.Add("res://LevelObjects/Props/Flower003_Normalized.tscn");
        _complimentaryProps.Add("res://LevelObjects/Props/Grass001_Normalized.tscn");
        _complimentaryProps.Add("res://LevelObjects/Props/Grass002_Normalized.tscn");
        _complimentaryProps.Add("res://LevelObjects/Props/Grass003_Normalized.tscn");
        _complimentaryProps.Add("res://LevelObjects/Props/Rock001_Normalized.tscn");
        base._CustomReady();
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
