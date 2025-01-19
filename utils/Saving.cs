using Godot;
using System;

public partial class Saving : Node
{
	private static int _currentLevel = -1;
	/// <summary>
	/// Writes the next level the user needs to beat to the game storage
	/// </summary>
	/// <param name="level">The next level the user needs to beat</param>
	public static void WriteLevel(int level)
	{
		_currentLevel = level;
		using var saveFile = FileAccess.Open("user://savegame.save", FileAccess.ModeFlags.Write);
		saveFile.StoreLine("" + level);
	}
	/// <summary>
	/// Gets the level the user needs to beat, assumed the user starts at the first
	/// level in the result of any issues with file reading.
	/// </summary>
	/// <returns>Integer representing the next level the user has to beat</returns>
	public static int GetLevel()
	{
		if (_currentLevel == -1)
		{
			try
			{
				using var saveFile = FileAccess.Open("user://savegame.save", FileAccess.ModeFlags.Read);
				if (saveFile.GetPosition() < saveFile.GetLength())
				{
					_currentLevel = int.Parse(saveFile.GetLine());
					return _currentLevel;
				}
			}
			catch (Exception e)
			{
				GD.PushError(e);
			}
			_currentLevel = 1;
		}
		return _currentLevel;
	}
}
