using Godot;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;

public partial class Ceiling : Node2D
{
	private List<CeilingTile> _tiles = new();

	private Timer timer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		timer = GetNode<Timer>("../Timer");
		timer.Timeout += () => HangRandomTile();

		_tiles = GetTree().GetNodesInGroup("tiles").Cast<CeilingTile>().ToList();
		timer.Start();
	}

	private void HangRandomTile()
	{
		Random rnd = new();
		int tileToHang = rnd.Next(_tiles.Count);
		// // TODO this is not the best lol
		while (!_tiles[tileToHang].HangTile())
		{
			tileToHang = rnd.Next(_tiles.Count);
		}
		timer.Start();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
