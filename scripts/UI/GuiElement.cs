using Godot;
using System;

public partial class GuiElement : Control
{
	public virtual void ShowTween()
	{
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(this, "modulate:a", 1.0f, .2f);
		tween.Finished += () => Show();
	}

	public virtual void HideTween()
	{
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(this, "modulate:a", 0.0f, .2f);
		tween.Finished += () => Hide();
	}
}
