using Godot;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

public partial class Ceiling : Node2D
{
	private readonly List<CeilingTile> _tiles = new();

	private Vector2 _defaultSize = new(80, 40);

	public int ceilingWidth = 5;
	public int ceilingHeight = 5;

	private Timer timer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		timer = GetNode<Timer>("../Timer");
		timer.Timeout += () => HangRandomTile();

		timer.Start();
	}

	private void HangRandomTile()
	{
		int numTiles = ceilingHeight * ceilingWidth;

		Random rnd = new();
		int tileToHang = rnd.Next(ceilingHeight * ceilingWidth);
		// TODO this is not the best lol
		while (!_tiles[tileToHang].HangTile())
		{
			tileToHang = rnd.Next(numTiles);
		}
		timer.Start();
		var tests = GetTree().GetNodesInGroup("");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
