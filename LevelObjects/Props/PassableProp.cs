using Godot;
using System;
using System.Collections.Generic;

public class PassableProp : Spatial
{
    protected List<string> _complimentaryProps = new List<string>();
    //chance to spawn extra prop
    protected float _extraPropChance = 0.25f;
    protected static Random _rnd = new Random();

    virtual public void Init(Node rootScene, Spatial scenePropsContainer, float x, float z)
    {
        //getting the object we'll be moving and rotating within tile
        var _propHolder = GetNode("Prop") as Spatial;
        //defining the move distance and rotation
        float offsetX = x + (float)_rnd.NextDouble() * 0.8f + 0.1f;
        float offsetZ = z + (float)_rnd.NextDouble() * 0.8f + 0.1f;
        int offsetAngle = _rnd.Next(360);
        //moving and rotating prop (has to be of scale 1)
        _propHolder.Translate(new Vector3(offsetX, 0, offsetZ));
        _propHolder.Rotate(Vector3.Up, Mathf.Deg2Rad(offsetAngle));
        //CallDeferred is needed becaus the expect tree parent and root are not valid yet
        scenePropsContainer.CallDeferred("add_child", this);
        CallDeferred("set_owner", rootScene);
        //we check if complimentary (extra) props list is not empty and check probability
        //if passed we simply generate one more prop selected random from _complimentaryProps list
        if (_complimentaryProps.Count > 0
        && _rnd.NextDouble() < _extraPropChance)
        {
            var extraProp = GD.Load<PackedScene>(_complimentaryProps[_rnd.Next(_complimentaryProps.Count)]);
            var extraPropInstance = extraProp.Instance() as PassableProp;
            extraPropInstance.Init(rootScene, scenePropsContainer, x, z);
        }

    }
}
