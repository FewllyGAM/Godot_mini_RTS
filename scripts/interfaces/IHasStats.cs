using Godot;
using System;
using System.Collections.Generic;

public interface IHasStats
{
    Vector2 Position {get;set;}
    bool IsUntargetable {get;}

    void SetHealthBar();
    void ChangeHealth(float v);
}
