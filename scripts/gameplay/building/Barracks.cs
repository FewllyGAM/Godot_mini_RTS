using Godot;
using System;

public partial class Barracks : WorkBuilding
{
    protected override void ChangeSkin(Villager worker)
    {
        base.ChangeSkin(worker);
        worker.IsSoldier= true;
    }

}
