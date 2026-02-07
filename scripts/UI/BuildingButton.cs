using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class BuildingButton : ButtonWithCost
{
	PackedScene building;
	BuildingData buildingData;

	//[Export] public Button button;
	//[Export] public CanvasItem resourceCostArea;
	//[Export] public PackedScene resourceDisplay;
	//List<ResourceDisplay> costsDiplay = new List<ResourceDisplay>();

	public void SetBuilding(BuildingData data)
	{
		button.Text = data.buildingName;
		building = data.GetScene();
		buildingData = data;
		SetCosts(data.BuildingCosts);

		// foreach (ResourceData cost in buildingData.BuildingCosts)
		// {
		// 	ResourceDisplay display = (ResourceDisplay)resourceDisplay.Instantiate();
		// 	resourceCostArea.AddChild(display);
		// 	costsDiplay.Add(display);
			
		// 	display.ChangeResource(cost.Resource);
		// 	display.Update(cost.Quantity);
		// }

		// GameManager.Instance.ResourcesChanged += CheckCosts;
		// CheckCosts(null);
	}

	// public void CheckCosts(ResourceData data)
	// {
	// 	if (!GameManager.Instance.CheckAvailableResources(buildingData.BuildingCosts))
	// 	{
	// 		Debug.Print($"NO RESOURCES FOR {buildingData.buildingName}");
	// 		button.Disabled = true;
	// 	}
	// 	else button.Disabled = false;

	// 	for (int i = 0; i < costsDiplay.Count; i++)
	// 	{
	// 		costsDiplay[i].SetMissing(buildingData.BuildingCosts[i].IsMissing);
	// 	}
	// }

	public override void OnButtonDown()
	{
		//GameManager.Instance.SpendCosts(buildingData.BuildingCosts);
		base.OnButtonDown();

		VillageBuilding newBuilding = (VillageBuilding)building.Instantiate();
		GameManager.Instance.buildingsRoot.AddChild(newBuilding);
		newBuilding.CreateBuilding();
	}
}
