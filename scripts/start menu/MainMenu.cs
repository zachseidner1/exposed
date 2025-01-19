using Godot;
using System;

public partial class MainMenu : MarginContainer
{

	[Export]
	public AudioStreamPlayer Bgm;
	[Export]
	public AudioStreamPlayer Sfx;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnNewGamePressed()
	{
		Sfx.Play();
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(this, "modulate", new Color(1, 1, 1, 0), 3.0)
			.SetTrans(Tween.TransitionType.Cubic)
			.SetEase(Tween.EaseType.Out);
		tween.Parallel().TweenMethod(Callable.From((float volume) =>
		{
			Bgm.VolumeDb = volume;
		}), 0, -30, 3.0);
		tween.Finished += () =>
		{
			GetTree().ChangeSceneToFile("res://scenes/CeilingTileGame.tscn");
		};
	}

	private void OnQuitPressed()
	{
		GetTree().Quit();
	}
}
