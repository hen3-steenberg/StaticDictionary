// See https://aka.ms/new-console-template for more information
using StaticDictionary;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text;

public static class Program
{
	static ConcurrentQueue<string> OutputQueue = new ConcurrentQueue<string>();

	static CancellationTokenSource TokenSrc = new CancellationTokenSource();

	static void OutputWriter()
	{
		CancellationToken cancellationToken = TokenSrc.Token;
		while((!cancellationToken.IsCancellationRequested) || !OutputQueue.IsEmpty)
		{
			if(OutputQueue.TryDequeue(out string output))
			{
				Console.WriteLine(output);
			}
			else
			{
				Thread.Sleep(100);
			}
		}
	}
	static void Main()
	{
		Thread writer = new Thread(OutputWriter);
		writer.Start();

		var loopResult = Parallel.ForEach(
			TestResults.RunAllTests(),
			new ParallelOptions { CancellationToken = TokenSrc.Token, MaxDegreeOfParallelism = 4 } , 
			(TestResult) =>
		{
			StringBuilder outp = new StringBuilder();
			outp.AppendLine($"====== Element Count : {TestResult.Key} =======");
			foreach (var result in TestResult)
			{
				outp.AppendLine($"\t{result.Description}");
				outp.AppendLine($"\tStatic Dictionary = {result.StaticDuration}");
				outp.AppendLine($"\tSystem.Dictionary = {result.DictionaryDuration}");
				outp.AppendLine($"\tTotal Time = {result.TotalTime}");
				outp.AppendLine($"\tSuccesses = {result.Success}");
				outp.AppendLine();
			}
			outp.AppendLine();

			OutputQueue.Enqueue( outp.ToString() );
		});

		Thread.Sleep(1000);
		TokenSrc.Cancel();
		writer.Join();
		TestResults.SaveResults();
	}
}
