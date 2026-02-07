using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;

public partial class ResourceArea : SelectableStructure, IHasWorkers
{
	//[Export] public new ResourceAreaData BuildingData {get; protected set;}

	List<Villager> workers = new List<Villager>();
	int maxWorkers;
	public bool ReachedMaxCapacity => workers.Count == BuildingData.maxWorkers;

	bool sentHome;

    public override void _Ready()
    {
        GameManager.Instance.AddResourceArea(this);
		maxWorkers = BuildingData.maxWorkers;

		GameManager.Instance.Dawn += Awake;
		GameManager.Instance.Nighfall += SendWorkersHome;
		untargetable = true;
	}

	public void OnBodyEntered(Node2D body)
	{		
		if (body is Villager villager && villager.State == WorkerState.HeadingWork && villager.FoundArea.Name.Equals(this.Name))
		{
			RegisterWorker(villager);
		}
	}
		
	public void RegisterWorker(Villager worker)
	{
		Debug.Print($"Registred worker {worker.Name}");
		workers.Add(worker);

		worker.StartWork(((ResourceAreaData)BuildingData).extractionTime);
		FinishWork(worker);

		GuiControl.Instance.UpdateWorkers();
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

	async void FinishWork(Villager worker)
	{
		ResourceAreaData areaData = (ResourceAreaData)BuildingData;

		await ToSignal(GetTree().CreateTimer(areaData.extractionTime), SceneTreeTimer.SignalName.Timeout);

		if (sentHome) return;

		Debug.Print("Finished");
		FinishWork(worker, true);
	}

	void Awake()
	{
		sentHome = false;
	}
	void SendWorkersHome()
	{
		sentHome = true;
		for(int i = workers.Count - 1; i >= 0; i--)
		{
			FinishWork(workers[i], false);
		}
	}

	void FinishWork(Villager worker, bool completed)
	{
		ResourceAreaData areaData = (ResourceAreaData)BuildingData;

		workers.RemoveAt(workers.FindIndex(x => x.GetIndex() == worker.GetIndex()));

		ResourceData data = new() 
		{ 
			Resource = areaData.AreaResource,
			Quantity = completed ? areaData.resourceQuant : Mathf.CeilToInt(worker.WorkProgress / areaData.extractionTime * areaData.resourceQuant)
		};

		worker.CarryResource(data);

		worker.EndWork();

		GuiControl.Instance.UpdateWorkers();
	}
}
