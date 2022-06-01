using Godot;
using System;

public class GeneratedScene : Spatial
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

        var SunScene = GD.Load<PackedScene>("res://LevelObjects/MainSunLight.tscn");
        var SunInstance = SunScene.Instance();
        AddChild(SunInstance);
        SunInstance.Owner = this;

        CanvasLayer UICanvas = new CanvasLayer();
        UICanvas.Name = "UICanvas";
        AddChild(UICanvas);
        UICanvas.Owner = this;

        var UIScene = GD.Load<PackedScene>("res://UI/UI.tscn");
        var UIInstance = UIScene.Instance();
        UIInstance.Name = "UI";
        UICanvas.AddChild(UIInstance);
        UIInstance.Owner = this;

        Spatial Enemies = new Spatial();
        Enemies.Name = "Enemies";
        AddChild(Enemies);
        Enemies.Owner = this;

        Spatial Props = new Spatial();
        Props.Name = "Props";
        AddChild(Props);
        Props.Owner = this;

        Spatial Ground = new Spatial();
        Ground.Name = "Ground";
        AddChild(Ground);
        Ground.Owner = this;
        var FloorScene = GD.Load<PackedScene>("res://LevelObjects/FloorsAndWalls/FloorTile001_Normalized.tscn");
        var PassablePropScene = GD.Load<PackedScene>("res://LevelObjects/Props/Flower001_Normalized.tscn");
        for (int i = -2; i < 2; i++)
        {
            for (int j = -2; j < 2; j++)
            {
                Spatial FloorTile = FloorScene.Instance() as Spatial;
                AddChild(FloorTile);
                FloorTile.Owner = this;
                FloorTile.Translate(new Vector3(i, 0, j));

                var PassableProp = PassablePropScene.Instance() as PassableProp;
                PassableProp.Init(this, Props, i, j);

            }

        }


        var PlayerScene = GD.Load<PackedScene>("res://LevelObjects/Player/Character001_Normalized.tscn");
        var PlayerInstance = PlayerScene.Instance();
        PlayerInstance.Name = "Character001_Normalized";
        AddChild(PlayerInstance);
        PlayerInstance.Owner = this;
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
