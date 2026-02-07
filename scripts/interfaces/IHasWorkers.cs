using Godot;
using System;
using System.Collections.Generic;

public interface IHasWorkers
{
    int MaxWorkers();
    int WorkersCount();
    List<Villager> GetWorkers();
}
