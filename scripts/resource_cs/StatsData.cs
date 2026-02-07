using Godot;
using System;
using System.ComponentModel;
using System.Diagnostics;

[GlobalClass]
public partial class StatsData : Resource
{
    [Export] public float MaxHealth;
    public float Health {get; private set;}

    [Export] public float AttackDamage;
    [Export] public float AttackRange = 15f;

    public void Init()
    {
        Debug.Print("INIT");
        Health = MaxHealth;
    }

    public float ChangeHealth(float value)
    {
        Health += value;
        Health = Mathf.Clamp(Health, 0, MaxHealth);

        //Mudar alguma barrinha se tiver, com tween
        return Health > 0 ? Health / MaxHealth : 0;
    }
}
