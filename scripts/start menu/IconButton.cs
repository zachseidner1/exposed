using Godot;
using System;

public partial class IconButton : Button
{
	[Export]
	public Texture2D IconTexture;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Icon = null;
	}
	private void OnMouseEntered()
	{
		Icon = IconTexture;
	}

	private void OnMouseExited()
	{
		Icon = null;
	}



	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

}
