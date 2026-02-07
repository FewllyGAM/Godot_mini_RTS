using Godot;
using System;
using System.Diagnostics;

public partial class EnemySpawn : SelectableStructure
{
    public int EnemiesQuantity {get;set;}
    [Export] public PackedScene enemyScene;

    Timer spawnTimer;
    int spawnedQuant;

    public override void _Ready()
    {
        base._Ready();

        spawnTimer = GetNode<Timer>("spawn_timer");

        GameManager.Instance.Nighfall += SpawnHorde;
        GameManager.Instance.AddEnemySpawner(this);
        EnemiesQuantity = 0;

        untargetable = true;
    }

    void SpawnHorde()
    {
        spawnedQuant = 0;
        spawnTimer.Start();
    }

    void OnTimeout()
    {
        if (EnemiesQuantity == 0) return;
        Enemy enemy = SpawnEnemy();
        enemy.EnemySpawn = this;
        spawnedQuant++;
        if (spawnedQuant < EnemiesQuantity) spawnTimer.Start();
        Debug.Print($"{spawnedQuant} < {EnemiesQuantity}");
    }

    Enemy SpawnEnemy()
    {
        Debug.Print("SPAWN ENEMY");
        Enemy newEnemy = (Enemy)enemyScene.Instantiate();
        GetParent().AddChild(newEnemy);

        newEnemy.GlobalPosition = Position + Vector2.Down * 45;

        //villagers.Add(newEnemy);
        return newEnemy;
    }
}
