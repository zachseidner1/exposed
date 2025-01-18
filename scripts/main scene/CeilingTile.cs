using Godot;
using System;
using System.Collections.ObjectModel;
using System.Data.SqlTypes;
using System.Linq;


public partial class CeilingTile : Polygon2D
{

	public enum TileState
	{
		Hanging,
		Fallen,
		Stable,
	}

	public TileState TileStatus { get; private set; } = TileState.Stable;
	public Tween MyTween;

	private Timer _fallTimer;

	private AudioStreamPlayer2D _player;

	[Export]
	public AudioStream TileHangSound;

	[Export]
	public AudioStream TileFallSound;

	[Export]
	public AudioStream TilePickupSound;

	/// <summary>
	/// Signal ran whenever a ceiling tile falls
	/// </summary>
	/// <param name="tileCenter">The global coordinates of the center of the fallen ceiling tile.</param>
	[Signal]
	public delegate void CeilingTileFallenEventHandler(Vector2 tileCenter);

	/// <summary>
	/// Not guaranteed to be a vertice, a vector that contains the minimum x and 
	/// maximum y of the vertices of the polygon
	/// </summary>
	private Vector2 _bottomLeftPosition;
	/// <summary>
	/// Not guaranteed to be a vertice, a vector that contains the maximum x and 
	/// minimum y of the vertices of the polygon
	/// </summary>
	private Vector2 _topRightPosition;

	public float Width { get; private set; }
	public float Height { get; private set; }

	public Vector2 Center { get; private set; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Initialize timer
		_fallTimer = new()
		{
			OneShot = true
		};
		AddChild(_fallTimer);
		_fallTimer.Timeout += () => FallTile();
		var polygonXs = Polygon.Select((vert) => vert.X + Position.X);
		var polygonYs = Polygon.Select((vert) => vert.Y + Position.Y);
		_player = new();
		AddChild(_player);

		_bottomLeftPosition = new(polygonXs.Min(), polygonYs.Max());
		_topRightPosition = new(polygonXs.Max(), polygonYs.Min());
		Width = _topRightPosition.X - _bottomLeftPosition.X;
		Height = _bottomLeftPosition.Y - _topRightPosition.Y;
		Center = new Vector2(Position.X + Width / 2, Position.Y + Height / 2);
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		// TODO check that mouse is within polygon bounds

		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			float eventX = mouseEvent.Position.X;
			float eventY = mouseEvent.Position.Y;
			if (
				_bottomLeftPosition.X <= eventX && eventX <= _topRightPosition.X &&
				_topRightPosition.Y <= eventY && eventY <= _bottomLeftPosition.Y
			)
			{
				PickupTile();
			}
		}
		;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <returns>True if the ceiling tile was able to fall, false otherwise</returns>
	public bool HangTile()
	{
		if (TileStatus != TileState.Stable)
		{
			return false;
		}
		TileStatus = TileState.Hanging;
		TweenTileHang();
		_fallTimer.Start(3);

		return true;
	}

	protected void TweenTileHang()
	{
		TweenTileHang(() => { });
	}

	protected void TweenTileHang(Action after)
	{
		_player.Stream = TileHangSound;
		_player.Play();

		MyTween?.Kill();
		MyTween = GetTree().CreateTween();
		MyTween.TweenMethod(
				Callable.From((float rotation) => Rotation = rotation),
				0f, 1.22173f, 1.0f)
				 .SetTrans(Tween.TransitionType.Elastic)
				 .SetEase(Tween.EaseType.Out);
		MyTween.Finished += after;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <returns>True if we are able to repair the hanging ceiling tile, false otherwise</returns>
	public bool PickupTile()
	{
		if (TileStatus != TileState.Hanging) return false;
		TileStatus = TileState.Stable;
		_fallTimer?.Stop();
		TweenTilePickup();
		return true;
	}

	protected void TweenTilePickup()
	{
		TweenTilePickup(() => { });
	}

	protected void TweenTilePickup(Action after)
	{
		_player.Stream = TilePickupSound;
		_player.Play();

		ShaderMaterial shader = (ShaderMaterial)Material;
		MyTween?.Kill();
		MyTween = GetTree().CreateTween();
		MyTween.TweenMethod(
				Callable.From((float rotation) => Rotation = rotation),
				 1.22173f, 0f, 0.5f)
				 .SetTrans(Tween.TransitionType.Quart)
				 .SetEase(Tween.EaseType.Out);
		MyTween.Finished += after;
	}

	public void FallTile()
	{
		TileStatus = TileState.Fallen;
		TweenTileFall();
		_fallTimer.QueueFree();
		EmitSignal(SignalName.CeilingTileFallen, Center);
	}

	protected void TweenTileFall()
	{
		TweenTileFall(() => { });
	}

	protected void TweenTileFall(Action after)
	{
		// Boost tile fall sound audio db
		_player.Stream = TileFallSound;
		_player.VolumeDb = 5;
		_player.Play();
		_player.Finished += () =>
		{
			_player.VolumeDb = 0;
		};

		MyTween?.Kill();
		MyTween = GetTree().CreateTween();
		var fallenPosition = new Vector2(Position.X, 600);
		MyTween.TweenMethod(Callable.From((Vector2 position) =>
		{
			Position = position;
		}), Position, fallenPosition, 1.0)
		.SetTrans(Tween.TransitionType.Sine)
		.SetEase(Tween.EaseType.In);
		MyTween.Finished += after;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
