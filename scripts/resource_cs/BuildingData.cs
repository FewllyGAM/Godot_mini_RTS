using Godot;
using System;

[GlobalClass]
public partial class BuildingData : Resource
{
    [Export] public string buildingName;
    [Export] public string buildingScene;
    [Export] public StatsData Stats = new StatsData();
    [Export] public ResourceData[] BuildingCosts {get;set;}

    //Relacionado a construções com trabalhadores
    [Export] public int maxWorkers = 2;

    public void Init()
    {
        Stats = Stats.Duplicate() as StatsData;
        Stats.Init();
    }

    public PackedScene GetScene()
    {
        return GD.Load<PackedScene>($"res://scenes/buildings/{buildingScene}.tscn");
    }
}
