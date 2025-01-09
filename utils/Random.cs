
using System;

public class Random
{
  private readonly static System.Random _random = new();

  /// <summary>
  /// Returns a random float between the lower and upper bound, inclusive both 
  /// ways.
  /// </summary>
  /// <param name="lower">The lower bound</param>
  /// <param name="upper">The upper bound</param>
  /// <returns>a random float between the lower and upper bound, inclusive both 
  /// ways</returns>
  public static float Next(float lower, float upper)
  {
    return ((float)_random.NextDouble()) * (upper - lower) + lower;
  }

  /// <summary>
  /// Returns a random integer between the lower and upper bound, inclusive both
  /// ways.
  /// </summary>
  /// <param name="lowerBound"></param>
  /// <param name="upperBound"></param>
  /// <returns>a random integer between the lower and upper bound, inclusive both
  /// ways</returns> 
  public static int Next(int lowerBound, int upperBound)
  {
    return _random.Next(lowerBound, upperBound);
  }

  /// <summary>
  /// Returns a non-negative random integer that is less than the specified maximum.
  /// </summary>
  /// <param name="upperBound"></param>
  /// <returns>  A 32-bit signed integer that is greater than or equal to 0, and less than maxValue; that is, the range of return values ordinarily includes 0 but not maxValue. However, if maxValue equals 0, maxValue is returned.
  /// </returns>
  public static int Next(int upperBound)
  {
    return _random.Next(upperBound);
  }

}