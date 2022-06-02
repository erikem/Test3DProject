using Godot;
using System;
using System.Collections.Generic;

public class GeneratedScene : Spatial
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    private static Random _rnd = new Random();
    private int _mapSize = 5;
    private float _groundAreaPercentage = 0.3f;
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
    private Spatial _wallContainer = new Spatial();
    private string _wallContainerName = "Walls";
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
    private PackedScene _enemyScene = GD.Load<PackedScene>("res://LevelObjects/Enemies/EnemyNormal001_Normalized.tscn");
    private PackedScene _coinScene = GD.Load<PackedScene>("res://LevelObjects/Treasures/Coin001_Normalized.tscn");
    private PackedScene _frontWallScene = GD.Load<PackedScene>("res://LevelObjects/FloorsAndWalls/Wall001_Normalized.tscn");
    private PackedScene _backWallScene = GD.Load<PackedScene>("res://LevelObjects/FloorsAndWalls/WallTransparent001_Normalized.tscn");
    private PackedScene _lavaFloorScene = GD.Load<PackedScene>("res://LevelObjects/FloorsAndWalls/LavaFloor001_Normalized.tscn");
    private PackedScene _treePillarScene = GD.Load<PackedScene>("res://LevelObjects/Props/Tree001_Normalized.tscn");

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        PopulateBasicTemplatesForProceduralGen();
        MapGenerator GameMap = new MapGenerator(_mapSize, true, _groundAreaPercentage,
                0.01f, 20, 40,
                0.01f, 4, 8,
                0.01f, 10, 15,
                1, "MaxFreeArea", false, 100);

        GenerateSun();
        GenerateUI();
        GenerateContainers();
        GenerateBoundingWalls();

        for (int i = 0; i < _mapSize; i++)
        {
            for (int j = 0; j < _mapSize; j++)
            {
                PlaceGenericTile(i, j, "Floor");
                PlacePassableProp(i, j);
            }
        }
        PlaceGenericTile(3, 3, "FrontWall");
        PlaceGenericTile(3, 2, "BackWall");
        PlaceGenericTile(2, 3, "LavaFloor");
        PlaceTree(1, 3);
        PlaceCoin(0, 0);
        PlacePlayer(0, 0);
        //PlaceEnemy(1, 1);

    }

    private static void PopulateBasicTemplatesForProceduralGen()
    {
        MapTemplate.AllTemplates.Clear();
        MapTemplate.AddTemplate(
            new float[7, 7]
            {
                {0,0,1,1,1,0,0},
                {0,0,1,1,1,0,0},
                {1,1,1,1,1,1,1},
                {1,1,1,1,1,1,1},
                {1,1,1,1,1,1,1},
                {0,0,1,1,1,0,0},
                {0,0,1,1,1,0,0}
            },
            new Coordinates2D(0, 2),
            new Coordinates2D(0, 4),
            new Coordinates2D(6, 2),
            new Coordinates2D(6, 4));

        MapTemplate.AddTemplate(
             new float[7, 7]
             {
                 {0,0,1,1,1,0,0},
                 {0,0,1,1,1,0,0},
                 {1,1,1,1,1,1,1},
                 {1,1,1,1,1,1,1},
                 {1,1,1,1,1,1,1},
                 {0,0,1,1,1,0,0},
                 {0,0,1,1,1,0,0}
             },
             new Coordinates2D(2, 0),
             new Coordinates2D(2, 6),
             new Coordinates2D(4, 0),
             new Coordinates2D(4, 6));

        MapTemplate.AddTemplate(
            new float[7, 7]
            {
                {0,0,1,1,1,0,0},
                {0,0,1,1,1,0,0},
                {1,1,1,1,1,1,1},
                {1,1,1,1,1,1,1},
                {1,1,1,1,1,1,1},
                {0,0,1,1,1,0,0},
                {0,0,1,1,1,0,0}
            },
            new Coordinates2D(3, 0),
            new Coordinates2D(0, 3),
            new Coordinates2D(6, 3),
            new Coordinates2D(3, 6));

        MapTemplate.AddTemplate(
            new float[6, 6]
            {
                {0,0,1,1,0,0},
                {0,0,1,1,0,0},
                {1,1,1,1,1,1},
                {1,1,1,1,1,1},
                {0,0,1,1,0,0},
                {0,0,1,1,0,0}
            },
            new Coordinates2D(0, 2),
            new Coordinates2D(0, 3),
            new Coordinates2D(5, 2),
            new Coordinates2D(5, 3));

        MapTemplate.AddTemplate(
            new float[6, 6]
            {
                {1,1,1,1,1,1},
                {1,1,1,1,1,1},
                {0,0,0,0,1,1},
                {0,0,0,0,1,1},
                {0,0,0,0,1,1},
                {0,0,0,0,1,1}
            },
            new Coordinates2D(0, 5),
            new Coordinates2D(0, 5),
            new Coordinates2D(5, 4),
            new Coordinates2D(5, 5));

        MapTemplate.AddTemplate(
            new float[6, 6]
            {
                {1,1,0,0,0,0},
                {1,1,0,0,0,0},
                {1,1,0,0,0,0},
                {1,1,0,0,0,0},
                {1,1,1,1,1,1},
                {1,1,1,1,1,1}
            },
            new Coordinates2D(0, 0),
            new Coordinates2D(0, 1),
            new Coordinates2D(5, 0),
            new Coordinates2D(5, 5));


        MapTemplate.AddTemplate(
            new float[5, 5]
            {
                {0,1,1,1,0},
                {1,1,1,1,1},
                {1,1,1,1,1},
                {1,1,1,1,1},
                {0,1,1,1,0}
            },
            new Coordinates2D(0, 1),
            new Coordinates2D(0, 3),
            new Coordinates2D(3, 0),
            new Coordinates2D(3, 3));

        MapTemplate.AddTemplate(
            new float[5, 5]
            {
                {0,1,1,1,0},
                {1,1,1,1,1},
                {1,1,1,1,1},
                {1,1,1,1,1},
                {0,1,1,1,0}
            },
            new Coordinates2D(2, 0),
            new Coordinates2D(0, 2),
            new Coordinates2D(4, 2),
            new Coordinates2D(2, 4));

        MapTemplate.AddTemplate(
            new float[5, 5]
            {
                {0,0,1,0,0},
                {0,0,1,0,0},
                {1,1,1,1,1},
                {0,0,1,0,0},
                {0,0,1,0,0}
            },
            new Coordinates2D(2, 0),
            new Coordinates2D(0, 2),
            new Coordinates2D(4, 2),
            new Coordinates2D(2, 4));
    }

    private void GenerateBoundingWalls()
    {
        PlaceGenericTile(-1, -1, "BackWall");
        PlaceGenericTile(_mapSize, _mapSize, "FrontWall");
        PlaceGenericTile(-1, _mapSize, "FrontWall");
        PlaceGenericTile(_mapSize, -1, "FrontWall");
        for (int i = 0; i < _mapSize; i++)
        {
            PlaceGenericTile(-1, i, "BackWall");
            PlaceGenericTile(i, -1, "BackWall");
            PlaceGenericTile(_mapSize, i, "FrontWall");
            PlaceGenericTile(i, _mapSize, "FrontWall");
        }
    }

    private void PlacePlayer(float x, float z)
    {
        var PlayerScene = GD.Load<PackedScene>(_playerTreePath);
        _playerInstance = PlayerScene.Instance() as Spatial;
        _playerInstance.Name = _playerNodeName;
        AddChild(_playerInstance);
        _playerInstance.Owner = this;
        _playerInstance.Translate(new Vector3(x + 0.5f, 0, z + 0.5f));
    }

    private void PlaceEnemy(float x, float z)
    {
        Spatial _enemyInstance = _enemyScene.Instance() as Spatial;
        _enemiesContainer.AddChild(_enemyInstance);
        _enemyInstance.Owner = this;
        _enemyInstance.Translate(new Vector3(x + 0.5f, 0, z + 0.5f));
    }

    private void PlacePassableProp(int x, int z)
    {
        var PassableProp = _passablePropsScenes[_rnd.Next(_passablePropsScenes.Count)].Instance() as PassableProp;
        PassableProp.Init(this, _propsContainer, x, z);
    }

    private bool PlaceGenericTile(int x, int z, string type)
    {
        Spatial GenericTile;
        switch (type)
        {
            case "Floor":
                GenericTile = _groundScene.Instance() as Spatial;
                _groundContainer.AddChild(GenericTile);
                break;
            case "FrontWall":
                GenericTile = _frontWallScene.Instance() as Spatial;
                _wallContainer.AddChild(GenericTile);
                break;
            case "BackWall":
                GenericTile = _backWallScene.Instance() as Spatial;
                _wallContainer.AddChild(GenericTile);
                break;
            case "LavaFloor":
                GenericTile = _lavaFloorScene.Instance() as Spatial;
                _wallContainer.AddChild(GenericTile);
                break;
            default:
                return false;
        }
        GenericTile.Owner = this;
        GenericTile.Translate(new Vector3(x, 0, z));
        return true;
    }

    private void PlaceTree(int x, int z)
    {
        PlaceGenericTile(x, z, "Floor");
        Spatial Tree = _treePillarScene.Instance() as Spatial;
        _wallContainer.AddChild(Tree);
        Tree.Owner = this;
        Tree.Translate(new Vector3(x + 0.5f, 0, z + 0.5f));
    }

    private void PlaceCoin(int x, int z)
    {
        Spatial Coin = _coinScene.Instance() as Spatial;
        _treasureContainer.AddChild(Coin);
        Coin.Owner = this;
        Coin.Translate(new Vector3(x + 0.5f, 0, z + 0.5f));
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

        _wallContainer.Name = _wallContainerName;
        AddChild(_wallContainer);
        _wallContainer.Owner = this;

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
