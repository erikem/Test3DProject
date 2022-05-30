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
    private float CurrentHP = 10f;
    private float MaxHP = 10f;
    private float Damage = 1f;
    public float Gold = 0f;
    private TimeSpan CurrentAttackDelay = TimeSpan.FromSeconds(0.5f);
    SortedList<string, float> DelaysByAttackDict;
    private DateTime LastAttackTime;
    private float MoveSpeed = 1.5f;
    private float RunSpeedModifier = 2f;
    private float JumpForce = 5;
    private float Gravity = 10;
    private Vector3 Vel;
    private Camera PlayerCamera;
    private Spatial PlayerCameraController;
    private RayCast AttackRayCast;
    private float RegenAmount = 1f;
    private float RegenFrequency = 10f;
    private Timer RegenTimer;
    private bool DoubleJumpExecuted = false;
    private bool IsFalling = false;
    private DateTime StartedFallingAt;
    private TimeSpan FallToDeathTime = TimeSpan.FromSeconds(5);
    private UI PlayerUI;
    private AnimationPlayer SwordAnimator;
    private bool WeaponDamageDealt = false;
    private Spatial model;
    private Spatial enemiesContainer;
    private Godot.Collections.Array allEnemies;
    private KinematicBody lockOnTrarget = null;
    private float moveTowardsThreshold = 15f;
    private float moveBackwardsThreshold = 165f;
    private bool MovingTowardsTarget = false;
    private bool MovingBackwardsFromTarget = false;


    public override void _Ready()
    {
        Vel = new Vector3();
        PlayerCamera = GetNode("PlayerAttachedCameraController/PlayerCamera") as Camera;
        PlayerCameraController = GetNode("PlayerAttachedCameraController") as Spatial;
        AttackRayCast = GetNode("Model/Container/SwordController/Sword/AttackRayCast") as RayCast;
        RegenTimer = GetNode("RegenTimer") as Timer;
        model = GetNode("Model") as Spatial;
        enemiesContainer = GetNode("/root/MainScene/Enemies") as Spatial;
        //allEnemies = enemiesContainer.GetChildren();
        RegenTimer.WaitTime = RegenFrequency;
        RegenTimer.Start();
        SwordAnimator = GetNode("Model/Container/SwordController/SwordAnimator") as AnimationPlayer;
        PlayerUI = GetNode("/root/MainScene/UICanvas/UI") as UI;
        PlayerUI.UpdateHealthBar(CurrentHP, MaxHP);
        PlayerUI.UpdateGoldText(Gold);
        LastAttackTime = DateTime.MinValue;
        DelaysByAttackDict = new SortedList<string, float>();
        DelaysByAttackDict["Attack1"] = 0.75f;
        DelaysByAttackDict["Attack2"] = 0.75f;
        DelaysByAttackDict["Attack3"] = 0.75f;
        DelaysByAttackDict["Lunge"] = 1f;
        DelaysByAttackDict["Rising"] = 1f;


    }
    public override void _PhysicsProcess(float delta)
    {
        MovingTowardsTarget = false;
        MovingBackwardsFromTarget = false;
        Vel.x = 0;
        Vel.z = 0;
        Vector3 input = new Vector3();
        float runSpeed = 1f;
        if (Input.IsActionPressed("Sprint"))
        {
            runSpeed = RunSpeedModifier;
        }
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
        Vector3 direction = Transform.basis.z * input.z + Transform.basis.x * input.x;

        Vel.x = direction.x * MoveSpeed * runSpeed;
        Vel.z = direction.z * MoveSpeed * runSpeed;
        Vel.y -= Gravity * delta;

        if (Input.IsActionJustPressed("Jump") && (IsOnFloor() || !DoubleJumpExecuted))
        {
            if (!IsOnFloor())
            {
                DoubleJumpExecuted = true;
            }
            Vel.y = JumpForce;
            //GD.Print("Now character has this many gold pieces: <" + Gold.ToString() + ">");
        }

        if (direction.x != 0 || direction.z != 0)
        {
            //this line makes sure that player always move in expected direction regardless of PlayerAttachedCameraController y rotation.It basically rotates the Vel vector to match teh rotation of teh camera
            Vel = Vel.Rotated(new Vector3(0, 1, 0), PlayerCameraController.Rotation.y);

            //rotates to the direction of movement only if we don't target specific enemt (right mouse button by default)
            if (!Input.IsActionPressed("FocusTarget"))
            {
                //these lines rotate the model of teh chaarcter and his sword to match the moevement direction. This is done by rotatin Model spatial that does not contain either KinematicBody or Camera
                var rot = new Vector2(-Vel.x, -Vel.z).Angle() + Mathf.Deg2Rad(90) + model.Rotation.y;
                model.Transform = model.Transform.Rotated(new Vector3(0, 1, 0), -rot);
            }

        }
        //following code is to target (face) closes enemy if such enemy exists and if player preses FocusTarget action (right mouse button by default)
        if (enemiesContainer.GetChildren().Count > 0 && Input.IsActionPressed("FocusTarget"))
        {
            //we check if player is already locking onto some target
            //We don't want the lock to switch rapidly as enemies change places thus we ensure that while player holds the button he keep targeting the same enemy until either player releases button or enemy dies
            if (!IsInstanceValid(lockOnTrarget) || lockOnTrarget == null)
            {
                //we cycle all enemies that are a children of Mainscene/Enemies and find the closest one
                allEnemies = enemiesContainer.GetChildren();
                float currentDistance = float.MaxValue;
                int currentIndex = 0;
                //GD.Print(allEnemies.Count);
                for (int i = 0; i < allEnemies.Count; i++)
                {
                    if (!IsInstanceValid(allEnemies[i] as Spatial))
                    {
                        continue;
                    }
                    var enemy = ((Spatial)allEnemies[i]).GetNode("Container/EnemyNormal") as KinematicBody;
                    float dist = GlobalTransform.origin.DistanceTo(enemy.GlobalTransform.origin);

                    //GD.Print(i + ": " + dist);
                    if (dist <= currentDistance)
                    {
                        currentDistance = dist;
                        currentIndex = i;
                    }
                }
                //GD.Print(currentIndex);
                //we sent the found enemy as our LockOn target
                lockOnTrarget = ((Spatial)allEnemies[currentIndex]).GetNode("Container/EnemyNormal") as KinematicBody;
            }
            //just in case something is wrong with our selected target we do the null and validity check again
            //probably would be better to let it throw exception and handle the root cause but not in this proto
            if (IsInstanceValid(lockOnTrarget) && lockOnTrarget != null)
            {
                //we determine look direction
                var LookDirection = lockOnTrarget.GlobalTransform.origin;
                //setting Y value of look direction to Translation.y to make sure that player doesn't make weird turns when jumping
                LookDirection.y = Translation.y;
                //Trying to look at enemy but failing because we are looking 180 degress other way
                model.LookAt(LookDirection, Vector3.Up);
                //compensating 180 degree turn
                model.RotateObjectLocal(Vector3.Up, Mathf.Pi);
                float movementToTargetAngle = Mathf.Rad2Deg(Vel.AngleTo(lockOnTrarget.GlobalTransform.origin - GlobalTransform.origin));
                //GD.Print(Mathf.Rad2Deg(movementToTargetAngle));
                if (movementToTargetAngle < moveTowardsThreshold)
                {
                    MovingTowardsTarget = true;
                }
                else if (movementToTargetAngle > moveBackwardsThreshold)
                {
                    MovingBackwardsFromTarget = true;
                }

            }

        }
        else
        {
            //we set LockOn target to null once player releases button of if there are no more enemies left on scene
            lockOnTrarget = null;
        }
        Vel = MoveAndSlide(Vel, Vector3.Up);


    }
    private void _on_RegenTimer_timeout()
    {
        ChangeHealth(RegenAmount);
    }

    public void ReceiveDamage(float damage)
    {
        ChangeHealth(-damage);
    }


    private void ChangeHealth(float value)
    {
        CurrentHP = Mathf.Clamp(CurrentHP + value, 0, MaxHP);
        PlayerUI.UpdateHealthBar(CurrentHP, MaxHP);
        if (CurrentHP <= 0)
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

        if (DateTime.Now - LastAttackTime < CurrentAttackDelay)
        {
            return false;
        }
        SwordAnimator.Stop();
        WeaponDamageDealt = false;
        LastAttackTime = DateTime.Now;
        if (MovingTowardsTarget)
        {
            CurrentAttackDelay = TimeSpan.FromSeconds(DelaysByAttackDict["Lunge"]);
            SwordAnimator.Play("Lunge");
        }
        else
        {
            List<string> Animations = new List<string>();
            Animations.Add("Attack1");
            Animations.Add("Attack2");
            Animations.Add("Attack3");
            var currentAttackAnimation = Animations[Rand.Next(Animations.Count)];
            CurrentAttackDelay = TimeSpan.FromSeconds(DelaysByAttackDict[currentAttackAnimation]);
            SwordAnimator.Play(currentAttackAnimation);
        }


        return true;
    }
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (!IsOnFloor() && Vel.y < 0)
        {
            if (!IsFalling)
            {
                IsFalling = true;
                StartedFallingAt = DateTime.Now;
            }
            else
            {
                if (DateTime.Now - StartedFallingAt > FallToDeathTime)
                {
                    Die();
                }
            }
        }
        else
        {
            if (IsOnFloor())
            {
                DoubleJumpExecuted = false;
            }
            IsFalling = false;

        }
        if (Input.IsActionPressed("Attack"))
        {
            TryAttack();
        }
        if (SwordAnimator.IsPlaying()
        && !WeaponDamageDealt
        && AttackRayCast.IsColliding()
        //such a hack!
        && AttackRayCast.GetCollider().GetType().ToString() == "EnemyNormal")
        {
            (AttackRayCast.GetCollider() as EnemyNormal).ReceiveDamage(Damage);
            WeaponDamageDealt = true;
        }

    }
    public void AddGold(float amount)
    {
        Gold += amount;
        PlayerUI.UpdateGoldText(Gold);
        //Console.WriteLine("Now character has this many gold: <" + Gold.ToString() + ">");
    }
}