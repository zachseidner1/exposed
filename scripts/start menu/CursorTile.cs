using Godot;
using System;

public partial class CursorTile : CeilingTile
{
	private Tween _followMouseTween;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Input.MouseMode = Input.MouseModeEnum.Hidden;
	}

	private double _timeSinceLastTween;
	private bool _isFalling;
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_timeSinceLastTween += delta;
		if (_timeSinceLastTween > 0.1 && !_isFalling)
		{
			_followMouseTween?.Kill();
			_followMouseTween = GetTree().CreateTween();
			_followMouseTween.TweenProperty(this, "position", GetViewport().GetMousePosition(), 1)
				.SetTrans(Tween.TransitionType.Elastic)
				.SetEase(Tween.EaseType.Out);
			_timeSinceLastTween = 0;
		}
	}
	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		if (@event is InputEventMouseButton inputEventMouseButton &&
		inputEventMouseButton.Pressed)
		{
			TweenTileHang(() =>
			{
				_isFalling = true;
				TweenTileFall(() =>
				{
					_isFalling = false;
					Rotation = 0;
					// Mouse position has probably changed by the time we got here, requery
					Position = GetViewport().GetMousePosition();
				});
			});
		}
	}
}
