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
		_jumpTimer = new();
		AddChild(_jumpTimer);
		_sitting = Texture;
		_jumping = GD.Load<Texture2D>("res://assets/cat_jumping.png");
		_falling = GD.Load<Texture2D>("res://assets/cat_falling.png");
		_jumpTimer.Timeout += Jump;

		GD.Print("jump timer started");
		// TODO figure out why timer keeps going
		_jumpTimer.Start(GetNextJumpTime());
	}

	private double GetNextJumpTime()
	{
		return 2;
	}
	private double GetJumpTime()
	{
		// TODO some formula based on cat difficulty value
		return 1;
	}

	private void Jump()
	{
		Texture = _jumping;
		int tileToJumpTo = Random.Next(_tiles.Count);
		while (_tiles[tileToJumpTo].TileStatus != CeilingTile.TileState.Stable)
		{
			tileToJumpTo = Random.Next(_tiles.Count);
		}
		_tween?.Kill();
		_tween = GetTree().CreateTween();
		_tween.TweenMethod(
			Callable.From((Vector2 tween) => Position = tween),
			Position, _tiles[tileToJumpTo].Position, 1.0f);
		_tween.Finished += Fall;
	}
	private void Fall()
	{
		_tween?.Kill();
		_tween = GetTree().CreateTween();
		Texture = _falling;
		_tween.TweenMethod(
			Callable.From((Vector2 tween) => Position = tween),
			Position, new Vector2(Position.X, Position.Y + 200), 1.0f);
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
					GD.Print("A click!");
				}
			}
		}
	}
	private void Clicked()
	{
		GD.Print("Clicked");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
