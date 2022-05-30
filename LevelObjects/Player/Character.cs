using Godot;
using System;

public class Character : KinematicBody
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    private float CurrentHP = 10f;
    private float MaxHP = 10f;
    private float Damage = 1f;
    public float Gold = 0f;
    private TimeSpan AttackRate = TimeSpan.FromSeconds(1f);
    private DateTime LastAttackTime;
    private float MoveSpeed = 15f;
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

    public override void _Ready()
    {
        Vel = new Vector3();
        PlayerCamera = GetNode("PlayerAttachedCameraController/PlayerCamera") as Camera;
        PlayerCameraController = GetNode("PlayerAttachedCameraController") as Spatial;
        AttackRayCast = GetNode("Model/SwordController/Sword/AttackRayCast") as RayCast;
        RegenTimer = GetNode("RegenTimer") as Timer;
        model = GetNode("Model") as Spatial;
        RegenTimer.WaitTime = RegenFrequency;
        RegenTimer.Start();
        SwordAnimator = GetNode("Model/SwordController/SwordAnimator") as AnimationPlayer;
        PlayerUI = GetNode("/root/MainScene/UICanvas/UI") as UI;
        PlayerUI.UpdateHealthBar(CurrentHP, MaxHP);
        PlayerUI.UpdateGoldText(Gold);
        LastAttackTime = DateTime.MinValue;

    }
    public override void _PhysicsProcess(float delta)
    {
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
            //GD.Print(new Vector2(Vel.x, -Vel.z).Angle());
            //these lines rotate the model of teh chaarcter and his sword to match the moevement direction. This is done by rotatin Model spatial that does not contain either KinematicBody or Camera
            var rot = new Vector2(-Vel.x, -Vel.z).Angle() + Mathf.Deg2Rad(90) + model.Rotation.y;
            model.Transform = model.Transform.Rotated(new Vector3(0, 1, 0), -rot);

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

        if (DateTime.Now - LastAttackTime < AttackRate)
        {
            return false;
        }
        SwordAnimator.Stop();
        WeaponDamageDealt = false;
        LastAttackTime = DateTime.Now;
        if (GD.RandRange(1, 100) > 50)
        {
            SwordAnimator.Play("Attack1");
        }
        else
        {
            SwordAnimator.Play("Attack2");
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