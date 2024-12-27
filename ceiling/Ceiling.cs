using Godot;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

public partial class Ceiling : Area2D
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
		PopulateCeiling();
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
	}

	public void PopulateCeiling()
	{
		for (int i = 0; i < 5; i++)
		{
			for (int j = 0; j < 5; j++)
			{
				CeilingTile tile = new()
				{
					Name = "tile " + i + j,
					Position = new Vector2(43 * i * 2 + 20 * j, 21 * 2 * j),
					Color = new Color(1, 1, 1),
					Size = new Vector2(80 - 4 * j, 160 - 4 * j),
				};
				_tiles.Add(tile);
				AddChild(tile);
			}
		}
	}




	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
