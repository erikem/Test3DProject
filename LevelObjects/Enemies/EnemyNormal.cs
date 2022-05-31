using Godot;
using System;

public class EnemyNormal : KinematicBody
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    private float _currentHP = 3f;
    private float _maxHP = 3f;
    private float _damage = 1f;
    private float _attackDistance = 1.5f;
    private TimeSpan _attackRate = TimeSpan.FromSeconds(1.5f);
    private DateTime _lastAttackTime;
    private float _moveSpeed = 0.1f;
    private float _gravity = 10;
    private Vector3 _vel;
    private RayCast _attackRayCast;
    private bool _isFalling = false;
    private DateTime _startedFallingAt;
    private TimeSpan _fallToDeathTime = TimeSpan.FromSeconds(5);
    private AnimationPlayer _swordAnimator;
    private bool _weaponDamageDealt = false;
    private KinematicBody _player;
    private Spatial _playerModel;
    private Spatial _myRoot;
    private bool _forcedDragInProcess = false;
    private TimeSpan _forcedDragDuration;
    private DateTime _forcedDragStarted;
    private Vector3 _forcedDragDirection;
    private void ApplyForcedDrag(ForcedDrag drag)
    {
        _forcedDragStarted = DateTime.Now;
        _forcedDragInProcess = true;
        _forcedDragDirection = drag.DragVector.Normalized();
        _forcedDragDirection = _forcedDragDirection * drag.SpeedModifier;
        _forcedDragDuration = drag.Duration;
    }
    public override void _Ready()
    {
        _vel = new Vector3();
        _attackRayCast = GetNode("SwordController/Sword/AttackRayCast") as RayCast;
        _swordAnimator = GetNode("SwordController/SwordAnimator") as AnimationPlayer;
        _lastAttackTime = DateTime.MinValue;
        _player = GetNode("/root/MainScene/Character001_Normalized/PlayerCharacter") as KinematicBody;
        _playerModel = GetNode("/root/MainScene/Character001_Normalized/PlayerCharacter/Model") as Spatial;
        _myRoot = GetParent().GetParent() as Spatial;

    }
    public override void _PhysicsProcess(float delta)
    {
        if (_forcedDragInProcess
        && DateTime.Now - _forcedDragStarted >= _forcedDragDuration)
        {
            _forcedDragInProcess = false;
        }
        if (!_forcedDragInProcess)
        {
            var dist = GlobalTransform.origin.DistanceTo(_playerModel.GlobalTransform.origin);

            //direction from current enemy to player in 3D plane
            var direction3D = _playerModel.GlobalTransform.origin - GlobalTransform.origin;
            //nomrlaized direction with assuming Y value is 0 so it is not take into account
            var direction = new Vector3(direction3D.x, 0, direction3D.z).Normalized();
            //general direction of player
            var LookDirection = _playerModel.GlobalTransform.origin;
            //setting Y value of look direction sop that enemies don't look UP/DOWN in case player is above/belwo them
            GD.Print(Translation.y);
            LookDirection.y = 0;
            //Trying to look at player but failing because we are looking 180 degress other way
            LookAt(LookDirection, Vector3.Up);
            //compensating 1800 degrees to actually look at player
            RotateObjectLocal(Vector3.Up, Mathf.Pi);
            //setting up speed to match general direction of player, multiplying by speed of enemy. Y value is modified by gravity
            _vel.x = direction.x * _moveSpeed;
            _vel.y -= _gravity * delta;
            _vel.z = direction.z * _moveSpeed;
        }
        else
        {
            _vel = _forcedDragDirection;
            _vel.y -= _gravity * delta;

        }
        //standard move and slide
        _vel = MoveAndSlide(_vel, Vector3.Up);

    }

    public void ReceiveDamage(float damage, ForcedDrag drag = null)
    {
        if (drag != null)
        {
            ApplyForcedDrag(drag);
        }
        ChangeHealth(-damage);
    }


    private void ChangeHealth(float value)
    {
        _currentHP = Mathf.Clamp(_currentHP + value, 0, _maxHP);
        if (_currentHP <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        _myRoot.QueueFree();
    }

    public bool TryAttack()
    {

        if (DateTime.Now - _lastAttackTime < _attackRate)
        {
            return false;
        }
        _swordAnimator.Stop();
        _weaponDamageDealt = false;
        _lastAttackTime = DateTime.Now;
        if (GD.RandRange(1, 100) > 50)
        {
            _swordAnimator.Play("Attack1");
        }
        else
        {
            _swordAnimator.Play("Attack2");
        }
        return true;
    }
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        FallToDeathCheck();
        if (GlobalTransform.origin.DistanceTo(_playerModel.GlobalTransform.origin) <= _attackDistance)
        {
            TryAttack();
        }
        if (_swordAnimator.IsPlaying()
        && !_weaponDamageDealt
        && _attackRayCast.IsColliding()
        && _attackRayCast.GetCollider().GetType().ToString() == "Character")
        {
            (_attackRayCast.GetCollider() as Character).ReceiveDamage(_damage);
            _weaponDamageDealt = true;
        }

    }

    private void FallToDeathCheck()
    {
        if (!IsOnFloor() && _vel.y < 0)
        {
            if (!_isFalling)
            {
                _isFalling = true;
                _startedFallingAt = DateTime.Now;
            }
            else
            {
                if (DateTime.Now - _startedFallingAt > _fallToDeathTime)
                {
                    Die();
                }
            }
        }
        else
        {
            _isFalling = false;
        }
    }
}






