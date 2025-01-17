using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Ceiling : Node2D
{
	private List<CeilingTile> _tiles = new();

	private Timer timer;

	[Export]
	public Bat Bat;

	private double _elapsedTime;

	[Export]
	public Label TimeLabel;

	/// <summary>
	/// The length of one in-game hour, in seconds
	/// </summary>
	private readonly int _hourLength = 60;


	[Export(PropertyHint.Range, "0,20,1")]
	public int CeilingDifficulty { get; set; } = 1;
	private int _fallenTilesCount = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		timer = GetNode<Timer>("../Timer");
		timer.Timeout += () => HangRandomTile();

		_tiles = GetTree().GetNodesInGroup("tiles").Cast<CeilingTile>().ToList();

		timer.Start(3);
		foreach (var tile in _tiles)
		{
			tile.CeilingTileFallen += HandleCeilingTileFallen;
		}
	}

	private void HandleCeilingTileFallen(Vector2 tileCenter)
	{
		_fallenTilesCount++;
		if (_fallenTilesCount >= 3)
		{
			timer.Stop();
			timer.QueueFree();
			Bat.ScarePlayer(tileCenter);
		}
	}

	private void HangRandomTile()
	{
		int tileToHang = Random.Next(_tiles.Count);
		while (!_tiles[tileToHang].HangTile())
		{
			tileToHang = Random.Next(_tiles.Count);
		}
		timer.Start(GetNextHangDelay());
	}

	private double GetNextHangDelay()
	{
		double hangDelay = 1 - CeilingDifficulty / 50.0;
		hangDelay -= Random.Next(0f, 0.55f) * CeilingDifficulty * .05;
		hangDelay = Math.Clamp(hangDelay, 0.05, 1 - CeilingDifficulty / 50.0);
		return hangDelay;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_elapsedTime += delta;
		TimeLabel.Text = GetCurrentHour() + " AM";
		if (GetCurrentHour() == 6)
		{
			// TODO end game
		}
	}

	private int GetCurrentHour()
	{
		return (int)_elapsedTime / _hourLength + 1;
	}
}
