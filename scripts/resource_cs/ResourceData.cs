using Godot;
using System;
using System.ComponentModel;

[GlobalClass]
public partial class ResourceData : Resource
{
    [Export] public VillageResource Resource {get;set;}
    [Export] public int Quantity {get;set;}

    public bool IsMissing {get;set;}

    // public ResourceData(VillageResource r, int c)
    // {
    //     Resource = r;
    //     Quantity = c;
    // }
    public void ChangeQuantity(int value)
    {
        Quantity += value;
    }
}
