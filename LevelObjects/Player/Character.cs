using Godot;
using System;
using System.Collections.Generic;
public class Character : KinematicBody
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public static Random Rand = new Random();
    private float _currentHP = 10f;
    private float _maxHP = 10f;
    private float _damage = 1f;
    public float Gold = 0f;
    private TimeSpan _currentAttackDelay = TimeSpan.FromSeconds(0.5f);
    private SortedList<string, float> _delaysByAttackDict;
    private DateTime _lastAttackTime;
    private float _moveSpeed = 1.5f;
    private float _runSpeedModifier = 2f;
    private float _jumpForce = 5;
    private float _gravity = 10;
    private Vector3 _vel;
    private Camera _playerCamera;
    private Spatial _playerCameraController;
    private RayCast _attackRayCast;
    private float _regenAmount = 1f;
    private float _regenFrequency = 10f;
    private Timer _regenTimer;
    private bool _doubleJumpExecuted = false;
    private bool _isFalling = false;
    private DateTime _startedFallingAt;
    private TimeSpan _fallToDeathTime = TimeSpan.FromSeconds(5);
    private UI _playerUI;
    private AnimationPlayer _swordAnimator;
    private bool _weaponDamageDealt = false;
    private Spatial _model;
    private Spatial _enemiesContainer;
    private Godot.Collections.Array _allEnemies;
    private KinematicBody _lockOnTrarget = null;
    private float _moveTowardsThreshold = 35f;
    private float _moveBackwardsThreshold = 155f;
    private bool _movingTowardsTarget = false;
    private bool _movingBackwardsFromTarget = false;
    private bool _forcedDragInProcess = false;
    private TimeSpan _forcedDragDuration;
    private DateTime _forcedDragStarted;
    private Vector3 _forcedDragDirection;
    private string _forcedDragName = "";
    private bool _targetCentricControlsOrientation = false;
    private int _attackUniqueID = 0;
    public int AttackID { get => _attackUniqueID; }
    private DateTime _lastDashAt;
    private TimeSpan _dashCooldown = TimeSpan.FromSeconds(0.75f);
    //private _playerCollisionShape


    public KinematicBody LockOnTrarget { get => _lockOnTrarget; set => _lockOnTrarget = value; }
    public AnimationPlayer SwordAnimator { get => _swordAnimator; set => _swordAnimator = value; }

    public override void _Ready()
    {
        _vel = new Vector3();
        _playerCamera = GetNode("PlayerAttachedCameraController/PlayerCamera") as Camera;
        _playerCameraController = GetNode("PlayerAttachedCameraController") as Spatial;
        _attackRayCast = GetNode("Model/Container/SwordController/Sword/AttackRayCast") as RayCast;
        _regenTimer = GetNode("RegenTimer") as Timer;
        _model = GetNode("Model") as Spatial;
        _enemiesContainer = GetNode("/root/MainScene/Enemies") as Spatial;
        //allEnemies = enemiesContainer.GetChildren();
        _regenTimer.WaitTime = _regenFrequency;
        _regenTimer.Start();
        SwordAnimator = GetNode("Model/Container/SwordController/SwordAnimator") as AnimationPlayer;
        _playerUI = GetNode("/root/MainScene/UICanvas/UI") as UI;
        _playerUI.UpdateHealthBar(_currentHP, _maxHP);
        _playerUI.UpdateGoldText(Gold);
        _lastAttackTime = DateTime.MinValue;
        //defining attack delays after different attack animations
        _delaysByAttackDict = new SortedList<string, float>();
        _delaysByAttackDict["Attack1"] = 0.75f;
        _delaysByAttackDict["Attack2"] = 0.75f;
        _delaysByAttackDict["Attack3"] = 0.75f;
        _delaysByAttackDict["Lunge"] = 0.75f;
        _delaysByAttackDict["Rising"] = 0.5f;
    }

    private Vector3 HandleMovementInput()
    {
        Vector3 input = new Vector3();
        if (Input.IsActionPressed("MoveForward"))
        {
            input.z += 1;
        }
        if (Input.IsActionPressed("MoveBack"))
        {
            input.z -= 1;
        }
        if (Input.IsActionPressed("MoveRight"))
        {
            input.x -= 1;
        }
        if (Input.IsActionPressed("MoveLeft"))
        {
            input.x += 1;
        }
        input = input.Normalized();
        return Transform.basis.z * input.z + Transform.basis.x * input.x;
    }
    private float HandleSprintInput()
    {
        if (Input.IsActionPressed("Sprint"))
        {
            return _runSpeedModifier;
        }
        else
        {
            return 1f;
        }
    }

    private void ApplyForcedDrag(ForcedDrag drag)
    {
        _forcedDragStarted = DateTime.Now;
        _forcedDragInProcess = true;
        _forcedDragDirection = drag.DragVector.Normalized();
        _forcedDragDirection = _forcedDragDirection * drag.SpeedModifier;
        _forcedDragDuration = drag.Duration;
        _forcedDragName = drag.Name;
    }

    public override void _PhysicsProcess(float delta)
    {
        //resetting key movement properties to make sure that movement stops if there's no player input
        _movingTowardsTarget = false;
        _movingBackwardsFromTarget = false;
        _vel.x = 0;
        _vel.z = 0;



        if (_forcedDragInProcess
        && DateTime.Now - _forcedDragStarted >= _forcedDragDuration)
        {
            _forcedDragInProcess = false;
        }
        //check for forced drag. Forced drag basically means that player is being pushed somewhere and cannot use other control at that point
        if (!_forcedDragInProcess)
        {
            float runSpeed = HandleSprintInput();
            Vector3 direction = HandleMovementInput();
            _vel.x = direction.x * _moveSpeed * runSpeed;
            _vel.z = direction.z * _moveSpeed * runSpeed;
            _vel.y -= _gravity * delta;

            /*if (Input.IsActionJustPressed("Jump") && (IsOnFloor() || !_doubleJumpExecuted))
            {
                if (!IsOnFloor())
                {
                    _doubleJumpExecuted = true;
                }
                _vel.y = _jumpForce;
            }*/
            if ((direction.x != 0 || direction.z != 0))
            {
                //this line makes sure that player always move in expected direction regardless of PlayerAttachedCameraController y rotation.It basically rotates the Vel vector to match teh rotation of teh camera
                _vel = _vel.Rotated(new Vector3(0, 1, 0), _playerCameraController.Rotation.y);

                //rotates to the direction of movement only if we don't target specific enemt (right mouse button by default)
                if (!Input.IsActionPressed("FocusTarget"))
                {
                    //these lines rotate the model of teh chaarcter and his sword to match the moevement direction. This is done by rotatin Model spatial that does not contain either KinematicBody or Camera
                    var rot = new Vector2(-_vel.x, -_vel.z).Angle() + Mathf.Deg2Rad(90) + _model.Rotation.y;
                    _model.Transform = _model.Transform.Rotated(new Vector3(0, 1, 0), -rot);
                }

            }
            //following code is to target (face) closes enemy if such enemy exists and if player preses FocusTarget action (right mouse button by default)
            if (_enemiesContainer.GetChildren().Count > 0 && Input.IsActionPressed("FocusTarget"))
            {
                //we check if player is already locking onto some target
                //We don't want the lock to switch rapidly as enemies change places thus we ensure that while player holds the button he keep targeting the same enemy until either player releases button or enemy dies
                if (!IsInstanceValid(LockOnTrarget) || LockOnTrarget == null)
                {
                    //we cycle all enemies that are a children of Mainscene/Enemies and find the closest one
                    _allEnemies = _enemiesContainer.GetChildren();
                    float currentDistance = float.MaxValue;
                    int currentIndex = 0;
                    for (int i = 0; i < _allEnemies.Count; i++)
                    {
                        if (!IsInstanceValid(_allEnemies[i] as Spatial))
                        {
                            continue;
                        }
                        var enemy = ((Spatial)_allEnemies[i]).GetNode("Container/EnemyNormal") as KinematicBody;
                        float dist = GlobalTransform.origin.DistanceTo(enemy.GlobalTransform.origin);

                        if (dist <= currentDistance)
                        {
                            currentDistance = dist;
                            currentIndex = i;
                        }
                    }
                    //we sent the found enemy as our LockOn target
                    LockOnTrarget = ((Spatial)_allEnemies[currentIndex]).GetNode("Container/EnemyNormal") as KinematicBody;
                }
                //just in case something is wrong with our selected target we do the null and validity check again
                //probably would be better to let it throw exception and handle the root cause but not in this proto
                if (IsInstanceValid(LockOnTrarget) && LockOnTrarget != null)
                {
                    //we determine look direction
                    var LookDirection = LockOnTrarget.GlobalTransform.origin;
                    //setting Y value of look direction to Translation.y to make sure that player doesn't make weird turns when jumping
                    LookDirection.y = Translation.y;
                    //Trying to look at enemy but failing because we are looking 180 degress other way
                    _model.LookAt(LookDirection, Vector3.Up);
                    //compensating 180 degree turn
                    _model.RotateObjectLocal(Vector3.Up, Mathf.Pi);

                    //switches movement controls to target-centric. On this mode pressing forward will move you straight towards target instead of forward along Z axis
                    if (_targetCentricControlsOrientation)
                    {
                        _vel = _vel.Rotated(new Vector3(0, 1, 0), -_playerCameraController.Rotation.y);
                        _vel = _vel.Rotated(new Vector3(0, 1, 0), _model.Rotation.y);
                    }

                    //_vel = _vel.Rotated(new Vector3(0, 1, 0), Mathf.Deg2Rad(-90));
                    //_vel = new Vector3(_vel.z, _vel.y, _vel.x);

                    //determining if player is moving towards target, away from it or in some other direction
                    float movementToTargetAngle = Mathf.Rad2Deg(_vel.AngleTo(LockOnTrarget.GlobalTransform.origin - GlobalTransform.origin));
                    if (movementToTargetAngle < _moveTowardsThreshold)
                    {
                        _movingTowardsTarget = true;
                    }
                    else if (movementToTargetAngle > _moveBackwardsThreshold)
                    {
                        _movingBackwardsFromTarget = true;
                    }
                }
                else
                {
                    //setting it to null if instance is not valid
                    LockOnTrarget = null;
                }

            }
            else
            {
                //we set LockOn target to null once player releases button of if there are no more enemies left on scene
                LockOnTrarget = null;
            }

            TryDash();
        }
        else
        {
            _vel = _forcedDragDirection;
            _vel.y -= _gravity * delta;

        }


        _vel = MoveAndSlide(_vel, Vector3.Up);


        if (GetSlideCount() > 0
        && _forcedDragInProcess
        && (_forcedDragName == "evade"
        || _forcedDragName == "LungeAttacker"))
        {
            for (int i = 0; i < GetSlideCount(); i++)
            {
                if (GetSlideCollision(i).Collider.GetType() == typeof(EnemyNormal))
                {
                    _forcedDragInProcess = false;
                }
            }
        }

    }

    private void TryDash()
    {
        if (Input.IsActionJustPressed("Dash")
                    && DateTime.Now - _lastDashAt >= _dashCooldown)
        {
            _lastDashAt = DateTime.Now;
            if (Mathf.Abs(_vel.x) < 0.01 && Mathf.Abs(_vel.z) < 0.01)
            {
                ApplyForcedDrag(new ForcedDrag(new Vector3(0, 0, 1).Rotated(new Vector3(0, 1, 0), _model.Rotation.y), 15, 0.1f, "evade"));
            }
            else
            {
                ApplyForcedDrag(new ForcedDrag(_vel, 15, 0.1f, "evade"));
            }

        }
    }

    private void _on_RegenTimer_timeout()
    {
        ChangeHealth(_regenAmount);
    }

    public void ReceiveDamage(float damage)
    {
        ChangeHealth(-damage);
    }


    private void ChangeHealth(float value)
    {
        _currentHP = Mathf.Clamp(_currentHP + value, 0, _maxHP);
        _playerUI.UpdateHealthBar(_currentHP, _maxHP);
        if (_currentHP <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        GetTree().ReloadCurrentScene();
    }

    public bool TryAttack()
    {

        if (DateTime.Now - _lastAttackTime < _currentAttackDelay)
        {
            return false;
        }
        SwordAnimator.Stop();
        _weaponDamageDealt = false;
        _lastAttackTime = DateTime.Now;
        if (_movingTowardsTarget && GlobalTransform.origin.DistanceTo(LockOnTrarget.GlobalTransform.origin) > 0.5f)
        {
            _currentAttackDelay = TimeSpan.FromSeconds(_delaysByAttackDict["Lunge"]);
            SwordAnimator.Play("Lunge");
            ApplyForcedDrag(new ForcedDrag(new Vector3(0, 0, 1).Rotated(new Vector3(0, 1, 0), _model.Rotation.y), 20, 0.15f, "LungeAttacker"));
        }
        else if (_movingBackwardsFromTarget && IsOnFloor())
        {
            _currentAttackDelay = TimeSpan.FromSeconds(_delaysByAttackDict["Rising"]);
            SwordAnimator.Play("Rising");
            ApplyForcedDrag(new ForcedDrag(new Vector3(0, 1, 0.2f).Rotated(new Vector3(0, 1, 0), _model.Rotation.y), 2.5f, 0.5f, "RisingAttacker"));
        }
        else
        {
            List<string> Animations = new List<string>();
            Animations.Add("Attack1");
            Animations.Add("Attack2");
            Animations.Add("Attack3");
            var currentAttackAnimation = Animations[Rand.Next(Animations.Count)];
            _currentAttackDelay = TimeSpan.FromSeconds(_delaysByAttackDict[currentAttackAnimation]);
            SwordAnimator.Play(currentAttackAnimation);
        }
        _attackUniqueID = (int)GD.Randi();


        return true;
    }
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        //switching between control orientations
        ProcessFallDeathTime();
        if (Input.IsActionPressed("GlobalControlsOrientation"))
        {
            _targetCentricControlsOrientation = false;
        }
        if (Input.IsActionPressed("TargetCentricControlsOrientation"))
        {
            _targetCentricControlsOrientation = true;
        }
        //processing attack input here
        if (Input.IsActionPressed("Attack"))
        {
            TryAttack();
        }
        ProcessAttackInProgress();

    }

    private void ProcessAttackInProgress()
    {
        if (SwordAnimator.IsPlaying()
                && !_weaponDamageDealt
                && _attackRayCast.IsColliding()
                //such a hack!
                && _attackRayCast.GetCollider().GetType().ToString() == "EnemyNormal")
        {
            ForcedDrag dragToTarget = null;
            if (SwordAnimator.CurrentAnimation == "Lunge")
            {

            }
            else if (SwordAnimator.CurrentAnimation == "Rising")
            {
                dragToTarget = new ForcedDrag(new Vector3(0, 1, 0.1f).Rotated(new Vector3(0, 1, 0), _model.Rotation.y), 2.5f, 0.5f, "RisingTarget");
            }
            (_attackRayCast.GetCollider() as EnemyNormal).ReceiveDamage(_damage, dragToTarget);
            _weaponDamageDealt = true;
        }
    }

    private void ProcessFallDeathTime()
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
            if (IsOnFloor())
            {
                _doubleJumpExecuted = false;
            }
            _isFalling = false;

        }
    }

    public void AddGold(float amount)
    {
        Gold += amount;
        _playerUI.UpdateGoldText(Gold);
        //Console.WriteLine("Now character has this many gold: <" + Gold.ToString() + ">");
    }
}