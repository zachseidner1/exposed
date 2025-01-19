using Godot;
using System;
using System.Collections.Generic;

public partial class SfxPlayer : AudioStreamPlayer
{
	[Export]
	public AudioStream VentCrawl;

	[Export]
	public AudioStream MetalClang;

	private double _elapsedTime;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_elapsedTime += delta;
		if (_elapsedTime >= 10)
		{
			_elapsedTime = 0;
			int chosenSound = Random.Next(2);

			Stream = chosenSound == 1 ? VentCrawl : MetalClang;
			float start = chosenSound == 1 ? 13.7F : 0F;
			if (Random.Next(5) == 0)
			{
				GD.Print("playing sound???");
				Play(start);
			}
			else
			{
				GD.Print("Failed check");
			}
		}
	}
}
