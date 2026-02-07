using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class WorkBuilding : VillageBuilding, IHasWorkers
{
    [Export] public bool ResourceBuilding;
    [Export] public VillageResource Resource;
    [Export] public Texture2D workerSkin;

    List<Villager> workers = new List<Villager>();
    int maxWorkers;

    public override void _Ready()
    {
        base._Ready();
        maxWorkers = BuildingData.maxWorkers;
    }


    public virtual bool HireWorker()
    {
        if (workers.Count == BuildingData.maxWorkers) return false;

        Villager newWorker = GameManager.Instance.FindUnployedVillager();
        if (newWorker != null)
        {
            workers.Add(newWorker);
            newWorker.SetWork(this);
            ChangeSkin(newWorker);

            //GuiControl.Instance.UpdateWorkers();
            return true;
        }
        return false;
    }

    public void RemoveWorker(Villager villager)
    {
        int idx = workers.FindIndex(x => x.Name.Equals(villager.Name));
        if (idx >= 0) workers.RemoveAt(idx);

        if (Selected) GuiControl.Instance.UpdateWorkers();
    }

    protected virtual void ChangeSkin(Villager worker)
    {
        if (workerSkin != null) worker.ChangeSkin(workerSkin);
    }

    public int WorkersCount()
    {
        return workers.Count;
    }
    public int MaxWorkers()
    {
        return maxWorkers;
    }
    public List<Villager> GetWorkers()
	{
		return workers;
	}
}
