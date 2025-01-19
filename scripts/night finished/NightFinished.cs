using Godot;
using System;

public partial class NightFinished : Control
{

	[Export]
	public ColorRect PostProcessing;

	[Export]
	public Label Hour6;

	[Export]
	public Button Continue;

	[Export]
	public Button Quit;

	[Export]
	public CursorTile MyCursorTile;

	[Export]
	public AudioStreamPlayer NightFinishedSoundEffect;

	[Export]
	public AudioStreamPlayer NextLevelSoundEffect;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		EmptyScene();
		Timer timer = new() { OneShot = true };
		AddChild(timer);
		timer.Start(0.5);
		timer.Timeout += () =>
		{
			NightFinishedSoundEffect.Play();
		};
		// NightFinishedSoundEffect.Play();
		RepopulateScene();
		Saving.WriteLevel(Saving.GetLevel() + 1);
	}
	/// <summary>
	/// Make all items in scene invisible, and remove post processing
	/// </summary>
	private void EmptyScene()
	{
		Hour6.Modulate = new(0, 0, 0);
		Continue.Visible = false;
		PostProcessing.Visible = false;
		Quit.Visible = false;
		MyCursorTile.Visible = false;
	}

	private void RepopulateScene()
	{
		var tween = GetTree().CreateTween();
		tween.TweenMethod(
			Callable.From((Color color) => { Hour6.Modulate = color; }),
			new Color(0, 0, 0, 0),
			 new Color(1, 1, 1, 1),
				4.0);
		tween.Finished += () =>
		{
			Continue.Visible = true;
			PostProcessing.Visible = true;
			Quit.Visible = true;
			MyCursorTile.Visible = true;
		};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnContinuePressed()
	{
		GD.Print("Help me");
		NextLevelSoundEffect.Play();

		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(this, "modulate", new Color(1, 1, 1, 0), 3.0)
			.SetTrans(Tween.TransitionType.Cubic)
			.SetEase(Tween.EaseType.Out);

		// TODO increment level counter 

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
