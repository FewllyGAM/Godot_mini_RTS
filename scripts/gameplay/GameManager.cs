using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class GameManager : Node
{
    public static GameManager Instance { get; private set; }

    [Export] public DirectionalLight2D sun;
    [Export] public float maxSunForce;
    [Export] public float dayDuration;
    [Export] public Curve dayCurve = new Curve();
    float currentTime = 0;
    public bool IsNight {get;private set;}

    List<ResourceData> villageResources = new List<ResourceData>();

    int population = 0;
    int maxPopulation = 1;

    [Export] public PackedScene villager;

    [Export] public BuildingData[] availableBuildings;

    [Export] public Node buildingsRoot;

    List<ResourceArea> resourceAreas = [];
    List<EnemySpawn> enemySpawners = [];

    List<Villager> villagers = [];
    [Export] public Node2D PatrolArea;
    [Export] public int[] EnemiesPerDay;
    public int Day => day;
    int day = 0;
    public int CurrentEnemyCount;

    bool fastForwarded;

    public override void _EnterTree()
    {
        if (Instance != null) Instance = null;
        Instance = this;        
    }

    public override void _Ready()
    {
        //SpawnVillager(Vector2.Zero);

        int resourceCount = Enum.GetNames(typeof(VillageResource)).Length;
        for (int i = 0; i < resourceCount; i++)
        {
            villageResources.Add(new() {Resource = (VillageResource)i, Quantity = 50});           
        }
        UpdateResources();
        day++;
        Debug.Print($"Day {day}");
    }

    async void UpdateResources()
    {
        await ToSignal( GetTree().CreateTimer(.05f), SceneTreeTimer.SignalName.Timeout);
        GuiControl.Instance.UpdateResources(villageResources);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsKeyPressed(Key.F)) FastForward();

        currentTime += (float)delta;
        float dayNightRate = dayCurve.Sample(currentTime / dayDuration);
        sun.Energy = maxSunForce * (IsNight ? dayNightRate : (1 - dayNightRate));

        if (currentTime >= dayDuration)
        {
            currentTime = 0;
            IsNight = !IsNight;
            if (!IsNight)
            {
                day++;
                Debug.Print($"Day {day}");
                Dawn?.Invoke();
            }
            else Nighfall?.Invoke();

            if (CurrentEnemyCount < EnemiesPerDay.Length && day == EnemiesPerDay[CurrentEnemyCount])
            {
                CurrentEnemyCount++;
                foreach (EnemySpawn spawner in enemySpawners)
                {
                    spawner.EnemiesQuantity = CurrentEnemyCount;
                }
            }
        }
    }

    //Adiciona uma área de recurso a lista global
    public void AddResourceArea(ResourceArea resourceArea)
    {
        resourceAreas.Add(resourceArea);
    }
    public ResourceArea FindAvailableResourceArea(VillageResource resource)
    {
        return resourceAreas.Find(x => !x.ReachedMaxCapacity && (((ResourceAreaData)x.BuildingData).AreaResource == resource));
    }

    public void IncreaseMaxPopulation(int value)
    {
        maxPopulation += value;
    }

    //Adiciona um spawner de inimigo a lista global
    public void AddEnemySpawner(EnemySpawn spawner)
    {
        enemySpawners.Add(spawner);
    }

    //Cria um aldeão em uma construção especificada
    public Villager SpawnVillager(Vector2 pos)
    {
        Debug.Print("ADD NEW VILLAGER");
        Villager newVillager = (Villager)villager.Instantiate();
        GetChild(0).AddChild(newVillager);

        newVillager.GlobalPosition = pos;
        population += 1;

        villagers.Add(newVillager);
        return newVillager;
    }
    public void RemoveVillager(string villagerName)
    {
        int i = villagers.FindIndex(x => x.Name.Equals(villagerName));
        if (i >= 0) villagers.RemoveAt(i);
    }

    //Busca um aldeão desempregado
    public Villager FindUnployedVillager()
    {
        return villagers.Find(x => !x.Working);
    }

    //Adiciona ou diminui uma quantidade de um recurso específico
    public void ChangeResource(ResourceData data)
    {
        villageResources[(int)data.Resource].ChangeQuantity(data.Quantity);

        GuiControl.Instance.UpdateResources(villageResources);
        ResourcesChanged?.Invoke(data);
    }

    public bool CheckAvailableResources(ResourceData[] cost, bool spend = false)
    {
        bool isMissing = false;
        foreach (ResourceData data in cost)
        {
            if (data.Quantity > villageResources[(int)data.Resource].Quantity)
            {
                data.IsMissing = true;
                isMissing = true;
            }
            else data.IsMissing = false;
        }
        return !isMissing;
    }

    public void SpendCosts(ResourceData[] costs)
    {
        foreach (ResourceData data in costs)
        {
            ResourceData newData = data.Duplicate() as ResourceData;
            newData.Quantity *= -1;
            ChangeResource(newData);
        }
    }
    public void SpendCosts(ResourceData cost)
    {
        ResourceData newData = cost.Duplicate() as ResourceData;
        newData.Quantity *= -1;
        ChangeResource(newData);
    }

    public void FastForward()
    {
        if (!fastForwarded) Engine.TimeScale = 2.5f;
        else Engine.TimeScale = 1.0f;
        fastForwarded = !fastForwarded;
    }
    ///TODO
    /// * Portais inimigos estão espalhados pelo mapa, e são destrutíveis no final do jogo
    /// (Criar script de wander, vagar por ai aleatoriamente)
    /// * Criar um botão no portal do inimigo, disponível só quando o player tiver 4 soldados. Envia os soldados para destruir o portal
    /// que quando estiver sob ataque, spawna inimigos aos poucos

    #region  Events
	//EVENTOS

    public Action<ResourceData> ResourcesChanged;
    public Action Dawn;
    public Action Nighfall;

    // public void OnResourcesChanged(ResourceData data)
    // {
    //     ResourcesChanged?.Invoke(data);
    // }

	#endregion
}

public enum VillageResource { Wood, Food, Stone }