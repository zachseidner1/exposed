using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Eyes : Sprite2D
{
	private List<CeilingTile> _tiles;

	private Timer _timer;

	private int _tileHidingUnder = -1;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_tiles = GetTree().GetNodesInGroup("tiles").Cast<CeilingTile>().ToList();
		_timer = new()
		{
			OneShot = true
		};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// TODO this logic is not the best, should use object visitor but this is a 
		// small scale game so I don't care that much 
		if (_tileHidingUnder != -1)
		{
			if (_tiles[_tileHidingUnder].TileStatus == CeilingTile.TileState.Hanging)
			{
				OnTileExposed();
			}
		}
	}

	public void HideUnderTile()
	{
		_tileHidingUnder = Random.Next(_tiles.Count);
		while (_tiles[_tileHidingUnder].TileStatus != CeilingTile.TileState.Stable)
		{
			_tileHidingUnder = Random.Next(_tiles.Count);
		}
		Position = _tiles[_tileHidingUnder].Center;
		Visible = true;
	}

	public void OnTileExposed()
	{
		_timer.Start();
		_timer.Timeout += () =>
		{
			var tween = GetTree().CreateTween();
			tween.TweenProperty(this, "modulate", new Color(0, 0, 0), 1.0);
			tween.Finished += () => { Visible = false; };
		};
	}
}
