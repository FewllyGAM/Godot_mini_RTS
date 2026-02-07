using Godot;
using System;
using System.Diagnostics;
using System.Formats.Tar;
using System.Reflection.Metadata;

public partial class Entity : CharacterBody2D, IHasStats
{
    [Export] public float speed = 50;

    [Export] public StatsData Stats = new StatsData();

    protected NavigationAgent2D navAgent;
    protected CollisionShape2D collider;

    protected bool engaged;
    protected bool attackOnCD;
    protected bool targetInsideRange;

    HealthBar healthBar;

    public bool IsUntargetable => untargetable;
    protected bool untargetable;

    protected IHasStats currentTarget;
    public ThreatArea ThreatArea {get;set;}

    public override void _Ready()
    {
        navAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
        collider = GetNode<CollisionShape2D>("CollisionShape2D");

        SetHealthBar();

        Stats = Stats.Duplicate() as StatsData;
        Stats.Init();
    }

    public override void _PhysicsProcess(double delta)
    {
        //if (Input.IsActionJustPressed("right")) ChangeHealth(-10);

        if (engaged)
        {
            TryAttackTarget();
        }

        Vector2 dir = ToLocal(navAgent.GetNextPathPosition()).Normalized();
        navAgent.Velocity = dir * speed;
        MoveAndSlide();
    }


    public void SetPath(Vector2 target)
    {
        navAgent.TargetPosition = target;
    }

    protected void OnAgentVelocityComputed(Vector2 safeVelocity)
    {
        Velocity = safeVelocity;
    }

    protected virtual void OnNavigationFinished()
    {
        Debug.Print("REACHED TARGET");        

        if (engaged)
        {
            targetInsideRange = true;
        }
        else Velocity = Vector2.Zero;
    }

    //Relacionado a Stats

    public void SetHealthBar()
    {
        //emptyHealthBar = GetNode<Control>("health_bar_empty");
        healthBar = GetNode<HealthBar>("HP");

        if (healthBar != null) healthBar.Visible = false;
    }

    public void ChangeHealth(float value)
    {
        healthBar.Visible = true;

        Stats.ChangeHealth(value);

        healthBar.UpdateBar(Stats.Health, Stats.MaxHealth);

        if (Stats.Health == 0) Die();
    }

    //Relacionado a combate
    protected virtual void ResearchNextTarget()
    {
        //base.ResearchNextTarget();

        engaged = false;
        // threatArea.Disabled = true;
        ThreatArea.DisableArea();
        //EnableThreatArea();
        ThreatArea.EnableThreatArea();
    }

    public virtual void SetAttackTarget(Node2D target)
    {
        if (engaged) return;

        if (target is IHasStats targ) 
        {
            if (targ.IsUntargetable) return;
            currentTarget = targ;
            engaged = true;
            navAgent.TargetDesiredDistance = Stats.AttackRange;
            SetPath(target.Position);
        }
    }
    protected void SetAttack()
    {
        if (currentTarget == null) return;
        SetPath(currentTarget.Position);
    }

    void TryAttackTarget()
    {
        Debug.Print("TRY ATTACK");
        if (currentTarget == null || currentTarget.IsUntargetable)
        {
            ResearchNextTarget();
            return;
        }

        if (!attackOnCD)
        {
            if (targetInsideRange)
            {
                Debug.Print("ATATCK");
                currentTarget.ChangeHealth(-Stats.AttackDamage);

                AttackCooldown();
            }
            else SetAttack();
        }
    }

    async void AttackCooldown()
    {
        attackOnCD = true;
        await ToSignal(GetTree().CreateTimer(1.0f), SceneTreeTimer.SignalName.Timeout);
        targetInsideRange = false;
        attackOnCD = false;
    }

    public virtual void Die()
    {
        DelayDie();
        //QueueFree();
    }
    async void DelayDie()
    {
        engaged = false;
        ThreatArea.DisableArea();
        untargetable = true;
        Visible = false;
        await ToSignal(GetTree().CreateTimer(.5f), SceneTreeTimer.SignalName.Timeout);
        QueueFree();
    }
}
