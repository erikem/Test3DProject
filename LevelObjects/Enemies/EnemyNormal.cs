using Godot;
using System;

public class EnemyNormal : KinematicBody
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    private float CurrentHP = 3f;
    private float MaxHP = 3f;
    private float Damage = 1f;
    private float AttackDistance = 8f;
    private TimeSpan AttackRate = TimeSpan.FromSeconds(1.5f);
    private DateTime LastAttackTime;
    private float MoveSpeed = 10f;
    private float Gravity = 60;
    private Vector3 Vel;
    private RayCast AttackRayCast;
    private bool IsFalling = false;
    private DateTime StartedFallingAt;
    private TimeSpan FallToDeathTime = TimeSpan.FromSeconds(5);
    private AnimationPlayer SwordAnimator;
    private bool WeaponDamageDealt = false;
    private KinematicBody Player;

    public override void _Ready()
    {
        Vel = new Vector3();
        AttackRayCast = GetNode("SwordController/Sword/AttackRayCast") as RayCast;
        SwordAnimator = GetNode("SwordController/SwordAnimator") as AnimationPlayer;
        LastAttackTime = DateTime.MinValue;
        Player = GetNode("/root/MainScene/Character") as KinematicBody;

    }
    public override void _PhysicsProcess(float delta)
    {
        //var dist = Translation.DistanceTo(Player.Translation);
        var direction = (Player.Translation - Translation).Normalized();
        var LookDirection = Player.Translation;
        // LookDirection.z = 0;
        // LookDirection.x = 0;
        LookDirection.y = 1;
        LookAt(LookDirection, Vector3.Up);
        RotateObjectLocal(Vector3.Up, Mathf.Pi);
        Vel.x = direction.x * MoveSpeed;
        Vel.y -= Gravity * delta;
        Vel.z = direction.z * MoveSpeed;
        //Vel.z = -MoveSpeed;
        Vel = MoveAndSlide(Vel, Vector3.Up);

    }

    public void ReceiveDamage(float damage)
    {
        ChangeHealth(-damage);
    }


    private void ChangeHealth(float value)
    {
        CurrentHP = Mathf.Clamp(CurrentHP + value, 0, MaxHP);
        if (CurrentHP <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        QueueFree();
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
            IsFalling = false;
        }
        if (Translation.DistanceTo(Player.Translation) <= AttackDistance)
        {
            TryAttack();
        }
        if (SwordAnimator.IsPlaying()
        && !WeaponDamageDealt
        && AttackRayCast.IsColliding()
        && AttackRayCast.GetCollider().GetType().ToString() == "Character")
        {
            (AttackRayCast.GetCollider() as Character).ReceiveDamage(Damage);
            WeaponDamageDealt = true;
        }

    }
}






