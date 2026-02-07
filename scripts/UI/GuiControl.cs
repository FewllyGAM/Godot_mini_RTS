using Godot;
using System;
using System.Collections.Generic;

public partial class GuiControl : Control
{
	public static GuiControl Instance;

	[Export] public Label dayLabel;
	[Export] public Texture2D[] VillageResourceIcons;

	[Export] public BuildingList buildingsList;
	[Export] public BuildingDetails buildingDetails;

	[Export] public CanvasItem resourcesTab;
	List<ResourceDisplay> resources = new List<ResourceDisplay>();

    public override void _EnterTree()
    {
        if (Instance != null) Instance = null;
		Instance = this;

		for (int i = 0; i < resourcesTab.GetChildren().Count; i++)
		{
			resources.Add((ResourceDisplay)resourcesTab.GetChildren()[i]);
			resources[i].ChangeResource((VillageResource)i);
		}
    }

    public override void _Ready()
    {
        GameManager.Instance.Dawn += UpdateDay;
    }


    public override void _Process(double delta)
    {
        if (Input.IsKeyLabelPressed(Key.Escape))
		{
			UnselectBuilding();
		}
    }


	public void SelectBuilding(SelectableStructure building)
	{
		buildingDetails.SelectBuilding(building);
		buildingsList.HideTween();
	}
	public void UnselectBuilding()
	{	
		buildingDetails.SelectedBuilding?.Unselect();
		buildingDetails.HideTween();
		buildingsList.ShowTween();
	}
	public void UpdateWorkers()
	{
		buildingDetails.UpdateWorkers();
	}

	//Atualiza o lugar onde mostra os recursos da vila
	public void UpdateResources(List<ResourceData> allResources)
	{
		for (int i = 0; i < resources.Count; i++)
		{
			resources[i].Update(allResources[i].Quantity);
		}
	}

	public void UpdateDay()
	{
		dayLabel.Text = $"Dia {GameManager.Instance.Day}";
	}

	#region  Events
	//EVENTOS

	#endregion
}
