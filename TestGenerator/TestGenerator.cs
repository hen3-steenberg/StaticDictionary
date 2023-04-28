using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGenerator
{
	public static class TestGenerator
	{
		private static IEnumerable<string> GetKeys(int num_entries)
		{
			for(int i = 0; i < num_entries; i++)
			{
				yield return $"{i + 1}";
			}
		}

		private static IEnumerable<string> GetValues(int num_entries)
		{
			for (int i = 0; i < num_entries; i++)
			{
				int number = i + 1;
				yield return $"\"{number.PrintNumber()}\"";
			}
		}
		private static string GenerateKeys(int num_entries)
		{
			return $"public static int[] Keys = {{ {String.Join(", ",GetKeys(num_entries))} }};";
		}

		private static string GenerateValues(int num_entries)
		{
			return $"public static string[] Values = {{ {String.Join(", ", GetValues(num_entries))} }};";
		}
		public static string GenerateTest(int num_entries)
		{
			string res = $@"public partial class TestDictionary{num_entries} : IStaticDictionaryFactoryDefinition<int, string>
	{{
		{GenerateKeys(num_entries)}

		{GenerateValues(num_entries)}
	}}";
			return res;
		}

		public static string GenerateTestFile(IEnumerable<int> TestSizes)
		{
			StringBuilder SourceBuilder = new StringBuilder();
			string header = @"using System;
using StaticDictionary.Interface;

namespace StaticDictionary
{";
			SourceBuilder.AppendLine(header);

			foreach(int test_size in TestSizes)
			{
				SourceBuilder.AppendLine(GenerateTest(test_size));
				SourceBuilder.AppendLine();
			}

			SourceBuilder.AppendLine("}");
			return SourceBuilder.ToString();
		}
	}
}
