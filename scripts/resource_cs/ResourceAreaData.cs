using Godot;
using System;

[GlobalClass]
public partial class ResourceAreaData : BuildingData
{
    [Export] public VillageResource AreaResource;
	[Export] public float extractionTime = 25f;
    [Export] public int resourceQuant = 15;
}
