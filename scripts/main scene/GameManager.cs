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
	private readonly int _hourLength = 30;


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
		BeginPhoneLevel();
		switch (Saving.GetLevel())
		{
			case 1:
				break;
			case 2:
				StartCeilingAtDifficulty(1);
				break;
		}
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
		if (tileToHang == Eyes.TileHidingUnder)
		{
			Eyes.OnTileExposed();
		}
		CeilingHangTimer.Start(GetNextHangDelay());
	}

	private double GetNextHangDelay()
	{
		double maxHangTime = Math.Pow(0.8, CeilingDifficulty - 10) + 0.7;
		double minHangTime = Math.Pow(0.8, CeilingDifficulty - .8) + 0.05;
		double hangTime = Random.Next(minHangTime, maxHangTime);

		if (CeilingDifficulty >= 10 && hangTime < (maxHangTime + minHangTime) / 2.0)
		{
			hangTime = Random.Next(minHangTime, maxHangTime);
		}

		GD.Print("Chosen delay: " + Math.Clamp(hangTime, 0.05, 5) + "from difficulty " + CeilingDifficulty);
		return Math.Clamp(hangTime, 0.05, 5);
	}

	private void BeginPhoneLevel()
	{
		Phone.BeginCall();
	}

	private void OnPhoneCallFinished()
	{
		if (Saving.GetLevel() == 1)
		{
			StartCeilingAtDifficulty(1);
		}
	}

	private void StartCeilingAtDifficulty(int difficulty)
	{
		CeilingDifficulty = difficulty;
		CeilingHangTimer.Start(GetNextHangDelay());
	}

	private void OnHour2Reached()
	{
		switch (Saving.GetLevel())
		{
			case 1:
				break;
			case 2:
				CeilingDifficulty = 9;
				break;
		}
	}
	private void OnHour3Reached()
	{
		switch (Saving.GetLevel())
		{
			case 1:
				Eyes.HideUnderTile();
				CeilingDifficulty = 1;
				CeilingHangTimer.Start(GetNextHangDelay());
				break;
			case 2:
				CeilingDifficulty = 10;
				break;
		}
	}
	private void OnHour4Reached()
	{
		switch (Saving.GetLevel())
		{
			case 1:
				GD.Print("Upping ceiling difficulty to 5");
				CeilingDifficulty = 5;
				break;
		}
	}

	private void OnHour5Reached()
	{
		switch (Saving.GetLevel())
		{
			case 1:
				GD.Print("Upping ceiling difficulty to 9");
				CeilingDifficulty = 9;
				break;
			case 2:
				CeilingDifficulty = 12;
				break;
		}
	}

}
