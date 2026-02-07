using Godot;
using System;
using System.Collections.Generic;

public partial class House : VillageBuilding, IHasWorkers
{
    [Export] public bool IsFirstHouse;

    [Export] public ResourceData VillagerCost = new ResourceData();

    List<Villager> inhabitants = new List<Villager>();
    int maxInhabitants;

    public Vector2 InhabitantDefaultPosition => Position + Vector2.Down * 45;

    public override void _Ready()
    {
         base._Ready();

        maxInhabitants = BuildingData.maxWorkers;

        if (IsFirstHouse) SpawnVillager();
    }

    public void SpawnVillager()
    {
        if (inhabitants.Count == maxInhabitants) return;

        //if (!free) GameManager.Instance.SpendCosts(VillagerCost);

        Villager villager = GameManager.Instance.SpawnVillager(Position);
        inhabitants.Add(villager);
        villager.MyHouse = this;

        villager.GoToDefaultPosition();
    }
    public void RemoveVillager(Villager villager)
    {
        int idx = inhabitants.FindIndex(x => x.Name.Equals(villager.Name));
        if (idx >= 0) inhabitants.RemoveAt(idx);

        if (Selected) GuiControl.Instance.UpdateWorkers();
    }

    public override void Destroy()
    {
        base.Destroy();
        foreach (Villager villager in inhabitants)
        {
            villager.Awake();
        }
    }


    public List<Villager> GetWorkers()
    {
        return inhabitants;
    }

    public int MaxWorkers()
    {
        return maxInhabitants;
    }

    public int WorkersCount()
    {
        return inhabitants.Count;
    }
}
