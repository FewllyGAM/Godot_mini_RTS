using Godot;
using System;
using System.Diagnostics;
using System.Reflection;

public partial class WorkerSlot : TextureRect
{
	// [Export] public Texture2D defaultVillager;
	Texture2D defaultVillager;
	CanvasItem notHiredPanel;

	[Export] public CanvasItem progress;
	Control progressBar;

	Villager currentWorker;

    public override void _Ready()
    {
        notHiredPanel = GetNode<CanvasItem>("nothired");
		progressBar = progress.GetNode<Control>("work_progress");

		defaultVillager = Texture;
	}


	public void SetWorker(Villager worker, bool hasProgress)
	{
		//if (currentWorker != null) {currentWorker.CurrentWorkerSlot = null; Debug.Print("SET WORKER UNSET");}
		currentWorker = worker;
		currentWorker.CurrentWorkerSlot = this;

		notHiredPanel.Hide();

		if (hasProgress)
		{
			progress.Show();
		}
		else progress.Hide();
	}

	public void UnsetWorker()
	{
		//if (currentWorker != null) { currentWorker.CurrentWorkerSlot = null; Debug.Print("UNSET WORKER UNSET");}
		currentWorker = null;

		notHiredPanel.Show();
		progress.Hide();
	}

	public void UpdateProgressBar(float rate)
	{
		//Debug.Print($"PROGRESS {rate} WORKER {currentWorker.Name}");
		progressBar.Scale = new Vector2(rate, 1.0f);
	}

	public void UpdateSkin(Texture2D texture)
	{
		if (texture == null) 
		{
			Texture = defaultVillager;
			return;
		}
		this.Texture = texture;
	}
}
