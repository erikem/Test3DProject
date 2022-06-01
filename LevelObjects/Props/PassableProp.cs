using Godot;
using System;
using System.Collections.Generic;

public class PassableProp : Spatial
{
    protected List<string> _complimentaryProps = new List<string>();
    protected float _extraPropChance = 0.25f;
    protected static Random _rnd = new Random();

    virtual public void Init(Node rootScene, Spatial scenePropsContainer, float x, float z)
    {
        var _propHolder = GetNode("Prop") as Spatial;
        float offsetX = x + (float)_rnd.NextDouble() * 0.8f + 0.1f;
        float offsetZ = z + (float)_rnd.NextDouble() * 0.8f + 0.1f;
        int offsetAngle = _rnd.Next(360);
        _propHolder.Translate(new Vector3(offsetX, 0, offsetZ));
        _propHolder.Rotate(Vector3.Up, Mathf.Deg2Rad(offsetAngle));
        scenePropsContainer.CallDeferred("add_child", this);
        CallDeferred("set_owner", rootScene);
        if (_complimentaryProps.Count > 0
        && _rnd.NextDouble() < _extraPropChance)
        {
            var extraProp = GD.Load<PackedScene>(_complimentaryProps[_rnd.Next(_complimentaryProps.Count)]);
            var extraPropInstance = extraProp.Instance() as PassableProp;
            extraPropInstance.Init(rootScene, scenePropsContainer, x, z);
        }

    }
}
