using Godot;
using System;

public partial class Saving : Node
{
	/// <summary>
	/// Writes the next level the user needs to beat to the game storage
	/// </summary>
	/// <param name="level">The next level the user needs to beat</param>
	public static void WriteLevel(int level)
	{
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
		try
		{
			using var saveFile = FileAccess.Open("user://savegame.save", FileAccess.ModeFlags.Read);
			while (saveFile.GetPosition() < saveFile.GetLength())
			{
				return int.Parse(saveFile.GetLine());
			}
		}
		catch (Exception e)
		{
			GD.PushError(e);
		}
		return 1;
	}
}
