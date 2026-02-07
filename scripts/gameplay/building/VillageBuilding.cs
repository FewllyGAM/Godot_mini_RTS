using Godot;
using System;
using System.Diagnostics;

public partial class VillageBuilding : SelectableStructure
{
    bool dragging = false;
    bool initializing = true;

    public override void _PhysicsProcess(double delta)
    {
        if (dragging) 
        {
            GlobalPosition = GetGlobalMousePosition();
            if (Input.IsMouseButtonPressed(MouseButton.Left) && !initializing)
            {
                dragging = false;
            }
        }
    }


    public void CreateBuilding()
    {
        dragging = true;
        //BuildingData = data;
    }
    void OnTimerTimeout()
    {
        initializing = false;
    }
}
