using Godot;
using System;
using System.Collections.ObjectModel;
using System.Linq;


public partial class CeilingTile : Polygon2D
{

	public enum TileState
	{
		Hanging,
		Fallen,
		Stable,
	}

	private Vector3 _defaultRotation = new(284.438f, 360f, 5.058f);
	private Vector3 _hangingRotation = new(284.438f, 288f, 5.058f);
	private float _currentRotation;
	public TileState TileStatus { get; private set; } = TileState.Stable;
	private Tween _tween;

	private Timer _fallTimer;

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
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Initialize timer
		_fallTimer = new();
		AddChild(_fallTimer);
		_fallTimer.Timeout += () => FallTile();
		var polygonXs = Polygon.Select((vert) => vert.X + Position.X);
		var polygonYs = Polygon.Select((vert) => vert.Y + Position.Y);

		_bottomLeftPosition = new(polygonXs.Min(), polygonYs.Max());
		_topRightPosition = new(polygonXs.Max(), polygonYs.Min());
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
			else if (TileStatus == TileState.Hanging)
			{
				GD.Print("x range: " + _bottomLeftPosition.X + ", " + _topRightPosition.X);
				GD.Print("y range: " + _topRightPosition.Y + ", " + _bottomLeftPosition.Y);
				GD.Print("mouse coords: " + mouseEvent.Position);
			}
		};
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

	private void TweenTileHang()
	{
		ShaderMaterial shader = (ShaderMaterial)Material;
		_tween?.Kill();
		_tween = GetTree().CreateTween();
		_tween.TweenMethod(
				Callable.From((float rotation) => SetShaderRotation(rotation)),
				0f, 1.22173f, 1.0f)
				 .SetTrans(Tween.TransitionType.Elastic)
				 .SetEase(Tween.EaseType.Out);
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

	private void TweenTilePickup()
	{
		ShaderMaterial shader = (ShaderMaterial)Material;
		_tween?.Kill();
		_tween = GetTree().CreateTween();
		_tween.TweenMethod(
				Callable.From((float rotation) => SetShaderRotation(rotation)),
				 1.22173f, 0f, 0.5f)
				 .SetTrans(Tween.TransitionType.Quart)
				 .SetEase(Tween.EaseType.Out);
	}

	private void FallTile()
	{
		TileStatus = TileState.Fallen;
		TweenTileFall();
		_fallTimer.QueueFree();
	}

	private void TweenTileFall()
	{
		_tween?.Kill();
		_tween = GetTree().CreateTween();
		var fallenPosition = new Vector2(Position.X, 500);
		_tween.TweenMethod(Callable.From((Vector2 position) =>
		{
			Position = position;
		}), Position, fallenPosition, 1.0)
		.SetTrans(Tween.TransitionType.Sine)
		.SetEase(Tween.EaseType.In);
	}

	/// <summary>
	/// Updates the shader parameters based on the rotation vector, and sets the 
	/// current rotation to said vector
	/// </summary>
	/// <param name="rotation">The rotation of the object</param>
	private void SetShaderRotation(float rotation)
	{
		Rotation = rotation;
		_currentRotation = rotation;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
