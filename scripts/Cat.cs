using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

public partial class Cat : Sprite2D
{

	public enum CatState
	{
		Sitting,
		Jumping,
		Falling,
	}

	[Export(PropertyHint.Range, "0,20,1")]
	public int CatDifficulty { get; set; } = 1;
	public CatState CatStatus = CatState.Sitting;

	private List<CeilingTile> _tiles = new();

	private Timer _jumpTimer;

	private Tween _tween;

	private Texture2D _jumping;
	private Texture2D _falling;
	private Texture2D _sitting;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_tiles = GetTree().GetNodesInGroup("tiles").Cast<CeilingTile>().ToList();
		_jumpTimer = new()
		{
			OneShot = true
		};
		AddChild(_jumpTimer);
		_sitting = GD.Load<Texture2D>("res://assets/cat_sitting.png");
		_jumping = GD.Load<Texture2D>("res://assets/cat_jumping.png");
		_falling = GD.Load<Texture2D>("res://assets/cat_falling.png");

		_jumpTimer.Timeout += Jump;

		_jumpTimer.Start(GetNextJumpTime());
	}

	private double GetNextJumpTime()
	{
		return 2;
	}
	private double GetJumpTime()
	{
		// TODO some formula based on cat difficulty value
		return 2;
	}

	private void Jump()
	{
		CatStatus = CatState.Jumping;
		Texture = _jumping;

		int tileToJumpTo = Random.Next(_tiles.Count);
		while (_tiles[tileToJumpTo].TileStatus != CeilingTile.TileState.Stable)
		{
			tileToJumpTo = Random.Next(_tiles.Count);
		}

		Vector2 targetPosition = _tiles[tileToJumpTo].Position;
		targetPosition.X += _tiles[tileToJumpTo].Width / 2;
		targetPosition.Y += _tiles[tileToJumpTo].Height / 2;


		// Flip if travelling left
		FlipH = targetPosition.X < Position.X;
		if (targetPosition.X < Position.X)
		{
			Offset = new Vector2(16, Offset.Y);
		}
		else
		{
			Offset = new Vector2(-16, Offset.Y);
		}

		_tween?.Kill();
		_tween = GetTree().CreateTween();
		_tween.TweenMethod(
			Callable.From((Vector2 pos) => Position = pos),
			Position, targetPosition, 1.0f);
		_tween.Finished += Fall;
		_tween.Finished += () => { _tiles[tileToJumpTo].FallTile(); };
	}
	private void Fall()
	{
		CatStatus = CatState.Falling;
		_tween?.Kill();
		_tween = GetTree().CreateTween();
		Texture = _falling;
		_tween.TweenMethod(
			Callable.From((Vector2 pos) => Position = pos),
			Position, new Vector2(Math.Clamp(Position.X, 144, 456), 370), 1.0f);
		_tween.Finished += () =>
		{
			Texture = _sitting;
		};
		_jumpTimer.Start(GetNextJumpTime());
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		if (@event is InputEventMouseButton inputEventMouse)
		{
			if (inputEventMouse.Pressed && inputEventMouse.ButtonIndex == MouseButton.Left)
			{
				if (GetRect().HasPoint(ToLocal(inputEventMouse.Position)))
				{
					Clicked();
				}
			}
		}
	}
	private void Clicked()
	{
		if (CatStatus == CatState.Jumping)
		{
			Fall();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
