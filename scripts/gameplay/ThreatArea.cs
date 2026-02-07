using Godot;
using System;
using System.Diagnostics;

public partial class ThreatArea : Area2D
{
    CollisionShape2D area;
    Entity entity;

    public override void _Ready()
    {
        area = GetNode<CollisionShape2D>("CollisionShape2D");
        entity = GetParent<Entity>();
        entity.ThreatArea = this;
    }

    void OnScanTarget(Node2D body)
    {
        Debug.Print($"SCANNED {body.Name}");

        entity.SetAttackTarget(body);
    }
    void OnScanArea(Area2D area)
    {
        Debug.Print($"SCANNED {area.Name}");

        entity.SetAttackTarget(area);
    }

    public void DisableArea()
    {
        area.Disabled = true;
    }
    public async void EnableThreatArea()
    {
        await ToSignal(GetTree().CreateTimer(.1f), SceneTreeTimer.SignalName.Timeout);
        area.Disabled = false;
    }
}
