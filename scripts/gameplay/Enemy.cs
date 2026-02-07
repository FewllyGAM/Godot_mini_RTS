using Godot;
using System;
using System.Diagnostics;
using System.Threading;

public partial class Enemy : Entity
{
    //CollisionShape2D threatArea;
    public EnemySpawn EnemySpawn {get;set;}
    bool returning;

    public override void _Ready()
    {
        base._Ready();

        //threatArea = GetNode<CollisionShape2D>("Area2D/CollisionShape2D");
        SetDefaultRoute();

        GameManager.Instance.Dawn += ReturnToSpawner;
    }


    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);


    }

    protected override void OnNavigationFinished()
    {
        if (returning) Die();

        base.OnNavigationFinished();
    }


    protected override void ResearchNextTarget()
    {
        base.ResearchNextTarget();
        SetDefaultRoute();
    }
    // async void EnableThreatArea()
    // {
    //     await ToSignal(GetTree().CreateTimer(.1f), SceneTreeTimer.SignalName.Timeout);
    //     threatArea.Disabled = false;
    // }

    void SetDefaultRoute()
    {
        SetPath(((Node2D)GameManager.Instance.buildingsRoot.GetChild(0)).Position);
    }

    // void OnScanTarget(Node2D body)
    // {   
    //     Debug.Print($"SCANNED {body.Name}");
    //     if (engaged) return;

    //     SetAttackTarget(body);
    // }
    // void OnScanArea(Area2D area)
    // {
    //     Debug.Print($"SCANNED {area.Name}");
    //     if (engaged) return;

    //     SetAttackTarget(area);
    // }

    void ReturnToSpawner()
    {
        returning = true;
        engaged = false;
        //threatArea.Disabled = true;
        ThreatArea.DisableArea();
        SetPath(EnemySpawn.Position);
    }

    public override void Die()
    {
        GameManager.Instance.Dawn -= ReturnToSpawner;

        base.Die();
    }

}
