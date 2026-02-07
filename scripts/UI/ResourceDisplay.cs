using Godot;
using System;

public partial class ResourceDisplay : Control
{
	[Export] public VillageResource Resource;
	[Export] public TextureRect texture;
	[Export] public Label quantity;

    // public override void _EnterTree()
    // {
    //     quantity = GetNode<Label>("quantity");
    // }

	public void ChangeResource(VillageResource newResource)
	{
		Resource = newResource;
		texture.Texture = GuiControl.Instance.VillageResourceIcons[(int)newResource];
	}

	public void Update(int value)
	{
		quantity.Text = value.ToString();
	}

	public void SetMissing(bool isMissing)
	{
		if (isMissing) quantity.AddThemeColorOverride("font_color", new Color(1, 0, 0));
		else quantity.RemoveThemeColorOverride("font_color");
	}
}
