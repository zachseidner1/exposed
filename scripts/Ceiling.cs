using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Ceiling : Node2D
{
	private List<CeilingTile> _tiles = new();

	private Timer timer;
	private Random random;


	[Export(PropertyHint.Range, "0,20,1")]
	public int CeilingDifficulty { get; set; } = 1;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		timer = GetNode<Timer>("../Timer");
		timer.Timeout += () => HangRandomTile();
		random = new Random();

		_tiles = GetTree().GetNodesInGroup("tiles").Cast<CeilingTile>().ToList();
		// timer.Start(3);
	}

	private void HangRandomTile()
	{
		int tileToHang = Random.Next(_tiles.Count);
		// // TODO this is not the best lol 
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
	}
}
