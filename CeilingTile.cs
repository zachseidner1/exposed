using Godot;
using System;


public partial class CeilingTile : ColorRect
{
	public enum TileState
	{
		Hanging,
		Fallen,
		Stable,
	}

	private Vector3 _defaultRotation = new(284.438f, 360f, 5.058f);
	private Vector3 _hangingRotation = new(284.438f, 288f, 5.058f);
	private Vector3 _currentRotation;
	public TileState TileStatus { get; private set; } = TileState.Stable;
	private string _shaderPath = "res://CeilingTile.gdshader";
	private Tween _tween;

	private Timer _fallTimer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Shader prep
		var shader = GD.Load<Shader>(_shaderPath);
		Material = new ShaderMaterial
		{
			Shader = shader
		};
		SetShaderRotation(_defaultRotation);
		// Connect mouse input
		GuiInput += (InputEvent @event) =>
		{
			if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
			{
				PickupTile();
			};
		};
		// Initialize timer
		_fallTimer = new();
		AddChild(_fallTimer);
		_fallTimer.Timeout += () => FallTile();
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
				Callable.From((Vector3 rotation) => SetShaderRotation(rotation)),
				 _currentRotation, _hangingRotation, 1.0)
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
		TileStatus = TileState.Hanging;
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
				Callable.From((Vector3 rotation) => SetShaderRotation(rotation)),
				 _currentRotation, _defaultRotation, 0.5)
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
		var fallenPosition = new Vector2(Position.X, Position.Y + 80);
		_tween.TweenMethod(Callable.From((Vector2 position) =>
		{
			Position = position;
		}), Position, fallenPosition, 1.0)
		.SetTrans(Tween.TransitionType.Circ)
		.SetEase(Tween.EaseType.In);
	}

	/// <summary>
	/// Updates the shader parameters based on the rotation vector, and sets the 
	/// current rotation to said vector
	/// </summary>
	/// <param name="rotation">The rotation of the object</param>
	private void SetShaderRotation(Vector3 rotation)
	{
		ShaderMaterial shader = (ShaderMaterial)Material;
		shader.SetShaderParameter("xDegrees", rotation.X);
		shader.SetShaderParameter("yDegrees", rotation.Y);
		shader.SetShaderParameter("zDegrees", rotation.Z);
		_currentRotation = rotation;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
