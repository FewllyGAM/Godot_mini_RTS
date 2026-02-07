using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class ButtonWithCost : Control
{
	protected ResourceData[] costs;

	[Export] public Button button;
	[Export] public CanvasItem resourceCostArea;
	protected List<ResourceDisplay> costsDiplay = [];
	bool initialized;


	public void SetCosts(ResourceData[] costs)
	{
		if (!initialized)
		{
			foreach (Node node in resourceCostArea.GetChildren())
			{
				costsDiplay.Add((ResourceDisplay)node);
			}
			initialized = true;
		}

		this.costs = costs;
		button.Disabled = false;

		for (int i = 0; i < costsDiplay.Count; i++)
		{
			costsDiplay[i].SetMissing(false);
			if (i < costs.Length)
			{
				costsDiplay[i].Show();
				costsDiplay[i].ChangeResource(costs[i].Resource);
				costsDiplay[i].Update(costs[i].Quantity);
			}
			else costsDiplay[i].Hide();
		}

		GameManager.Instance.ResourcesChanged += CheckCosts;
		CheckCosts(null);
	}

	public void CheckCosts(ResourceData data)
	{
		if (!GameManager.Instance.CheckAvailableResources(costs))
		{
			button.Disabled = true;
		}
		else button.Disabled = false;

		for (int i = 0; i < costsDiplay.Count; i++)
		{
			if (i < costs.Length) costsDiplay[i].SetMissing(costs[i].IsMissing);
		}
	}

	public virtual void OnButtonDown()
	{
		GameManager.Instance.SpendCosts(costs);
	}

	public void ToggleButton(bool v)
	{
		button.Disabled = !v;
	}
}
