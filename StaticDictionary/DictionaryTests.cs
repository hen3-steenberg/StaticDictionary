using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaticDictionary
{
	public static class DictionaryTests
	{
		public delegate PerformanceInfo DictionaryTest(IReadOnlyDictionary<int, string> sdict, IReadOnlyDictionary<int, string> ddict);
		public static IEnumerable<IReadOnlyDictionary<int, string>> GetAllTestDictionaries()
		{
			yield return TestDictionary1.CreateStaticDictionary();
			yield return TestDictionary2.CreateStaticDictionary();
			yield return TestDictionary3.CreateStaticDictionary();
			yield return TestDictionary4.CreateStaticDictionary();
			yield return TestDictionary5.CreateStaticDictionary();
			yield return TestDictionary6.CreateStaticDictionary();
			yield return TestDictionary7.CreateStaticDictionary();
			yield return TestDictionary8.CreateStaticDictionary();
			yield return TestDictionary9.CreateStaticDictionary();

			yield return TestDictionary10.CreateStaticDictionary();
			yield return TestDictionary20.CreateStaticDictionary();
			yield return TestDictionary30.CreateStaticDictionary();
			yield return TestDictionary40.CreateStaticDictionary();
			yield return TestDictionary50.CreateStaticDictionary();
			yield return TestDictionary60.CreateStaticDictionary();
			yield return TestDictionary70.CreateStaticDictionary();
			yield return TestDictionary80.CreateStaticDictionary();
			yield return TestDictionary90.CreateStaticDictionary();

			yield return TestDictionary100.CreateStaticDictionary();
			yield return TestDictionary200.CreateStaticDictionary();
			yield return TestDictionary300.CreateStaticDictionary();
			yield return TestDictionary400.CreateStaticDictionary();
			yield return TestDictionary500.CreateStaticDictionary();
			yield return TestDictionary600.CreateStaticDictionary();
			yield return TestDictionary700.CreateStaticDictionary();
			yield return TestDictionary800.CreateStaticDictionary();
			yield return TestDictionary900.CreateStaticDictionary();

			yield return TestDictionary1000.CreateStaticDictionary();
			yield return TestDictionary2000.CreateStaticDictionary();
			yield return TestDictionary3000.CreateStaticDictionary();
			yield return TestDictionary4000.CreateStaticDictionary();
			yield return TestDictionary5000.CreateStaticDictionary();
			yield return TestDictionary6000.CreateStaticDictionary();
			yield return TestDictionary7000.CreateStaticDictionary();
			yield return TestDictionary8000.CreateStaticDictionary();
			yield return TestDictionary9000.CreateStaticDictionary();
		}

		public static Dictionary<TKey, TValue> Copy<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dict) where TKey : notnull
		{
			Dictionary<TKey, TValue> res = new Dictionary<TKey, TValue>();
			foreach (var entry in dict)
			{
				res[entry.Key] = entry.Value;
			}
			return res;
		}

		public static PerformanceInfo Test1000Runs(DictionaryTest test, IReadOnlyDictionary<int, string> sdict, IReadOnlyDictionary<int, string> ddict)
		{
			Stopwatch total = Stopwatch.StartNew();
			PerformanceInfo Res = new PerformanceInfo()
			{
				DictionaryDuration = TimeSpan.Zero,
				StaticDuration = TimeSpan.Zero,
				StaticSuccess = true,
				DynamicSuccess = true
			};
			var result = test.Invoke(sdict, ddict);
			Res.StaticDuration += result.StaticDuration;
			Res.DictionaryDuration += result.DictionaryDuration;
			Res.StaticSuccess &= result.StaticSuccess;
			Res.DynamicSuccess &= result.DynamicSuccess;
			
			Res.Description = result.Description + ", 1000 Runs.";
			for (int i = 1; i < 1000; ++i)
			{
				result = test.Invoke(sdict, ddict);
				Res.StaticDuration += result.StaticDuration;
				Res.DictionaryDuration += result.DictionaryDuration;
				Res.StaticSuccess &= result.StaticSuccess;
				Res.DynamicSuccess &= result.DynamicSuccess;
			}
			total.Stop();
			Res.TotalTime = total.Elapsed;
			return Res;
		}

		public static PerformanceInfo RandomAccessTest(IReadOnlyDictionary<int, string> sdict, IReadOnlyDictionary<int, string> ddict)
		{
			Stopwatch total = Stopwatch.StartNew();
			const int num_tests = 1000000;
			int length = sdict.Count;
			int[] access = new int[num_tests];
			string[] results = new string[num_tests];
			for (int i = 0; i < num_tests; i++)
			{
				access[i] = Random.Shared.Next(length) + 1;
				results[i] = sdict[access[i]];
			}

			bool staticSuccess = true;

			Stopwatch stopwatch = Stopwatch.StartNew();

			for (int i = 0; i < num_tests; i++)
			{
				if (sdict[access[i]] != results[i])
				{
					staticSuccess = false;
					break;
				}
			}

			stopwatch.Stop();

			TimeSpan sdictTime = stopwatch.Elapsed;

			bool dynamicSuccess = true;

			stopwatch.Restart();

			for (int i = 0; i < num_tests; i++)
			{
				if (ddict[access[i]] != results[i])
				{
					dynamicSuccess = false;
					break;
				}
			}

			stopwatch.Stop();

			TimeSpan ddictTime = stopwatch.Elapsed;
			total.Stop();

			return new PerformanceInfo { Description = "Random Access Test", ElementCount = length, StaticDuration = sdictTime, DictionaryDuration = ddictTime, TotalTime = total.Elapsed, StaticSuccess = staticSuccess, DynamicSuccess = dynamicSuccess };
		}

		public static PerformanceInfo ContainsKey90PercentTest(IReadOnlyDictionary<int, string> sdict, IReadOnlyDictionary<int, string> ddict)
		{
			Stopwatch total = Stopwatch.StartNew();
			const int num_tests = 1000000;
			int length = (int)(sdict.Count * 1.1);
			int[] access = new int[num_tests];
			for (int i = 0; i < num_tests; i++)
			{
				access[i] = Random.Shared.Next(length) + 1;
			}

			int containeds = 0;
			int notContaineds = 0;

			Stopwatch stopwatch = Stopwatch.StartNew();

			for (int i = 0; i < num_tests; i++)
			{
				if (sdict.ContainsKey(access[i]))
				{
					++containeds;
				}
				else
				{
					++notContaineds;
				}
			}

			stopwatch.Stop();

			TimeSpan sdictTime = stopwatch.Elapsed;
			int containedd = 0;
			int notContainedd = 0;
			stopwatch.Restart();

			for (int i = 0; i < num_tests; i++)
			{
				if (ddict.ContainsKey(access[i]))
				{
					++containedd;
				}
				else
				{
					++notContainedd;
				}
			}

			stopwatch.Stop();

			TimeSpan ddictTime = stopwatch.Elapsed;

			bool success = (containeds == containedd) && (notContaineds == notContainedd);

			total.Stop();

			return new PerformanceInfo { Description = "ContainsKey test 90% hit Test", ElementCount = length, StaticDuration = sdictTime, DictionaryDuration = ddictTime, TotalTime = total.Elapsed, StaticSuccess = success, DynamicSuccess = success };
		}
	}
}
