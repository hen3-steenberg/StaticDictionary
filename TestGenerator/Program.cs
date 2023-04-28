using System.IO;
namespace TestGenerator
{
	internal class Program
	{
		static int[] testsizes = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000, 2000, 3000, 4000, 5000, 6000, 7000, 8000, 9000 };
		static void Main(string[] args)
		{
			Console.WriteLine("Starting");
			using (StreamWriter sw = new StreamWriter("TestDictionaryDefinitions.cs"))
			{
				sw.WriteLine(TestGenerator.GenerateTestFile(testsizes));
			}
			Console.WriteLine("Done.");
		}
	}
}