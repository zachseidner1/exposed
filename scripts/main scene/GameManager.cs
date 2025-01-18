using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

public partial class GameManager : Node2D
{
	private List<CeilingTile> _tiles = new();

	[Export]
	public Bat Bat;

	[Export]
	public Eyes Eyes;

	[Export]
	public Cat Cat;

	[Export]
	public Phone Phone;

	private double _elapsedTime;

	[Export]
	public Label TimeLabel;

	/// <summary>
	/// The length of one in-game hour, in seconds
	/// </summary>
	private readonly int _hourLength = 20;


	[Export(PropertyHint.Range, "0,20,1")]
	public int CeilingDifficulty { get; set; } = 1;

	[Export]
	public Timer CeilingHangTimer;
	private int _fallenTilesCount = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Visible;
		InitGameObjects();
		BeginLevel1();
	}

	private void InitGameObjects()
	{
		CeilingHangTimer.Timeout += () => HangRandomTile();
		_tiles = GetTree().GetNodesInGroup("tiles").Cast<CeilingTile>().ToList();

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
			CeilingHangTimer.Stop();
			CeilingHangTimer.QueueFree();
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
			// TODO end level
		}
	}

	private int GetCurrentHour()
	{
		return (int)_elapsedTime / _hourLength + 1;
	}

	private void BeginLevel1()
	{
		Phone.BeginCall();
	}

}
