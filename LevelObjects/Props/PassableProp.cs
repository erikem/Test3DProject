using Godot;
using System.Collections.Generic;

public class PassableProp : Spatial
{
    private Node _rootScene;
    protected List<string> _complimentaryProps = new List<string>();
    private Spatial _propHolder;
    private Spatial _scenePropsContainer;
    protected float _extraPropChance = 0.15f;
    protected void _CustomReady()
    {
        _rootScene = GetTree().Root.GetChild(0);
        _propHolder = GetNode("Prop") as Spatial;
        _scenePropsContainer = _rootScene.GetNode("Props") as Spatial;
        Place();
    }

    protected void Place()
    {
        float offsetX = GD.Randf() * 0.8f + 0.1f;
        float offsetZ = GD.Randf() * 0.8f + 0.1f;
        int offsetAngle = (int)GD.Randi() % 180;
        _propHolder.Translate(new Vector3(offsetX, 0, offsetZ));
        _propHolder.Rotate(Vector3.Up, Mathf.Deg2Rad(offsetAngle));
        if (_complimentaryProps.Count > 0
        && GD.Randf() < _extraPropChance)
        {
            var extraProp = GD.Load<PackedScene>(_complimentaryProps[Mathf.Abs((int)GD.Randi()) % _complimentaryProps.Count]);
            var extraPropInstance = extraProp.Instance();
            _scenePropsContainer.AddChild(extraPropInstance);
            extraPropInstance.Owner = _rootScene;
        }

    }
}
