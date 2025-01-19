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
		Disabled,
	}

	[Export(PropertyHint.Range, "0,20,1")]
	public int CatDifficulty { get; set; } = 1;
	public CatState CatStatus = CatState.Disabled;

	private List<CeilingTile> _tiles = new();

	private Timer _jumpTimer;

	private Tween _tween;

	private Tween _numMeowsTween;

	[Export]
	public Texture2D Jumping;
	[Export]
	private Texture2D Falling;
	[Export]
	private Texture2D Sitting;

	[Export]
	public AudioStreamPlayer2D MeowPlayer;

	[Export]
	public AudioStream CatMeow;

	[Export]
	public AudioStream CatGrowl;

	private int _numMeows;

	[Export]
	public Label MeowLabel;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Visible = false;
		_tiles = GetTree().GetNodesInGroup("tiles").Cast<CeilingTile>().ToList();
		_jumpTimer = new()
		{
			OneShot = true
		};
		AddChild(_jumpTimer);

		_jumpTimer.Timeout += Jump;

		if (Saving.GetLevel() > 1)
		{
			InitSitting();
		}
	}

	public void StartCatAtDifficulty(int difficulty)
	{
		CatDifficulty = difficulty;
		Position = new(317, 380);
		_jumpTimer.Start(GetNextJumpTime());
	}

	private void InitSitting()
	{
		Visible = true;
		Texture = Sitting;
		CatStatus = CatState.Sitting;
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
		Texture = Jumping;

		int tileToJumpTo = Random.Next(_tiles.Count);
		while (_tiles[tileToJumpTo].TileStatus != CeilingTile.TileState.Stable)
		{
			tileToJumpTo = Random.Next(_tiles.Count);
		}

		Vector2 targetPosition = _tiles[tileToJumpTo].Center;

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
		Texture = Falling;
		_tween.TweenMethod(
			Callable.From((Vector2 pos) => Position = pos),
			Position, new Vector2(Math.Clamp(Position.X, 144, 456), 370), 1.0f);
		_tween.Finished += () =>
		{
			Texture = Sitting;
			CatStatus = CatState.Sitting;
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
			MeowPlayer.Stream = CatGrowl;
			MeowPlayer.Play(0F);
			Fall();
		}
		else if (CatStatus == CatState.Sitting)
		{
			_numMeows++;
			MeowPlayer.Stream = CatMeow;
			MeowPlayer.Play(3.19F);
			Label MeowEffectLabel = (Label)MeowLabel.Duplicate();
			AddChild(MeowEffectLabel);

			MeowEffectLabel.Text = "+" + _numMeows + " meows!";
			MeowEffectLabel.Visible = true;
			MeowEffectLabel.Position = new Vector2(MeowLabel.Position.X, 0);

			var numMeowsTween = GetTree().CreateTween();
			numMeowsTween.SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Quad);
			numMeowsTween.TweenProperty(MeowEffectLabel, "modulate", new Color(0, 0, 0, 0), 1.0);
			numMeowsTween.Parallel().TweenProperty(MeowEffectLabel,
			 "position",
			 new Vector2(MeowEffectLabel.Position.X, MeowEffectLabel.Position.Y - 100),
				1.0);

			numMeowsTween.Finished += () => { MeowEffectLabel.QueueFree(); };

		}
	}

	private void OnHour3Reached()
	{
		if (Saving.GetLevel() == 2)
		{
			StartCatAtDifficulty(5);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
