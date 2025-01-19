using Godot;
using System;

public partial class MainMenu : MarginContainer
{

	[Export]
	public AudioStreamPlayer Bgm;
	[Export]
	public AudioStreamPlayer Sfx;

	[Export]
	public Button ContinueButton;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (Saving.GetLevel() != 1)
		{
			ContinueButton.Disabled = false;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnNewGamePressed()
	{
		Saving.WriteLevel(1);
		OnContinuePressed();
	}
	private void OnContinuePressed()
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
