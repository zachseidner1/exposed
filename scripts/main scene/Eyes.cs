using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Eyes : Sprite2D
{
	private List<CeilingTile> _tiles;

	private Timer _timer;

	public int TileHidingUnder = -1;

	[Export]
	public AudioStreamPlayer Player;

	[Export]
	public AudioStream ScareSoundEffect;

	private Boolean _disabled;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_tiles = GetTree().GetNodesInGroup("tiles").Cast<CeilingTile>().ToList();
		_timer = new()
		{
			OneShot = true
		};
		AddChild(_timer);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void HideUnderTile()
	{
		TileHidingUnder = Random.Next(_tiles.Count);
		while (_tiles[TileHidingUnder].TileStatus != CeilingTile.TileState.Stable)
		{
			TileHidingUnder = Random.Next(_tiles.Count);
		}
		Position = _tiles[TileHidingUnder].Center;
		Visible = true;
	}

	public void OnTileExposed()
	{
		if (_disabled) return;

		Player.Stream = ScareSoundEffect;
		Player.Play(2.64F);
		_timer.Start(1);
		_disabled = true;
		_timer.Timeout += () =>
		{
			var tween = GetTree().CreateTween();
			tween.TweenProperty(this, "modulate", new Color(0, 0, 0, 0), 1.0);
			tween.Finished += () => { Visible = false; };
		};
	}
}
