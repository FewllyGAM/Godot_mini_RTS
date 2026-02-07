using Godot;
using System;
using System.Runtime.InteropServices.Marshalling;

public partial class HealthBar : ColorRect
{
	Control healthBar;
	[Export] public Label healthValue;

    public override void _Ready()
    {
		healthBar = GetNode<Control>("health_bar");
	}

	public void UpdateBar(float current, float max, bool animate = true)
	{
		float rate = current/max;
		if (animate)
		{
			Tween tween = GetTree().CreateTween();
			tween.TweenProperty(healthBar, "scale", new Vector2(rate, 1), .1f);
		}
		else healthBar.Scale = new Vector2(rate, 1);

		if (healthValue != null)
		{
			healthValue.Text = $"{current}/{max}";
		}
	}
}
