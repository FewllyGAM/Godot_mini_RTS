using Godot;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

public enum WorkerState { Idle, HeadingHome, HeadingWork, HeadingWorkStation, Working }

public partial class Villager : Entity
{
    public House MyHouse;

    public bool Working {get; private set;}
    public bool IsSoldier {get;set;}
    WorkBuilding workBuilding;
    public WorkerState State => currentState;
    WorkerState currentState = WorkerState.Idle;
    public ResourceArea FoundArea {get; private set;}

    public float WorkProgress => workProgress;
    float workProgress = 0;
    float currentWorkTime;
    public WorkerSlot CurrentWorkerSlot {private get; set;}
    ResourceData carriedResource;

    public override void _Ready()
    {
        base._Ready();

        GameManager.Instance.Dawn += Awake;
        GameManager.Instance.Nighfall += TryGoHome;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsMouseButtonPressed(MouseButton.Right))
        {
            SetPath(GetGlobalMousePosition());
        }
        base._PhysicsProcess(delta);

        //PROGRESSO DO TRABALHO
        if (currentState == WorkerState.Working)
        {
            workProgress += (float)delta;
            if (CurrentWorkerSlot != null && FoundArea.Selected)
            {
                CurrentWorkerSlot.UpdateProgressBar(workProgress / currentWorkTime);
            }
        }
    }

    public void GoToDefaultPosition()
    {
        SetPath(MyHouse.InhabitantDefaultPosition);
    }

    ///Relacionado ao trabalho

    public void SetWork(WorkBuilding workStation = null)
    {
        if (workStation != null)
        {
            Working = true;
            workBuilding = workStation;
            navAgent.TargetDesiredDistance = workBuilding.BuildingRadius;
        }

        currentState = WorkerState.HeadingWorkStation;
        SetPath(workBuilding.Position);
    }

    protected override void OnNavigationFinished()
    {
        base.OnNavigationFinished();
        
        switch (currentState)
        {
            case WorkerState.HeadingHome:
                Visible = false;
                untargetable = true;
                currentState = WorkerState.Idle;
                break;
            case WorkerState.HeadingWorkStation:
                if (!GameManager.Instance.IsNight) GoToWork();
                else GoHome();
                if (workBuilding.ResourceBuilding)
                {
                    if (carriedResource != null)
                    { //Entrega o recurso pela metade se a construção estiver destruída
                        if (workBuilding.IsDestroyed) carriedResource.Quantity /= 2;
                        GameManager.Instance.ChangeResource(carriedResource);
                        carriedResource = null;
                    }
                }
                break;
            case WorkerState.HeadingWork:
                //
                break;
            default:
                break;
        }   
    }
    
    async void ResetCollider()
    {
        await ToSignal(GetTree().CreateTimer(.1f), SceneTreeTimer.SignalName.Timeout);
        collider.Disabled = false;
    }

    void TryGoHome()
    {
        if (currentState == WorkerState.Idle || currentState == WorkerState.HeadingWork) GoHome();
    }

    public void GoHome()
    {
        if (IsSoldier) return;

        if (!MyHouse.IsDestroyed)
        {
            currentState = WorkerState.HeadingHome;
            SetPath(MyHouse.Position);
        }
        else
        {
            GoToDefaultPosition();
        }
        
        collider.Disabled = true;
        ResetCollider();
    }

    void GoToWork()
    {
        if (workBuilding.ResourceBuilding)
        {
            FoundArea = GameManager.Instance.FindAvailableResourceArea(workBuilding.Resource);
            currentState = WorkerState.HeadingWork;
            SetPath(FoundArea.Position);
            collider.Disabled = true;
            ResetCollider();
        }
        else if (IsSoldier)
        {
            currentState = WorkerState.HeadingWork;
            SetPath(GameManager.Instance.PatrolArea.Position);
        }
    }

    public void StartWork(float workTime)
    {
        workProgress = 0;
        currentWorkTime = workTime;
        currentState =  WorkerState.Working;
        untargetable = true;
        Visible = false;
    }
    public void EndWork()
    {
        currentState = WorkerState.HeadingWorkStation;
        untargetable = false;
        Visible = true;
        SetPath(workBuilding.Position);
    }
    public void CarryResource(ResourceData resourceData)
    {
        carriedResource = resourceData;
    }

    public void Awake()
    {
        Debug.Print("ON AWAKE");
        if (IsSoldier)
        {
            SetPath(GameManager.Instance.PatrolArea.Position);
            return;
        }

        untargetable = false;
        Visible = true;
        if (Working && !GameManager.Instance.IsNight) SetWork();
        else GoToDefaultPosition();
    }

    public void ChangeSkin(Texture2D tex)
    {
        Sprite2D skin = GetNode<Sprite2D>("Sprite2D");
        skin.Texture = tex;
    }

    //Combate
    public override void Die()
    {
        GameManager.Instance.Dawn -= Awake;
        GameManager.Instance.Nighfall -= TryGoHome;

        untargetable = true;
        MyHouse.RemoveVillager(this);
        GameManager.Instance.RemoveVillager(this.Name);
        if (Working) workBuilding.RemoveWorker(this);
        base.Die();
    }

    public override void SetAttackTarget(Node2D target)
    {
        if (!IsSoldier) return;
        base.SetAttackTarget(target);
    }

}
