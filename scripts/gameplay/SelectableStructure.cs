    using Godot;
using System;
using System.Diagnostics;

public partial class SelectableStructure : Area2D, IHasStats
{
	[Export] public BuildingData BuildingData {get; protected set;}
    [Export] public float BuildingRadius = 50f;
    public bool Selected {get;private set;}
    public bool IsUntargetable => untargetable;
    protected bool untargetable;
    public bool IsDestroyed {get;private set;}
    public bool IsDamaged => BuildingData.Stats.Health < BuildingData.Stats.MaxHealth;

    HealthBar healthBar;

    public override void _Ready()
    {
        base._Ready();

        BuildingData = BuildingData.Duplicate() as BuildingData;

        BuildingData.Init();

        SetHealthBar();
    }


	public void OnInputEvent(Viewport viewport, InputEvent inputEvent, long shapeIdx)
    {
        if (inputEvent is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
        {            
            Select();
        }
    }

    void Select()
    {
        Selected = true;
        GuiControl.Instance.SelectBuilding(this);
    }
    public void Unselect()
    {
        Selected = false;
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

        BuildingData.Stats.ChangeHealth(value);

        healthBar.UpdateBar(BuildingData.Stats.Health, BuildingData.Stats.MaxHealth);
        if (Selected) GuiControl.Instance.buildingDetails.UpdateBuildingHP(BuildingData.Stats.Health, BuildingData.Stats.MaxHealth);

        if (BuildingData.Stats.Health == 0) Destroy();
    }

    public void Repair()
    {
        healthBar.Visible = false;
        untargetable = false;
        IsDestroyed = false;
        ChangeHealth(9999);
    }

    public virtual void Destroy()
    {
        untargetable = true;
        IsDestroyed = true;
    }
}
