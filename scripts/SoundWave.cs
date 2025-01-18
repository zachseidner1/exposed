using Godot;
using System;

public partial class SoundWave : Sprite2D
{
	private int _waveCount = 1;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void AnimateWave()
	{
		if (_waveCount == 3)
		{
			Visible = false;
			_waveCount = 1;
			return;
		}
		var tween = GetTree().CreateTween();
		Position = new(30, -2);
		Scale = new(1, 1);
		Modulate = new(1, 1, 1, 1);
		Visible = true;
		tween.TweenProperty(this, "position", new Vector2(46, -7), 1.0);
		tween.Parallel().TweenProperty(this, "scale", new Vector2(2, 2), 1.0);
		tween.Parallel().TweenProperty(this, "modulate", new Color(1, 1, 1, 0), 1.0);
		tween.Finished += AnimateWave;

		_waveCount++;
	}
}
