using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class BuildingDetails : GuiElement
{
	[Export] public Label buildingNameLabel;
	[Export] public CanvasItem workersTab;
	[Export] public PackedScene workerSlotScene;
	
	[Export] public HealthBar healthBar;
	[Export] public CanvasItem healthTab;

	public SelectableStructure SelectedBuilding => selectedBuilding;
	SelectableStructure selectedBuilding;

	[Export] public CanvasItem WorkerImages;
	[Export] public Button HireButton;
//	[Export] public Button SpawnButton;
	[Export] public ButtonWithCost SpawnButton;
	[Export] public ButtonWithCost RepairButton;
	List<WorkerSlot> workerSlots = new List<WorkerSlot>();
	List<Villager> currentBuildingWorkers = new List<Villager>();


    public override void _Ready()
    {
        for	(int i = 0; i < WorkerImages.GetChildCount(); i++)
		{
			workerSlots.Add((WorkerSlot)WorkerImages.GetChild(i));
		}
    }

    public override void HideTween()
    {
        base.HideTween();
		//UnsetWorkers();
    }


	public void SelectBuilding(SelectableStructure building)
	{
		Debug.Print($"Select {building.Name}");
		buildingNameLabel.Text = building.BuildingData.buildingName;
		ShowTween();
		selectedBuilding?.Unselect();
		selectedBuilding = building;

		if (building.IsDamaged && !GameManager.Instance.IsNight)
		{
			SetRepair();
		}
		else RepairButton.Hide();

		if (!(building is ResourceArea))
		{
			healthTab.Show();
			UpdateBuildingHP(selectedBuilding.BuildingData.Stats.Health, selectedBuilding.BuildingData.Stats.MaxHealth);
		}
		else healthTab.Hide();

		if (building is IHasWorkers)  
		{
			workersTab.Show();

			UpdateWorkersSlots();
			if (building is WorkBuilding) HireButton.Show();
			else 
			{
				HireButton.Hide();
			}

			if (building is House) 
			{
				SetSpawn();				
			}
			else SpawnButton.Hide();

			UpdateWorkers();
		}
		else 
		{
			//UnsetWorkers();
			workersTab.Hide();
		}
	}

	void UpdateWorkersSlots()
	{
		int max = ((IHasWorkers)selectedBuilding).MaxWorkers();
		Texture2D villagerSkin = null;
		if (selectedBuilding is WorkBuilding workBuilding) villagerSkin = workBuilding.workerSkin;
		for (int i = 0; i < workerSlots.Count; i++)
		{
			if (i < max) workerSlots[i].Show();
			else workerSlots[i].Hide();

			workerSlots[i].UpdateSkin(villagerSkin);
		}
	}

	void SetSpawn()
	{
		SpawnButton.Show();
		House currentHouse = (House)selectedBuilding;
		SpawnButton.SetCosts([currentHouse.VillagerCost]);
	}
	void SetRepair()
	{
		RepairButton.Show();
		RepairButton.SetCosts(selectedBuilding.BuildingData.BuildingCosts);
	}

	public void OnPressSpawn()
	{
		Debug.Print("SPAWN VILLAGER");
		if (selectedBuilding is House house) house.SpawnVillager();
		UpdateWorkers();
	}

	public void OnPressHire()
	{
		Debug.Print("HIRE");
		if (selectedBuilding is WorkBuilding workBuilding) workBuilding.HireWorker();
		UpdateWorkers();
	}

	public void OnPressRepair()
	{
		RepairButton.Hide();
		selectedBuilding.Repair();
	}

	public void UpdateWorkers()
	{	
		if (!(selectedBuilding is IHasWorkers building)) return;

		currentBuildingWorkers.Clear();
		currentBuildingWorkers.AddRange(building.GetWorkers());	

		bool hasWorkProgress = selectedBuilding is ResourceArea;
		for (int i = workerSlots.Count-1; i >= 0; i--)
		{
			if (i < currentBuildingWorkers.Count) workerSlots[i].SetWorker(currentBuildingWorkers[i], hasWorkProgress);
			else workerSlots[i].UnsetWorker();
		}

		if (building.WorkersCount() == building.MaxWorkers())
		{
			HireButton.Disabled = true;
			//SpawnButton.Disabled = true;
			SpawnButton.ToggleButton(false);
		}
		else
		{
			HireButton.Disabled = false;
			//SpawnButton.Disabled = false;
			SpawnButton.ToggleButton(true);
		}
	}

	public void UpdateBuildingHP(float current, float max)
	{
		Debug.Print($"{current} / {max}");
		healthBar.UpdateBar(current, max, false);
	}
}
