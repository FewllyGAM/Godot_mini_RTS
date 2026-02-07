using Godot;
using System;

public partial class CameraController : Node2D
{
	[Export] public float speed = 150;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		int verticalDir = 0, horizontalDir = 0;

		if (Input.IsActionPressed("up"))
		{
			verticalDir = -1;
		}
		else if (Input.IsActionPressed("down"))
		{
			verticalDir = 1;
		}

		if (Input.IsActionPressed("right"))
		{
			horizontalDir = 1;
		}
		else if (Input.IsActionPressed("left"))
		{
			horizontalDir = -1;
		}

		Vector2 cameraVelocity = new Vector2(horizontalDir, verticalDir);
		Position = Position + cameraVelocity * (float)delta * speed;
	}
}
