using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomString
{
	public static partial class ComparisonMetrics
	{
		public static double GetCustomEqualityScore(this string source, string target, params CustomStringComparisonOptions[] options)
		{
			List<double> comparisonResults = new List<double>();

			if (!options.Contains(CustomStringComparisonOptions.CaseSensitive))
			{
				source = source.Capitalize();
				target = target.Capitalize();
			}

			// Min: 0    Max: source.Length = target.Length
			if (options.Contains(CustomStringComparisonOptions.UseHammingDistance))
			{
				if (source.Length == target.Length)
				{
					comparisonResults.Add(source.HammingDistance(target)/target.Length);
				}
			}

			// Min: 0    Max: 1
			if (options.Contains(CustomStringComparisonOptions.UseJaccardDistance))
			{
				comparisonResults.Add(source.JaccardDistance(target));
			}

			// Min: 0    Max: 1
			if (options.Contains(CustomStringComparisonOptions.UseJaroDistance))
			{
				comparisonResults.Add(source.JaroDistance(target));
			}

			// Min: 0    Max: 1
			if (options.Contains(CustomStringComparisonOptions.UseJaroWinklerDistance))
			{
				comparisonResults.Add(source.JaroWinklerDistance(target));
			}

			// Min: 0    Max: LevenshteinDistanceUpperBounds - LevenshteinDistanceLowerBounds
			// Min: LevenshteinDistanceLowerBounds    Max: LevenshteinDistanceUpperBounds
			if (options.Contains(CustomStringComparisonOptions.UseNormalizedLevenshteinDistance))
			{
				comparisonResults.Add(Convert.ToDouble(source.NormalizedLevenshteinDistance(target))/
											 Convert.ToDouble((Math.Max(source.Length, target.Length) - source.LevenshteinDistanceLowerBounds(target))));
			}
			else if (options.Contains(CustomStringComparisonOptions.UseLevenshteinDistance))
			{
				comparisonResults.Add(Convert.ToDouble(source.LevenshteinDistance(target))/
											 Convert.ToDouble(source.LevenshteinDistanceUpperBounds(target)));
			}

			if (options.Contains(CustomStringComparisonOptions.UseLongestCommonSubsequence))
			{
				comparisonResults.Add(1 -
											 Convert.ToDouble((source.LongestCommonSubsequence(target).Length)/
																	Convert.ToDouble(Math.Min(source.Length, target.Length))));
			}

			if (options.Contains(CustomStringComparisonOptions.UseLongestCommonSubstring))
			{
				comparisonResults.Add(1 -
											 Convert.ToDouble((source.LongestCommonSubstring(target).Length)/
																	Convert.ToDouble(Math.Min(source.Length, target.Length))));
			}

			// Min: 0    Max: 1
			if (options.Contains(CustomStringComparisonOptions.UseSorensenDiceDistance))
			{
				comparisonResults.Add(source.SorensenDiceDistance(target));
			}

			// Min: 0    Max: 1
			if (options.Contains(CustomStringComparisonOptions.UseOverlapCoefficient))
			{
				comparisonResults.Add(1 - source.OverlapCoefficient(target));
			}

			// Min: 0    Max: 1
			if (options.Contains(CustomStringComparisonOptions.UseRatcliffObershelpSimilarity))
			{
				comparisonResults.Add(1 - source.RatcliffObershelpSimilarity(target));
			}

			return comparisonResults.Average();
		}


		public static bool ApproximatelyEquals(this string source, string target, CustomStringComparisonTolerance tolerance, params CustomStringComparisonOptions[] options)
		{
			if (options.Length == 0)
			{
				return false;
			}

			var score = source.GetCustomEqualityScore(target, options);

			if (tolerance == CustomStringComparisonTolerance.Strong)
			{
				if (score < 0.25)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			else if (tolerance == CustomStringComparisonTolerance.Normal)
			{
				if (score < 0.5)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			else if (tolerance == CustomStringComparisonTolerance.Weak)
			{
				if (score < 0.75)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			else if (tolerance == CustomStringComparisonTolerance.Manual)
			{
				if (score > 0.6)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				return false;
			}
		}
	}
}
