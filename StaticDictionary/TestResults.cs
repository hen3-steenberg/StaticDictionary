using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace StaticDictionary
{
	public struct PerformanceInfo
	{
		public string Description;
		public int ElementCount;
		public TimeSpan StaticDuration;
		public TimeSpan DictionaryDuration;
		public TimeSpan TotalTime;
		public bool StaticSuccess;
		public bool DynamicSuccess;

		public bool Success
		{
			get
			{
				return StaticSuccess && DynamicSuccess;
			}
		}
	}


	public class TestResults : IGrouping<int, PerformanceInfo>
	{
		private readonly IReadOnlyDictionary<int, string> testdict;
		private readonly IReadOnlyDictionary<int, string> dynamicdict;
		private List<PerformanceInfo> Results;
		public TestResults(IReadOnlyDictionary<int, string> _testdict) 
		{
			Results = new List<PerformanceInfo>();
			testdict = _testdict;
			Stopwatch sw = Stopwatch.StartNew();
			dynamicdict = testdict.Copy();
			sw.Stop();
			Console.WriteLine($"Creating a copy of a dictionary with {testdict.Count} elements, took {sw.Elapsed}");
		}

		public int Key => testdict.Count;

        public IEnumerable<PerformanceInfo> GetResults()
		{
			if(Results.Count == 0)
			{
				return RunTests();
			}
			else
			{
				return Results;
			}
		}

        private IEnumerable<PerformanceInfo> RunTests()
		{
			PerformanceInfo result = DictionaryTests.RandomAccessTest(testdict, dynamicdict);
			Results.Add(result);
			yield return result;

			result = DictionaryTests.Test1000Runs(DictionaryTests.RandomAccessTest, testdict, dynamicdict);
            Results.Add(result);
            yield return result;

            result = DictionaryTests.ContainsKey90PercentTest(testdict, dynamicdict);
            Results.Add(result);
            yield return result;

            result = DictionaryTests.Test1000Runs(DictionaryTests.ContainsKey90PercentTest, testdict, dynamicdict);
            Results.Add(result);
            yield return result;

        }

        public IEnumerator<PerformanceInfo> GetEnumerator()
		{
			return GetResults().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetResults().GetEnumerator();
		}

		private static readonly List<TestResults> AllTestResults = DictionaryTests.GetAllTestDictionaries().Select(dict => new TestResults(dict)).ToList();


        public static IEnumerable<TestResults> RunAllTests()
		{
			return AllTestResults;
		}

		public static void SaveResults()
		{
			//Save Result in markdown format
			using (StreamWriter MarkdownWriter = new StreamWriter("Results.md"))
			{
				MarkdownWriter.WriteLine("# Results");
				//Write table header
				MarkdownWriter.WriteLine("<table>");
				MarkdownWriter.WriteLine(@"<tr><th></th><th>Element Count</th><th collspan=""2"">Random Access</th><th collspan=""2"">Contains Key (90% hit)</th></tr>");
                MarkdownWriter.WriteLine(@"<tr><th></th><th>Runs</th><th>1</th><th>1000</th><th>1</th><th>1000</th></tr>");
                foreach (var test in AllTestResults)
				{
					MarkdownWriter.WriteLine("<tr>");
					
					MarkdownWriter.WriteLine(@$"<th>Static Duration</th><th rowspan=""2"">{test.Key}</th>");
                    StringBuilder Row2 = new StringBuilder();
					Row2.AppendLine("<tr>");
					Row2.AppendLine("<th>Dynamic Duration</th>");
                    foreach (var result in test.GetResults())
					{
                        MarkdownWriter.WriteLine($"<td>{result.StaticDuration}</td>");
						Row2.AppendLine($"<td>{result.DictionaryDuration}</td>");
					}
                    MarkdownWriter.WriteLine("</tr>");
					Row2.AppendLine("</tr>");
					MarkdownWriter.WriteLine(Row2);
                }
                MarkdownWriter.WriteLine("</table>");
            }

			using(StreamWriter CsvWriter = new StreamWriter("Results.csv"))
			{
				//Write Header
				CsvWriter.WriteLine("ElementCount, TestDescription, StaticSeconds, DynamicSeconds");
				foreach (var test in AllTestResults)
				{
					foreach (var result in test.GetResults())
					{
                        CsvWriter.WriteLine($"{test.Key}, {result.Description.Replace(',','_')}, {result.StaticDuration.TotalSeconds}, {result.DictionaryDuration.TotalSeconds}");
					}
				}
            }
		}
	}
}
