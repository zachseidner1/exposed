using Godot;
using System;

public partial class Bat : Sprite2D
{
	private Vector2 finalPosition = new(338, 218);
	private Vector2 finalScale = new(11.685F, 11.685F);
	private Tween _tween;

	[Export]
	public AudioStream BatScare;

	private AudioStreamPlayer2D Player;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Visible = false;
		Player = new();
		Player.Stream = BatScare;
		AddChild(Player);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void ScarePlayer(Vector2 origin)
	{
		Player.Play();
		Position = origin;
		Scale = new(1, 1);
		Visible = true;
		_tween?.Kill();
		_tween = GetTree().CreateTween();
		_tween.SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Quart);
		_tween.TweenProperty(this, "position", finalPosition, 1);
		_tween.Parallel().TweenProperty(this, "scale", finalScale, 1);
		_tween.Finished += () => { Visible = false; GetTree().ChangeSceneToFile("res://scenes/GameOver.tscn"); };
	}
}
