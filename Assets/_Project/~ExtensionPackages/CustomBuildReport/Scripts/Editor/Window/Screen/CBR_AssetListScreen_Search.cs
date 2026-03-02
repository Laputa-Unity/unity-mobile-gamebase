using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FuzzyString;

namespace CustomBuildReport.Window.Screen
{
	public partial class AssetList
	{
		CustomBuildReport.SizePart[] _searchResults;

		const double SEARCH_DELAY = 0.75f;
		double _lastSearchTime;
		string _lastSearchText = string.Empty;

		string _searchTextInput = string.Empty;

		int _searchViewOffset;

		bool _showSearchOptions;
		Rect _searchTextfieldRect;

		// Search algorithms that will weigh in for the comparison
		readonly FuzzyStringComparisonOptions[] _searchOptions =
		{
			FuzzyStringComparisonOptions.UseOverlapCoefficient,
			FuzzyStringComparisonOptions.UseLongestCommonSubsequence,
			FuzzyStringComparisonOptions.UseLongestCommonSubstring
		};

		void ClearSearch()
		{
			_searchTextInput = "";
			_lastSearchText = null;
			_searchResults = null;
		}

		void UpdateSearch(double timeNow, BuildInfo buildReportToDisplay, CustomBuildReport.AssetBundleSession assetBundleSession)
		{
			if (string.IsNullOrEmpty(_searchTextInput) && !string.IsNullOrEmpty(_lastSearchText))
			{
				// cancel search
				ClearSearch();
				if (buildReportToDisplay != null)
				{
					buildReportToDisplay.FlagOkToRefresh();
				}

				_searchViewOffset = 0;
			}
			else if ((timeNow - _lastSearchTime >= SEARCH_DELAY) &&
			         !_searchTextInput.Equals(_lastSearchText, StringComparison.Ordinal))
			{
				UpdateSearchNow(buildReportToDisplay, assetBundleSession);
				_lastSearchTime = timeNow;
			}
		}

		public void UpdateSearchNow(BuildInfo buildReportToDisplay, CustomBuildReport.AssetBundleSession assetBundleSession)
		{
			if (string.IsNullOrEmpty(_searchTextInput))
			{
				return;
			}

			// update search
			_lastSearchText = _searchTextInput;
			_lastSearchTime = EditorApplication.timeSinceStartup;

			if (buildReportToDisplay != null)
			{
				Search(_lastSearchText, CustomBuildReport.Options.SearchType, CustomBuildReport.Options.SearchFilenameOnly, CustomBuildReport.Options.SearchCaseSensitive, buildReportToDisplay, assetBundleSession);
				buildReportToDisplay.FlagOkToRefresh();
			}

			_searchViewOffset = 0;
			_currentSortType = CustomBuildReport.AssetList.SortType.None;
		}

		void Search(string searchText, SearchType searchType, bool searchFilenameOnly, bool caseSensitive, BuildInfo buildReportToDisplay, CustomBuildReport.AssetBundleSession assetBundleSession)
		{
			if (string.IsNullOrEmpty(searchText))
			{
				_searchResults = null;
				return;
			}

			CustomBuildReport.AssetList list = GetAssetListToDisplay(buildReportToDisplay, assetBundleSession);


			CustomBuildReport.FileFilterGroup filter = buildReportToDisplay.FileFilters;

			if (CustomBuildReport.Options.ShouldUseConfiguredFileFilters())
			{
				filter = _configuredFileFilterGroup;
			}

			List<CustomBuildReport.SizePart> searchResults = new List<CustomBuildReport.SizePart>();


			CustomBuildReport.SizePart[] assetListToSearchFrom = list.GetListToDisplay(filter);

			var options = caseSensitive ? System.Text.RegularExpressions.RegexOptions.None : System.Text.RegularExpressions.RegexOptions.IgnoreCase;

			for (int n = 0; n < assetListToSearchFrom.Length; ++n)
			{
				string input;
				if (searchFilenameOnly)
				{
					input = assetListToSearchFrom[n].Name.GetFileNameOnly();
				}
				else
				{
					input = assetListToSearchFrom[n].Name;
				}

				bool assetIsMatch;

				if (string.IsNullOrEmpty(input))
				{
					assetIsMatch = false;
				}
				else
				{
					switch (searchType)
					{
						case SearchType.Regex:
							try
							{
								assetIsMatch = System.Text.RegularExpressions.Regex.IsMatch(input, searchText, options);
							}
							catch (ArgumentException)
							{
								assetIsMatch = false;
							}
							break;
						case SearchType.Fuzzy:
							assetIsMatch = IsANearStringMatch(input, searchText);
							break;
						default:
							// default is SearchType.Basic
							assetIsMatch = System.Text.RegularExpressions.Regex.IsMatch(input, CustomBuildReport.Util.WildCardToRegex(searchText), options);
							break;
					}
				}

				if (assetIsMatch)
				{
					searchResults.Add(assetListToSearchFrom[n]);
				}
			}

			if (searchResults.Count > 0)
			{
				searchResults.Sort((a, b) =>
					GetFuzzyEqualityScore(searchText, a.Name).CompareTo(GetFuzzyEqualityScore(searchText, b.Name)));
			}

			_searchResults = searchResults.ToArray();
		}

		void SortBySearchRank(CustomBuildReport.SizePart[] assetList, string searchText)
		{
			if (assetList.Length <= 0)
			{
				return;
			}

			System.Array.Sort(assetList, (entry1, entry2) =>
				GetFuzzyEqualityScore(searchText, entry1.Name)
					.CompareTo(GetFuzzyEqualityScore(searchText, entry2.Name)));
		}

		bool IsANearStringMatch(string source, string target)
		{
			if (string.IsNullOrEmpty(target))
			{
				return false;
			}

			// Choose the relative strength of the comparison - is it almost exactly equal? or is it just close?
			const FuzzyStringComparisonTolerance TOLERANCE = FuzzyStringComparisonTolerance.Strong;

			// Get a boolean determination of approximate equality
			return source.ApproximatelyEquals(target, TOLERANCE, _searchOptions);
		}

		double GetFuzzyEqualityScore(string source, string target)
		{
			if (string.IsNullOrEmpty(target))
			{
				return 0;
			}

			return source.GetFuzzyEqualityScore(target, _searchOptions);
		}
	}
}