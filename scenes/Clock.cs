using Godot;
using System;

public partial class Clock : Label
{
	private double _elapsedTime = 0;

	[Export(PropertyHint.Range, "1,240,1")]
	public int SecondsPerNight;

	/// <summary>
	/// The previous 0-indexed game hour
	/// </summary>
	private int _prevHour = 0;

	[Signal]
	public delegate void Hour3EventHandler();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Text = "1 AM";
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_elapsedTime += delta;
		if ((int)_elapsedTime / SecondsPerNight != _prevHour)
		{
			_prevHour = (int)_elapsedTime / SecondsPerNight;
			Text = _prevHour + 1 + " AM";
			switch (_prevHour + 1)
			{
				case 3:
					EmitSignal(SignalName.Hour3);
					break;
				case 6:
					Saving.WriteLevel(Saving.GetLevel() + 1);
					GetTree().ChangeSceneToFile("res://scenes/night_finished.tscn");
					break;
			}
		}
	}
}
