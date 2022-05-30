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
    private float AttackDistance = 1.5f;
    private TimeSpan AttackRate = TimeSpan.FromSeconds(1.5f);
    private DateTime LastAttackTime;
    private float MoveSpeed = 0.1f;
    private float Gravity = 6;
    private Vector3 Vel;
    private RayCast AttackRayCast;
    private bool IsFalling = false;
    private DateTime StartedFallingAt;
    private TimeSpan FallToDeathTime = TimeSpan.FromSeconds(5);
    private AnimationPlayer SwordAnimator;
    private bool WeaponDamageDealt = false;
    private KinematicBody Player;
    private Spatial playerModel;
    private Spatial myRoot;

    public override void _Ready()
    {
        Vel = new Vector3();
        AttackRayCast = GetNode("SwordController/Sword/AttackRayCast") as RayCast;
        SwordAnimator = GetNode("SwordController/SwordAnimator") as AnimationPlayer;
        LastAttackTime = DateTime.MinValue;
        Player = GetNode("/root/MainScene/Character001_Normalized/PlayerCharacter") as KinematicBody;
        playerModel = GetNode("/root/MainScene/Character001_Normalized/PlayerCharacter/Model") as Spatial;
        myRoot = GetParent().GetParent() as Spatial;

    }
    public override void _PhysicsProcess(float delta)
    {
        var dist = GlobalTransform.origin.DistanceTo(playerModel.GlobalTransform.origin);

        //direction from current enemy to player in 3D plane
        var direction3D = playerModel.GlobalTransform.origin - GlobalTransform.origin;
        //nomrlaized direction with assuming Y value is 0 so it is not take into account
        var direction = new Vector3(direction3D.x, 0, direction3D.z).Normalized();
        //general direction of player
        var LookDirection = playerModel.GlobalTransform.origin;
        //setting Y value of look direction sop that enemies don't look UP/DOWN in case player is above/belwo them
        LookDirection.y = 0;
        //Trying to look at player but failing because we are looking 180 degress other way
        LookAt(LookDirection, Vector3.Up);
        //compensating 1800 degrees to actually look at player
        RotateObjectLocal(Vector3.Up, Mathf.Pi);
        //setting up speed to match general direction of player, multiplying by speed of enemy. Y value is modified by gravity
        Vel.x = direction.x * MoveSpeed;
        Vel.y -= Gravity * delta;
        Vel.z = direction.z * MoveSpeed;
        //standard move and slide
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
        myRoot.QueueFree();
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
        if (GlobalTransform.origin.DistanceTo(playerModel.GlobalTransform.origin) <= AttackDistance)
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






