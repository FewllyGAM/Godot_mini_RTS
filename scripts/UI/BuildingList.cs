using Godot;
using System;
using System.Collections.Generic;

public partial class BuildingList : GuiElement
{
	[Export] public PackedScene buildingButton;

	public override void _Ready()
	{
		List<BuildingData> allBuildings = [.. GameManager.Instance.availableBuildings];

		foreach (BuildingData building in allBuildings)
		{
			BuildingButton newBuildingButton = (BuildingButton)buildingButton.Instantiate();
			newBuildingButton.SetBuilding(building);
			GetChild(0).AddChild(newBuildingButton);
		}
	}
}
