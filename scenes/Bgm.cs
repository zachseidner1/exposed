using Godot;
using System;

public partial class Bgm : AudioStreamPlayer2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// This should loop the audio, but I have not tested this lol.
		Finished += () => { Play(); };
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
