using Godot;
using System;
using System.Collections.Generic;

public class GeneratedScene : Spatial
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    private static Random _rnd = new Random();
    private Node _sun;
    private string _sunTreePath = "res://LevelObjects/MainSunLight.tscn";
    private CanvasLayer _uiCanvas = new CanvasLayer();
    private string _uiCanvasName = "UICanvas";
    private Node _ui;
    private string _uiTreePath = "res://UI/UI.tscn";
    private string _uiName = "UI";
    private Spatial _enemiesContainer = new Spatial();
    private string _enemiesContainerName = "Enemies";
    private Spatial _propsContainer = new Spatial();
    private string _propsContainerName = "Props";
    private Spatial _groundContainer = new Spatial();
    private string _groundContainerName = "Ground";
    private Spatial _treasureContainer = new Spatial();
    private string _treasureContainerName = "Treasure";
    private PackedScene _groundScene = GD.Load<PackedScene>("res://LevelObjects/FloorsAndWalls/FloorTile001_Normalized.tscn");
    private List<PackedScene> _passablePropsScenes = new List<PackedScene>
    {
        GD.Load<PackedScene>("res://LevelObjects/Props/Flower001_Normalized.tscn"),
        GD.Load<PackedScene>("res://LevelObjects/Props/Flower002_Normalized.tscn"),
        GD.Load<PackedScene>("res://LevelObjects/Props/Flower003_Normalized.tscn"),
        GD.Load<PackedScene>("res://LevelObjects/Props/Grass001_Normalized.tscn"),
        GD.Load<PackedScene>("res://LevelObjects/Props/Grass002_Normalized.tscn"),
        GD.Load<PackedScene>("res://LevelObjects/Props/Grass003_Normalized.tscn"),
        GD.Load<PackedScene>("res://LevelObjects/Props/Rock001_Normalized.tscn")
    };
    private Spatial _playerInstance;
    private string _playerTreePath = "res://LevelObjects/Player/Character001_Normalized.tscn";
    private string _playerNodeName = "Character001_Normalized";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GenerateSun();
        GenerateUI();
        GenerateContainers();
        for (int i = -2; i < 2; i++)
        {
            for (int j = -2; j < 2; j++)
            {
                PlaceFloorTile(i, j);
                PlacePassableProp(i, j);
            }
        }
        PlacePlayer(1, 1);
    }

    private void PlacePlayer(float x, float z)
    {
        var PlayerScene = GD.Load<PackedScene>(_playerTreePath);
        _playerInstance = PlayerScene.Instance() as Spatial;
        _playerInstance.Name = _playerNodeName;
        AddChild(_playerInstance);
        _playerInstance.Owner = this;
        _playerInstance.Translate(new Vector3(x, 0, z));
    }

    private void PlacePassableProp(int x, int z)
    {
        var PassableProp = _passablePropsScenes[_rnd.Next(_passablePropsScenes.Count)].Instance() as PassableProp;
        PassableProp.Init(this, _propsContainer, x, z);
    }

    private void PlaceFloorTile(int x, int z)
    {
        Spatial FloorTile = _groundScene.Instance() as Spatial;
        _groundContainer.AddChild(FloorTile);
        FloorTile.Owner = this;
        FloorTile.Translate(new Vector3(x, 0, z));
    }

    private void GenerateContainers()
    {
        _enemiesContainer.Name = _enemiesContainerName;
        AddChild(_enemiesContainer);
        _enemiesContainer.Owner = this;

        _propsContainer.Name = _propsContainerName;
        AddChild(_propsContainer);
        _propsContainer.Owner = this;

        _groundContainer.Name = _groundContainerName;
        AddChild(_groundContainer);
        _groundContainer.Owner = this;

        _treasureContainer.Name = _treasureContainerName;
        AddChild(_treasureContainer);
        _treasureContainer.Owner = this;
    }

    private void GenerateUI()
    {
        _uiCanvas.Name = _uiCanvasName;
        AddChild(_uiCanvas);
        _uiCanvas.Owner = this;
        var UIScene = GD.Load<PackedScene>(_uiTreePath);
        _ui = UIScene.Instance();
        _ui.Name = _uiName;
        _uiCanvas.AddChild(_ui);
        _ui.Owner = this;
    }

    private void GenerateSun()
    {
        var SunScene = GD.Load<PackedScene>(_sunTreePath);
        _sun = SunScene.Instance();
        AddChild(_sun);
        _sun.Owner = this;
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
